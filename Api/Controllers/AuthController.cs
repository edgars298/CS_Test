using Api.Core.Configs;
using Api.ViewModels.User;
using BusinessLogic.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(
    UserManager<IdentityUser> _userManager,
    SignInManager<IdentityUser> _signInManager,
    IOptions<JwtConfig> _jwtConfig
) : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager = _userManager;
    private readonly SignInManager<IdentityUser> _signInManager = _signInManager;
    private readonly JwtConfig _jwtConfig = _jwtConfig.Value;

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginFormDto form)
    {
        const string AuthFailMessage = "Invalid username or password.";

        var user = await _userManager.FindByEmailAsync(form.Username);

        if (user is null || user.UserName is null)
        {
            return Unauthorized(AuthFailMessage);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, form.Password, false);

        if (!result.Succeeded)
        {
            return Unauthorized(AuthFailMessage);
        }

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(tokenString);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterFormDto form)
    {
        var user = new IdentityUser
        {
            UserName = form.UserName,
            Email = form.UserName
        };

        var result = await _userManager.CreateAsync(user, form.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        result = await _userManager.AddToRoleAsync(user, RoleNames.User);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }


        return NoContent();
    }
}
