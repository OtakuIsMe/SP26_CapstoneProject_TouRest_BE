using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.Common.Constants;
using TouRest.Application.DTOs.Agency;
using TouRest.Application.DTOs.Auth;
using TouRest.Application.DTOs.Provider;
using TouRest.Application.Interfaces;

namespace TouRest.Api.Controllers
{
    [ApiController]
    [Route("api/auths")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;

        public AuthController(ILogger<AuthController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        /// <summary>
        /// Login user with credentials
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            _logger.LogInformation("Login endpoint called for email: {Email}", request.Email);

            var result = await _authService.LoginAsync(request);

            SetRefreshTokenCookie(result.refreshToken);

            return ApiResponseFactory.Ok(result.auth, "Login successful");
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            _logger.LogInformation("Register endpoint called for email: {Email}", request.Email);

            await _authService.RegisterAsync(request);

            return ApiResponseFactory.Created(new { }, "User registered successfully");
        }

        [Authorize]
        [HttpGet("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            _logger.LogInformation("RefreshToken endpoint called");

            var refreshToken = Request.Cookies[AuthConstants.RefreshTokenCookieName];
            if (string.IsNullOrEmpty(refreshToken))
                throw new UnauthorizedAccessException("Refresh token is missing");

            var userId = User.GetUserId();

            var result = await _authService.RefreshTokenAsync(refreshToken, userId);

            SetRefreshTokenCookie(result.refreshToken);

            return ApiResponseFactory.Ok(result.auth, "Token refreshed successfully");
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.GetUserId();
            var result = await _authService.GetMeAsync(userId);
            return ApiResponseFactory.Ok(result, "User retrieved successfully");
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Logout endpoint called");

            var refreshToken = Request.Cookies[AuthConstants.RefreshTokenCookieName];
            if (string.IsNullOrEmpty(refreshToken))
                throw new UnauthorizedAccessException("Refresh token is missing");

            var userId = User.GetUserId();

            await _authService.LogoutAsync(refreshToken, userId);

            Response.Cookies.Delete(AuthConstants.RefreshTokenCookieName);

            return ApiResponseFactory.Ok(new { }, "Logout successful");
        }
        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDTO request)
        {
            var refreshToken = Request.Cookies[AuthConstants.RefreshTokenCookieName];
            if (string.IsNullOrEmpty(refreshToken))
                throw new UnauthorizedAccessException("Refresh token is missing");
            var userId = User.GetUserId();
            await _authService.ChangePasswordAsync(userId, request, refreshToken);
            return ApiResponseFactory.Ok(new { }, "Password changed successfully, Please Login again");
        }
        [HttpPut("change-role")]
        [Authorize]
        public async Task<IActionResult> ChangeRoleAsync(Guid id, [FromBody] ChangeRoleRequestDTO request)
        {
            var refreshToken = Request.Cookies[AuthConstants.RefreshTokenCookieName];
            if (string.IsNullOrEmpty(refreshToken))
                throw new UnauthorizedAccessException("Refresh token is missing");
            var userId = User.GetUserId();
            await _authService.ChangeRoleAsync(userId, request, refreshToken);
            return ApiResponseFactory.Ok(new { }, "Role have been changed, Please Login again");
        }
        private void SetRefreshTokenCookie(string refreshToken)
        {
            Response.Cookies.Append(AuthConstants.RefreshTokenCookieName, refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(AuthConstants.RefreshTokenExpiryDays)
            });
        }
    }
}