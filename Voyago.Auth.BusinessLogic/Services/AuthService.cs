using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Voyago.Auth.BusinessLogic.Config;
using Voyago.Auth.BusinessLogic.Exceptions;
using Voyago.Auth.BusinessLogic.Helpers;
using Voyago.Auth.DataAccessLayer.Entities;
using Voyago.Auth.DataAccessLayer.Repositories;

namespace Voyago.Auth.BusinessLogic.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly string _jwtAudience;
    private readonly string _jwtIssuer;
    private readonly string _jwtHashingKey;

    public AuthService(IUserRepository userRepository, IJWTConfig jwtConfig)
    {
        _userRepository = userRepository;
        _jwtAudience = jwtConfig.Audience;
        _jwtIssuer = jwtConfig.Issuer;
        _jwtHashingKey = jwtConfig.HashKey;
    }

    public async Task<Guid> RegisterAsync(User user, CancellationToken cancellationToken = default)
    {
        // Check if username or email already exists
        User? existingUser = await _userRepository.GetByUsernameAsync(user.Username, cancellationToken);
        if (existingUser != null) throw new UserAlreadyExistsException("Username already exists");

        existingUser = await _userRepository.GetByEmailAsync(user.Email, cancellationToken);
        if (existingUser != null) throw new UserAlreadyExistsException("Email already exists");

        // Hash the password before saving
        user.PasswordHash = PasswordHelper.HashPassword(user.PasswordHash);

        // Insert the new user
        bool success = await _userRepository.InsertAsync(user, cancellationToken);
        if (!success) throw new Exception("Something went wrong");
        return user.Id;
    }

    public async Task<string?> LoginAsync(string usernameOrEmail, string password, CancellationToken cancellationToken = default)
    {
        // Retrieve the user by username or email
        User? existingUser = await _userRepository.GetByUsernameAsync(usernameOrEmail, cancellationToken);
        if (existingUser == null) existingUser = await _userRepository.GetByEmailAsync(usernameOrEmail, cancellationToken);
        if (existingUser == null || !PasswordHelper.CheckPassword(password, existingUser.PasswordHash)) return null;

        // Generate JWT token
        JwtSecurityTokenHandler tokenHandler = new();
        byte[] key = Encoding.ASCII.GetBytes(_jwtHashingKey);

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, existingUser.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, _jwtIssuer),
                new Claim(JwtRegisteredClaimNames.Aud, _jwtAudience),
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
