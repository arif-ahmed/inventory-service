using InventoryService.Application.Dtos;
using InventoryService.Domain.Interfaces;
using MediatR;


namespace InventoryService.Application.Identities;

public class GenerateTokenCommand : IRequest<LoginResponse>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public GenerateTokenCommand(string username, string password)
    {
        Username = username;
        Password = password;
    }
}

public class GenerateTokenCommandHandler : IRequestHandler<GenerateTokenCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;
    public GenerateTokenCommandHandler(IUserRepository userRepository, ITokenService tokenService, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }
    public async Task<LoginResponse> Handle(GenerateTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        if(!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash)) 
            return new LoginResponse { IsAuthenticated = false };

        var token = await _tokenService.GenerateTokenAsync();
        return new LoginResponse { IsAuthenticated = true, Token = token };
    }
}
