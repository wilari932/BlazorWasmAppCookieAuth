using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wsamcookieauth.Server.Areas.Identity.Data;
using wsamcookieauth.Shared;

namespace wsamcookieauth.Server;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IdentityDataContext _applicationDbContext;
    //private readonly UserManager<IdentityUser> _userManager;
    public AuthController(IdentityDataContext applicationDbContext  )
    {
        _applicationDbContext = applicationDbContext;
        //_userManager = userManager;
     
    }
    [Authorize]
    [HttpGet]
    [Route("user-profile")]
    public async Task<IActionResult> UserProfileAsync()
    {
        string userId = HttpContext.User.Claims.Where
            (_ => _.Type == ClaimTypes.NameIdentifier).Select(_ => _.Value).First();

        var userProfile = await _applicationDbContext.Users.FirstOrDefaultAsync(x => x.Id.Equals(userId));
        
        return Ok(new UserProfileDto
        {
            UserId = userProfile.Id,
            Email = userProfile.Email,
            Name = userProfile.UserName,
            //Role = (await _userManager.GetRolesAsync(userProfile)).FirstOrDefault()
        });
    }
}