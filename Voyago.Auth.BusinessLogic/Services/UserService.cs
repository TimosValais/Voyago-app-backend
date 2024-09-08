using MassTransit;
using Voyago.App.Contracts.Messages;
using Voyago.Auth.DataAccessLayer.Entities;
using Voyago.Auth.DataAccessLayer.Repositories;

namespace Voyago.Auth.BusinessLogic.Services;
public class UserService : IUserService, IConsumer<UserUpdateMessage>
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Consume(ConsumeContext<UserUpdateMessage> context)
    {
        UserUpdateMessage message = context.Message;
        if (message == null) { return; }
        User? existingUser = await _userRepository.GetByIdAsync(message.UserId);
        if (existingUser == null) { return; };
        existingUser.Email = message.Email;
        existingUser.Username = message.Username;
        await _userRepository.UpdateAsync(existingUser);
    }
}
