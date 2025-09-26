using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailwayWebApi.Data;
using RailwayWebApi.Models;
using RailwayWebApi.Services;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RailwayWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly TokenService _tokenService;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext context, TokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _tokenService = tokenService;
        }

       

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
                return Ok("User registered successfully");
            else
                return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Attempt to sign in
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: false, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                // You can return some info or a success message
                var token =_tokenService.GenerateToken(model.UserName);
                return Ok(new LoginResponse
                {
                    Success = true,
                    Message="Login Succesfully",
                    Token=token
                });
            }
            else if(result.IsLockedOut)
            {
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = "Account locked.Try again later."

                });
            }
            else
            {
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = "Invalid username or password"

                });
            }
        }


        [HttpGet("GetTrainByNumber/{trainNumber}")]
        public async Task<IActionResult> GetTrainByNumber(int trainNumber)
        {
            var train = await _context.Trains.Where(t => t.TRAINNUMBER == trainNumber).FirstOrDefaultAsync();
            if (train == null)
                return NotFound(new {message="Train Not Found"});
            return Ok(train);
        }

        [HttpGet("GetTrainsByStation/{stationID}")]
        public IActionResult GetTrainsByStation(int stationID)
        {
           
                var trains = (from tr in _context.TrainRoutes
                              join t in _context.Trains on tr.TRAINID equals t.TRAINID
                              where tr.STATIONID == stationID
                              select new StationTrainList
                              {
                                  TRAINNAME = t.TRAINNAME,
                                  TRAINNUMBER = t.TRAINNUMBER,
                                  STATIONID = tr.STATIONID,
                                  SCHEDULEARRIVAL = tr.SCHEDULEARRIVAL,
                                  SCHEDULEDEPARTURE = tr.SCHEDULEDEPARTURE,
                                  PLATFORMNUMBER = tr.PLATFORMNUMBER
                              }).ToList();
                if (trains == null)
                    return NotFound(new { message = "Train Not Found" });
                return Ok(trains);
            
           
        }

        [HttpGet("GetTrainIdCoaches/{trainId}")]
        public IActionResult GetTrainIdCoaches(int trainId)
        { 
           var CoahesTrain = (from coas in _context.COACHES
                             where coas.TRAINID == trainId
                             select new { COACHPOSITION=coas.COACHPOSITION, COACHNUMBER=coas.COACHNUMBER}).ToList();
            if (CoahesTrain == null)
                return NotFound(new { message = "Invalid Train Number" });
            return Ok(CoahesTrain);
        }

        [HttpPost("reset-password-with-phone")]
        public async Task<IActionResult> ResetPasswordWithPhone([FromBody] ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest(new { message = "User not found" });
            if (user.PhoneNumber != model.PhoneNumber && user.Email != model.Email)
                return BadRequest(new { message = "Phone number and email does not match" });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (result.Succeeded)
                return Ok(new { message = "Password reset successfully" });
            return BadRequest(new { errors = result.Errors });
        }
    }
}
