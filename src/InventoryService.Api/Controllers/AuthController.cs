using InventoryService.Application.Dtos;
using InventoryService.Application.Identities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InventoryService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private IMediator _mediator;
    public AuthController(ILogger<AuthController> logger, IMediator mediator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpPost("token")]
    public async Task<IActionResult> GenerateToken([FromBody] LoginDto login)
    {
        var loginResponse = await _mediator.Send(new GenerateTokenCommand(login.Username, login.Password));

        if(!loginResponse.IsAuthenticated)
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }

        return Ok(new LoginResponse
        {
            Token = loginResponse.Token
        });
    }
}
