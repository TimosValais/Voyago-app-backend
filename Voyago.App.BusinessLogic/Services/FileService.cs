using MassTransit;
using Minio;
using Minio.DataModel.Args;
using Minio.DataModel.Response;
using Minio.Exceptions;
using System.Reactive.Linq;
using Voyago.App.BusinessLogic.Mappings;
using Voyago.App.Contracts.Messages;
using Voyago.App.DataAccessLayer.Repositories;

namespace Voyago.App.BusinessLogic.Services;

public class FileService : IFileService, IConsumer<UserProfilePictureUpdateMessage>, IConsumer<TaskFileUpdateMessage>
{
    private readonly IMinioClient _fileClient;
    private readonly IFlightBookingTaskRepository _flightBookingTaskRepository;
    private readonly IGeneralBookingTaskRepository _generalBookingTaskRepository;
    private readonly IHotelBookingTaskRepository _hotelBookingTaskRepository;
    private readonly IOtherTaskRepository _otherTaskRepository;
    private readonly IPlanningTaskRepository _planningTaskRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private const string BucketName = "files-bucket"; // Replace with your bucket name

    public FileService(IMinioClient fileClient,
                       IFlightBookingTaskRepository flightBookingTaskRepository,
                       IGeneralBookingTaskRepository generalBookingTaskRepository,
                       IHotelBookingTaskRepository hotelBookingTaskRepository,
                       IOtherTaskRepository otherTaskRepository,
                       IPlanningTaskRepository planningTaskRepository,
                       IUserProfileRepository userProfileRepository)
    {
        _fileClient = fileClient;
        _flightBookingTaskRepository = flightBookingTaskRepository;
        _generalBookingTaskRepository = generalBookingTaskRepository;
        _hotelBookingTaskRepository = hotelBookingTaskRepository;
        _otherTaskRepository = otherTaskRepository;
        _planningTaskRepository = planningTaskRepository;
        _userProfileRepository = userProfileRepository;
    }

    public async Task Consume(ConsumeContext<TaskFileUpdateMessage> context)
    {
        TaskFileUpdateMessage message = context.Message;
        if (message == null)
        {
            return;
        }
        DataAccessLayer.ValueObjects.TaskType type = message.Type.MapToEntity();

        string contentType = DetermineContentType(message.File);
        string fileId = await SaveFileToMinio(message.File, message.TaskId.ToString(), contentType);
        if (string.IsNullOrEmpty(fileId))
        {
            return;
        }

        switch (type)
        {
            case DataAccessLayer.ValueObjects.TaskType.GeneralBooking:
                DataAccessLayer.Entities.GeneralBookingTask? generalTask = await _generalBookingTaskRepository.GetByIdAsync(message.TaskId);
                if (generalTask == null)
                {
                    break;
                }
                generalTask.DocumentsUrls.Append(fileId);
                await _generalBookingTaskRepository.UpdateAsync(generalTask);
                break;
            case DataAccessLayer.ValueObjects.TaskType.HotelBooking:
                DataAccessLayer.Entities.HotelBookingTask? hotelTask = await _hotelBookingTaskRepository.GetByIdAsync(message.TaskId);
                if (hotelTask == null)
                {
                    break;
                }
                hotelTask.DocumentsUrls.Append(fileId);
                await _hotelBookingTaskRepository.UpdateAsync(hotelTask);
                break;
            case DataAccessLayer.ValueObjects.TaskType.TicketBooking:
                DataAccessLayer.Entities.FlightBookingTask? ticketTask = await _flightBookingTaskRepository.GetByIdAsync(message.TaskId);
                if (ticketTask == null)
                {
                    break;
                }
                ticketTask.DocumentsUrls.Append(fileId);
                await _flightBookingTaskRepository.UpdateAsync(ticketTask);
                break;
            case DataAccessLayer.ValueObjects.TaskType.Planning:
                DataAccessLayer.Entities.PlanningTask? planningTask = await _planningTaskRepository.GetByIdAsync(message.TaskId);
                if (planningTask == null)
                {
                    break;
                }
                planningTask.DocumentsUrls.Append(fileId);
                await _planningTaskRepository.UpdateAsync(planningTask);
                break;
            case DataAccessLayer.ValueObjects.TaskType.Other:
                DataAccessLayer.Entities.OtherTask? otherTask = await _otherTaskRepository.GetByIdAsync(message.TaskId);
                if (otherTask == null)
                {
                    break;
                }
                otherTask.DocumentsUrls.Append(fileId);
                await _otherTaskRepository.UpdateAsync(otherTask);
                break;
        }
    }

