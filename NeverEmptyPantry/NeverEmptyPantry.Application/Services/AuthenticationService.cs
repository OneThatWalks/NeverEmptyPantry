using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Account;
using NeverEmptyPantry.Common.Models.Identity;

namespace NeverEmptyPantry.Application.Services
{
    [ExcludeFromCodeCoverage]
    public class AuthenticationService : IAuthenticationService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtDetails _jwtDetails;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthenticationService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, JwtDetails jwtDetails, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtDetails = jwtDetails;
            _roleManager = roleManager;
        }

        public async Task<IOperationResult<TokenModel>> AuthenticateAsync(LoginModel model)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);

            if (!signInResult.Succeeded)
            {
                var error = new OperationError()
                {
                    Name = "Unauthorized",
                    Description = "Login failed"
                };
                return OperationResult<TokenModel>.Failed(error);
            }

            var appUser = await _userManager.FindByNameAsync(model.Username);

            return OperationResult<TokenModel>.Success(new TokenModel(await GetToken(appUser)));
        }

        private async Task<string> GetToken(ApplicationUser user)
        {
            var utcNow = DateTime.UtcNow;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, utcNow.ToString(CultureInfo.InvariantCulture))
            };

            //TODO: Custom claims with namespacing

            var userManagerClaims = await _userManager.GetClaimsAsync(user);

            claims.AddRange(userManagerClaims);

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
                var userManagerRole = await _roleManager.FindByNameAsync(role);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(userManagerRole);
                    foreach (Claim roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtDetails.JwtKey));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                    signingCredentials: signingCredentials,
                    claims: claims,
                    notBefore: utcNow,
                    expires: utcNow.AddMinutes(_jwtDetails.JwtExpireMinutes),
                    audience: _jwtDetails.JwtAudience,
                    issuer: _jwtDetails.JwtIssuer
                );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}