using Contracts;
using Identity.Data;
using Identity.Models;
using Identity.Requets;
using Identity.Responses;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Identity.Services
{
    public interface IIdentityService
    {
        Task ResetPassword(ResetPasswordRequest resetPasswordRequest);
        Task<ConfirmResetPasswordResponse> ConfirmResetPassword(ConfirmResetPasswordRequest confirmResetPasswordRequest);
        Task<ChangePasswordResponse> ChangePassword(ChangePasswordRequest request);
        Task<UserRegisterResponse> Register(UserRegisterRequest request);
        Task<UserLoginResponse> Login(UserLoginRequest request);
        Task<bool> ConfirmEmail(ConfirmEmailRequest request);
        Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest refreshTokenRequest);
        Task Logout(LogoutRequest logoutRequest);
    }

    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IdentityDbContext _context;
        private readonly IConfiguration _config;

        public IdentityService(
            UserManager<User> userManager,
            IJwtTokenService jwtTokenService,
            IPublishEndpoint publishEndpoint,
            IRefreshTokenService refreshTokenService,
            IdentityDbContext context,
            IConfiguration config)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _publishEndpoint = publishEndpoint;
            _refreshTokenService = refreshTokenService;
            _context = context;
            _config = config;
        }

        public async Task<ChangePasswordResponse> ChangePassword(ChangePasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email)
                ?? throw new KeyNotFoundException(nameof(request.Email));

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
                throw new InvalidOperationException("Password change failed");

            await _refreshTokenService.RevokeAllUserRefreshTokens(user.Id);

            var roles = await _userManager.GetRolesAsync(user);
            var permissions = await GetUserPermissionsAsync(user.Id);

            var accessToken = _jwtTokenService.CreateAccessToken(user, roles, permissions);
            var refreshToken = await _refreshTokenService.GenerateRefreshToken(user.Id, 7);

            return new ChangePasswordResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<bool> ConfirmEmail(ConfirmEmailRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId)
                ?? throw new KeyNotFoundException(nameof(request.UserId));

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
            var identityResult = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!identityResult.Succeeded)
                throw new InvalidOperationException("Email confirmation failed");

            return true;
        }

        public async Task<ConfirmResetPasswordResponse> ConfirmResetPassword(ConfirmResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email)
                ?? throw new KeyNotFoundException(nameof(request.Email));

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.ResetCode));
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);

            if (!result.Succeeded)
                throw new InvalidOperationException("Password reset failed");

            await _refreshTokenService.RevokeAllUserRefreshTokens(user.Id);

            var roles = await _userManager.GetRolesAsync(user);
            var permissions = await GetUserPermissionsAsync(user.Id);

            return new ConfirmResetPasswordResponse
            {
                AccessToken = _jwtTokenService.CreateAccessToken(user, roles, permissions),
                RefreshToken = await _refreshTokenService.GenerateRefreshToken(user.Id)
            };
        }

        public async Task<UserLoginResponse> Login(UserLoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email)
                ?? throw new KeyNotFoundException("User not found");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Invalid password");

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Count == 0)
                throw new UnauthorizedAccessException("User has no roles assigned");

            var permissions = await GetUserPermissionsAsync(user.Id);

            var accessToken = _jwtTokenService.CreateAccessToken(user, roles, permissions);
            var refreshToken = await _refreshTokenService.GenerateRefreshToken(user.Id);

            return new UserLoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task Logout(LogoutRequest logoutRequest)
        {
            var refreshToken = await _refreshTokenService.GetRefreshTokenAsync(logoutRequest.Token);

            if (refreshToken is null)
                return;

            var isValid = await _refreshTokenService.ValidateRefreshToken(refreshToken);
            if (!isValid)
                return;

            await _refreshTokenService.RevokeRefreshToken(refreshToken, null);
        }


        public async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request)
        {
            var existing = await _refreshTokenService.GetRefreshTokenAsync(request.Token);
            if (existing is null)
                throw new UnauthorizedAccessException("Refresh token not found");

            var isValid = await _refreshTokenService.ValidateRefreshToken(existing);
            if (!isValid)
                throw new UnauthorizedAccessException("Invalid refresh token");

            var user = await _userManager.FindByIdAsync(existing.UserId)
                ?? throw new KeyNotFoundException("User not found");

            var roles = await _userManager.GetRolesAsync(user);
            var permissions = await GetUserPermissionsAsync(user.Id);

            var accessToken = _jwtTokenService.CreateAccessToken(user, roles, permissions);

            var newRawRefresh = await _refreshTokenService.GenerateRefreshToken(user.Id, request.RememberMe ? 30 : 7);
            var newHash = _refreshTokenService.HashToken(newRawRefresh);

            await _refreshTokenService.RevokeRefreshToken(existing, newHash);

            return new RefreshTokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRawRefresh
            };
        }

        public async Task<UserRegisterResponse> Register(UserRegisterRequest request)
        {
            var existing = await _userManager.FindByEmailAsync(request.Email);
            if (existing != null)
                throw new InvalidOperationException("User with this email already exists");

            var newUser = new User
            {
                UserName = request.Email,
                Email = request.Email,
                Firstname = request.Firstname,
                Lastname = request.Lastname,
                Age = request.Age
            };

            var result = await _userManager.CreateAsync(newUser, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"User creation failed: {errors}");
            }

            var addRoleResult = await _userManager.AddToRoleAsync(newUser, "User");
            if (!addRoleResult.Succeeded)
            {
                var errors = string.Join(", ", addRoleResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to add default role: {errors}");
            }

            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationToken));

            var baseUrl = "http://localhost:8080";
            var confirmUrl =
                $"{baseUrl}/api/Identity/ConfirmEmail" +
                $"?userId={Uri.EscapeDataString(newUser.Id)}" +
                $"&token={Uri.EscapeDataString(encodedToken)}";

            await _publishEndpoint.Publish(new EmailRequested
            {
                ToEmail = newUser.Email!,
                Subject = "Confirm your email",
                Template = EmailTemplateType.ConfirmEmail,
                FirstName = newUser.Firstname,
                ActionUrl = confirmUrl
            });

            var roles = await _userManager.GetRolesAsync(newUser);
            var permissions = await GetUserPermissionsAsync(newUser.Id);

            var accessToken = _jwtTokenService.CreateAccessToken(newUser, roles, permissions);
            var refreshToken = await _refreshTokenService.GenerateRefreshToken(newUser.Id, request.RememberMe ? 30 : 7);

            return new UserRegisterResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task ResetPassword(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email)
                ?? throw new KeyNotFoundException(nameof(request.Email));

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var frontendBase = _config["Frontend:BaseUrl"] ?? "http://localhost:5173";
            var resetUrl =
                $"{frontendBase}/reset-password" +
                $"?email={Uri.EscapeDataString(user.Email ?? string.Empty)}" +
                $"&code={Uri.EscapeDataString(encodedToken)}";

            await _publishEndpoint.Publish(new EmailRequested
            {
                ToEmail = user.Email!,
                Subject = "Reset your password",
                Template = EmailTemplateType.ResetPassword,
                FirstName = user.Firstname,
                ActionUrl = resetUrl
            });
        }

        private async Task<List<string>> GetUserPermissionsAsync(string userId)
        {
            return await (
                from ur in _context.UserRoles
                join rp in _context.RolePermissions on ur.RoleId equals rp.IdentityRoleId
                join p in _context.Persmissions on rp.PersmissionId equals p.Id
                where ur.UserId == userId
                select p.Name
            )
            .Distinct()
            .ToListAsync();
        }
    }
}




