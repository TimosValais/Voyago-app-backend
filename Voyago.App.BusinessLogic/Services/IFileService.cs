namespace Voyago.App.BusinessLogic.Services;
public interface IFileService
{
    Task<(byte[] File, string ContentType)?> GetFileBytes(string fileId, CancellationToken cancellationToken = default);
}