    public async Task Consume(ConsumeContext<UserProfilePictureUpdateMessage> context)
    {
        UserProfilePictureUpdateMessage message = context.Message;
        string contentType = DetermineContentType(message.ProfilePicture);
        string fileId = await SaveFileToMinio(message.ProfilePicture, message.UserId.ToString(), contentType);

        Minio.DataModel.Result.ListAllMyBucketsResult buckets = await _fileClient.ListBucketsAsync();
        if (string.IsNullOrEmpty(fileId))
        {
            return;
        }

        DataAccessLayer.Entities.UserProfile? userProfile = await _userProfileRepository.GetByIdAsync(message.UserId);
        if (userProfile is null)
        {
            return;
        }
        userProfile.ProfilePictureUrl = fileId;
        await _userProfileRepository.UpdateAsync(userProfile);
    }

    private async Task<string> SaveFileToMinio(byte[] fileBytes, string fileName, string contentType)
    {
        try
        {
            BucketExistsArgs beArgs = new BucketExistsArgs().WithBucket(BucketName);
            bool found = await _fileClient.BucketExistsAsync(beArgs);

            try
            {
                MakeBucketArgs mbArgs = new MakeBucketArgs().WithBucket(BucketName);
                await _fileClient.MakeBucketAsync(mbArgs);
            }
            catch (Exception)
            {
            }

            if (!found)
            {
                MakeBucketArgs mbArgs = new MakeBucketArgs().WithBucket(BucketName);
                await _fileClient.MakeBucketAsync(mbArgs);
            }

            string objectName = Guid.NewGuid().ToString();
            using MemoryStream stream = new(fileBytes);

            PutObjectResponse response = await _fileClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(BucketName)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType("application/octet-stream"));

            return objectName;
        }
        catch (MinioException ex)
        {
            Console.WriteLine($"MinIO Error: {ex.Message}");
            return string.Empty;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving file to MinIO: {ex.Message}");
            return string.Empty;
        }
    }

    public async Task<(byte[] File, string ContentType)?> GetFileBytes(string fileId, CancellationToken cancellationToken = default)
    {
        try
        {
            using MemoryStream memoryStream = new();

            // Define arguments to get the object from MinIO
            GetObjectArgs args = new GetObjectArgs()
                .WithBucket(BucketName)
                .WithObject(fileId)
                .WithCallbackStream(stream =>
                {
                    // Copy the stream to the memoryStream to retrieve the file bytes
                    stream.CopyTo(memoryStream);
                });

            // Execute the get object operation
            await _fileClient.GetObjectAsync(args, cancellationToken);

            // Convert the stream to byte array
            byte[] fileBytes = memoryStream.ToArray();

            // Determine the content type of the retrieved file
            string contentType = DetermineContentType(fileBytes);

            // Validate the retrieved bytes and content type
            if (fileBytes.Length == 0 || string.IsNullOrWhiteSpace(contentType))
            {
                Console.WriteLine("Failed to retrieve the correct file bytes or determine content type.");
                return null;
            }

            return (fileBytes, contentType);
        }
        catch (MinioException ex)
        {
            Console.WriteLine($"MinIO Error: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving file from MinIO: {ex.Message}");
            return null;
        }
    }


    private string DetermineContentType(byte[] fileBytes)
    {
        if (fileBytes == null || fileBytes.Length < 4)
        {
            return "application/octet-stream"; // Default for unknown types
        }

        // Common file signatures (magic numbers)
        byte[] pdfMagicNumber = { 0x25, 0x50, 0x44, 0x46 }; // %PDF
        byte[] jpegMagicNumber = { 0xFF, 0xD8, 0xFF }; // JPEG
        byte[] pngMagicNumber = { 0x89, 0x50, 0x4E, 0x47 }; // .PNG
        byte[] gifMagicNumber = { 0x47, 0x49, 0x46, 0x38 }; // GIF

        if (fileBytes.Take(4).SequenceEqual(pdfMagicNumber))
        {
            return "application/pdf";
        }
        else if (fileBytes.Take(3).SequenceEqual(jpegMagicNumber))
        {
            return "image/jpeg";
        }
        else if (fileBytes.Take(4).SequenceEqual(pngMagicNumber))
        {
            return "image/png";
        }
        else if (fileBytes.Take(4).SequenceEqual(gifMagicNumber))
        {
            return "image/gif";
        }
        return "application/octet-stream";
    }

}

