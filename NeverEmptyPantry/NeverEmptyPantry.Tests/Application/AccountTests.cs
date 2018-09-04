using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeverEmptyPantry.Application.Services;
using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Models.Account;
using NeverEmptyPantry.Common.Models.Identity;
using NeverEmptyPantry.Tests.Util;
using Microsoft.EntityFrameworkCore;
using NeverEmptyPantry.Common.Models;

namespace NeverEmptyPantry.Tests.Application
{
    [TestClass]
    public class AccountTests
    {
        private MockRepository _mockRepository;
        private Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private Mock<IAccountRepository> _mockAccountRepository;
        private Mock<IConfiguration> _mockConfiguration;

        [TestInitialize]
        public void TestInitialize()
        {
            // Our mock creator for all non helped mocks
            _mockRepository = new MockRepository(MockBehavior.Default);

            // Setup the user manager to use for sign in manager, courtesy of Microsoft source
            _mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            _mockRoleManager = MockHelpers.MockRoleManager<IdentityRole>();
            var hasher = _mockRepository.Create<IPasswordHasher<ApplicationUser>>();
            _mockUserManager.Object.PasswordHasher = hasher.Object;

            // Setup constructor parameters of sign in manager
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(a => a.HttpContext).Returns(context.Object);
            var roleManager = MockHelpers.MockRoleManager<IdentityRole>();
            var identityOptions = new IdentityOptions();
            var options = new Mock<IOptions<IdentityOptions>>();
            options.Setup(a => a.Value).Returns(identityOptions);
            var claimsFactory = new UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>(_mockUserManager.Object, roleManager.Object, options.Object);
            var logStore = new StringBuilder();
            var logger = MockHelpers.MockILogger<SignInManager<ApplicationUser>>(logStore);

            _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(_mockUserManager.Object, contextAccessor.Object, claimsFactory, options.Object, logger.Object, new Mock<IAuthenticationSchemeProvider>().Object);

            _mockAccountRepository = _mockRepository.Create<IAccountRepository>();
            _mockConfiguration = _mockRepository.Create<IConfiguration>();
        }

        [TestMethod]
        public async Task RegisterFailsWithNegativeIdentityResult()
        {
            // Arrange
            var model = new RegisterDto();
            var error = new IdentityError() { Code = "13", Description = "Test Identity Error" };
            var identityResult = IdentityResult.Failed(new IdentityError[] { error });
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(identityResult);

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.RegisterAsync(model);

            // Assert
            Assert.IsFalse(result.Succeeded);
        }

        [TestMethod]
        public async Task RegisterSucceedsWithTokenAndSignsIn()
        {
            // Arrange
            var model = new RegisterDto()
            {
                Email = "FORJWT"
            };
            var identityResult = IdentityResult.Success;
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(identityResult);
            var roles = new List<string>() { "TestRole" };
            _mockUserManager.Setup(um => um.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(roles);
            _mockConfiguration.Setup(c => c["JwtKey"]).Returns("YmYxNDg2NDktMmUxNS00YTgwLWE5ODQtZjQ4ZjVjNzdiYmNj");
            _mockConfiguration.Setup(c => c["JwtIssuer"]).Returns("http://yourdomain.com");
            _mockConfiguration.Setup(c => c["JwtExpireDays"]).Returns("30");

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.RegisterAsync(model);

            // Assert
            Assert.IsTrue(result.Succeeded);
            Assert.IsNotNull(result.Token);
        }

        [TestMethod]
        public async Task LoginFailedWithNegativeIdentityResult()
        {
            // Arrange
            var model = new LoginDto
            {
                Email = "TESTEMAIL",
                Password = "TESTPASS",
                RememberMe = false
            };
            var signInResult = SignInResult.Failed;
            _mockSignInManager.Setup(sim => sim.PasswordSignInAsync(model.Email, model.Password, false, false))
                .ReturnsAsync(signInResult);

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.LoginAsync(model);

            // Assert
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("500", result.Errors.First().Code);
        }

        [TestMethod]
        public async Task LoginSucceedsWithToken()
        {
            // Arrange
            var model = new LoginDto
            {
                Email = "TESTEMAIL",
                Password = "TESTPASS",
                RememberMe = false
            };
            var signInResult = SignInResult.Success;
            _mockSignInManager.Setup(sim => sim.PasswordSignInAsync(model.Email, model.Password, false, false))
                .ReturnsAsync(signInResult);
            var user = new ApplicationUser
            {
                Email = model.Email
            };
            var userList = new List<ApplicationUser>() { user };
            IQueryable<ApplicationUser> queryable = userList.AsQueryable();
            _mockUserManager.Setup(um => um.Users)
                .Returns(queryable);
            var roles = new List<string> { "TestRole" };
            _mockUserManager.Setup(um => um.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(roles);
            _mockConfiguration.Setup(c => c["JwtKey"]).Returns("YmYxNDg2NDktMmUxNS00YTgwLWE5ODQtZjQ4ZjVjNzdiYmNj");
            _mockConfiguration.Setup(c => c["JwtIssuer"]).Returns("http://yourdomain.com");
            _mockConfiguration.Setup(c => c["JwtExpireDays"]).Returns("30");

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.LoginAsync(model);

            // Assert
            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public async Task RegistrationFailsOnCreateUser()
        {
            // Arrange
            var model = new RegisterDto
            {
                Email = "EMAILTEST",
                Password = "PASSWORDTEST"
            };
            var error = new IdentityError
            {
                Code = "100",
                Description = "Description"
            };
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), model.Password)).ReturnsAsync(IdentityResult.Failed(error));

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.RegisterAsync(model);

            // Assert
            Assert.IsFalse(result.Succeeded);
            Assert.IsNotNull(result.Errors);
        }

        [TestMethod]
        public async Task RegistrationSucceedsAndSignsIn()
        {
            // Arrange
            var model = new RegisterDto
            {
                Email = "EMAILTEST",
                Password = "PASSWORDTEST"
            };
            var error = new IdentityError
            {
                Code = "100",
                Description = "Description"
            };
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), model.Password)).ReturnsAsync(IdentityResult.Success);
            _mockConfiguration.Setup(c => c["JwtKey"]).Returns("YmYxNDg2NDktMmUxNS00YTgwLWE5ODQtZjQ4ZjVjNzdiYmNj");
            _mockConfiguration.Setup(c => c["JwtIssuer"]).Returns("http://yourdomain.com");
            _mockConfiguration.Setup(c => c["JwtExpireDays"]).Returns("30");
            var roles = new List<string> { "TestRole" };
            _mockUserManager.Setup(um => um.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(roles);

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.RegisterAsync(model);

            // Assert
            Assert.IsTrue(result.Succeeded);
            Assert.IsNotNull(result.Token);
        }

        [TestMethod]
        public async Task GetProfileFailsWithNoQueryEmail()
        {
            // Arrange

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.GetProfileAsync(null);

            // Assert
            Assert.IsFalse(result.Succeeded);
            Assert.IsNotNull(result.Errors);
        }

        [TestMethod]
        public async Task GetProfileFailsWithNoAccountFound()
        {
            // Arrange
            ApplicationUser user = null;
            _mockAccountRepository.Setup(ar => ar.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.GetProfileAsync("EMAIL");

            // Assert
            Assert.IsFalse(result.Succeeded);
            Assert.IsNotNull(result.Errors);
        }

        [TestMethod]
        public async Task GetProfileSucceeds()
        {
            // Arrange
            ApplicationUser user = new ApplicationUser
            {
                Email = "EMAIL"
            };
            _mockAccountRepository.Setup(ar => ar.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.GetProfileAsync("EMAIL");

            // Assert
            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public async Task SetProfileFailsWithNoUserFound()
        {
            // Arrange
            ApplicationUser user = null;
            _mockAccountRepository.Setup(ar => ar.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
            var profile = new ProfileDto();

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.SetProfileAsync(profile);

            // Assert
            Assert.IsFalse(result.Succeeded);
            Assert.IsNotNull(result.Errors);
        }

        [TestMethod]
        public async Task SetProfileFailsWithNoUpdate()
        {
            // Arrange
            ApplicationUser user = new ApplicationUser
            {
                Email = "TEST1"
            };
            _mockAccountRepository.Setup(ar => ar.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
            var profile = new ProfileDto
            {
                Email = "TEST1"
            };
            var error = new IdentityError
            {
                Code = "100",
                Description = "Description"
            };
            var idResult = IdentityResult.Failed(error);
            _mockUserManager.Setup(um => um.UpdateAsync(user)).ReturnsAsync(idResult);

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.SetProfileAsync(profile);

            // Assert
            Assert.IsFalse(result.Succeeded);
            Assert.IsNotNull(result.Errors);
            Assert.AreEqual(2, result.Errors.Count());
        }

        [TestMethod]
        public async Task SetProfileSucceedsWithProfileResult()
        {
            // Arrange
            ApplicationUser user = new ApplicationUser
            {
                Email = "TEST1",
                FirstName = "ABRAHAM"
            };
            _mockAccountRepository.Setup(ar => ar.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
            var profile = new ProfileDto
            {
                Email = "TEST2",
                FirstName = "MOHAMMED"
            };
            var idResult = IdentityResult.Success;
            _mockUserManager.Setup(um => um.UpdateAsync(user)).ReturnsAsync(idResult);

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.SetProfileAsync(profile);

            // Assert
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(profile.FirstName, result.Profile.FirstName);
        }

        [TestMethod]
        public async Task LogoutSucceeds()
        {
            // Arrange

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.LogoutAsync();

            // Assert
            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public async Task GetsAllRoles()
        {
            // Arrange
            var roleList = new List<IdentityRole> { new IdentityRole() { Id = "ID1", Name = "Role 1" }, new IdentityRole() { Id = "ID2", Name = "Role 2" } };
            IQueryable<IdentityRole> queryable = roleList.AsQueryable();
            _mockRoleManager.Setup(rm => rm.Roles)
                .Returns(queryable);

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.GetRoles();

            // Assert
            Assert.IsNotNull(result);
            var resultAsList = result.ToList();
            Assert.IsTrue(resultAsList.Count() == 2);
            Assert.AreEqual("ID1", resultAsList.FirstOrDefault()?.Id);
            Assert.AreEqual("ID2", resultAsList.LastOrDefault()?.Id);
        }

        [TestMethod]
        public async Task GetsAllOfUsersRoles()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Email = "TEST@EMAIL.COM"
            };
            _mockAccountRepository.Setup(um => um.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);

            var roles = new List<string> { "Role 1" };
            _mockUserManager.Setup(um => um.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(roles);


            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.GetUserRoles("TEST@EMAIL.COM");

            // Assert
            Assert.IsNotNull(result);
            var resultAsList = result.ToList();
            Assert.IsTrue(resultAsList.Count() == 1);
            Assert.AreEqual("Role 1", resultAsList.FirstOrDefault());
        }

        [TestMethod]
        public async Task GetsApplicationUserFromClaims()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Email = "TEST@EMAIL.COM"
            };
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.GetUserFromClaimsPrincipal(new ClaimsPrincipal());

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user, result);
        }

        [TestMethod]
        public async Task CannotCreateRoleWithIdentityError()
        {
            // Arrange
            var err = new IdentityError
            {
                Description = "This is bad",
                Code = "100"
            };
            var identityResult = IdentityResult.Failed(err);
            _mockRoleManager.Setup(rm => rm.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(identityResult);

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.CreateRole("RoliRole");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Errors);
            Assert.AreEqual("This is bad", result.Errors.FirstOrDefault()?.Description);
        }

        [TestMethod]
        public async Task CreatesARoleSuccessfully()
        {
            // Arrange
            var identityResult = IdentityResult.Success;
            _mockRoleManager.Setup(rm => rm.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(identityResult);

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.CreateRole("RoliRole");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public async Task CannotRemoveRoleWithError()
        {
            // Arrange
            var role = new IdentityRole
            {
                Name = "RoliRole"
            };
            _mockRoleManager.Setup(rm => rm.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(role);
            var err = new IdentityError
            {
                Description = "This is bad",
                Code = "100"
            };
            var identityResult = IdentityResult.Failed(err);
            _mockRoleManager.Setup(rm => rm.DeleteAsync(role)).ReturnsAsync(identityResult);

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.RemoveRole("RoliRole");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Errors);
            Assert.AreEqual("This is bad", result.Errors.FirstOrDefault()?.Description);
        }

        [TestMethod]
        public async Task CannotRemoveRoleWithNoRoleFound()
        {
            // Arrange
            IdentityRole role = null;
            _mockRoleManager.Setup(rm => rm.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(role);

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.RemoveRole("RoliRole");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Errors);
        }

        [TestMethod]
        public async Task RemovesARoleSuccessfully()
        {
            // Arrange
            var role = new IdentityRole
            {
                Name = "RoliRole"
            };
            _mockRoleManager.Setup(rm => rm.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(role);
            var identityResult = IdentityResult.Success;
            _mockRoleManager.Setup(rm => rm.DeleteAsync(It.IsAny<IdentityRole>())).ReturnsAsync(identityResult);

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.RemoveRole("RoliRole");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Succeeded);
            _mockRoleManager.Verify(rm => rm.FindByNameAsync("RoliRole"), Times.AtMostOnce);
        }

        [TestMethod]
        public async Task UpdateFailsWithNoRoleFound()
        {
            // Arrange
            IdentityRole role = null;
            _mockRoleManager.Setup(rm => rm.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(role);

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.UpdateRole("RoliRole", "SweetRole");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Errors);
        }

        [TestMethod]
        public async Task UpdateFailsWithIdentityError()
        {
            // Arrange
            IdentityRole role = new IdentityRole
            {
                Name = "RoliRole"
            };
            _mockRoleManager.Setup(rm => rm.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(role);
            var err = new IdentityError()
            {
                Description = "This is bad",
                Code = "100"
            };
            var identityResult = IdentityResult.Failed(err);
            _mockRoleManager.Setup(rm => rm.UpdateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(identityResult);

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.UpdateRole("RoliRole", "SweetRole");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Errors);
            Assert.AreEqual("This is bad", result.Errors.FirstOrDefault()?.Description);
        }

        [TestMethod]
        public async Task UpdateSucceeds()
        {
            // Arrange
            IdentityRole role = new IdentityRole
            {
                Name = "RoliRole"
            };
            _mockRoleManager.Setup(rm => rm.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(role);
            var identityResult = IdentityResult.Success;
            _mockRoleManager.Setup(rm => rm.UpdateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(identityResult);

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.UpdateRole("RoliRole", "SweetRole");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public async Task AddUserToRoleFailsWithNoUser()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Email = "TEST@EMAIL.COM"
            };
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.UpdateRole("RoliRole", "SweetRole");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Errors);
        }

        [TestMethod]
        public async Task AddUserToRoleFailsWithNullUser()
        {
            // Arrange
            ApplicationUser user = null;
            _mockAccountRepository.Setup(um => um.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
            var claims = new ClaimsPrincipal();

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.AddUserToRole("TEST@EMAIL.COM", "SweetRole");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Errors);
        }

        [TestMethod]
        public async Task AddUserToRoleFailsWithIdentityError()
        {
            // Arrange
            ApplicationUser user = new ApplicationUser
            {
                Email = "TEST@EMAIL.COM"
            };
            _mockAccountRepository.Setup(um => um.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
            var err = new IdentityError
            {
                Description = "This is bad",
                Code = "100"
            };
            var identityResult = IdentityResult.Failed(err);
            _mockUserManager.Setup(um => um.AddToRoleAsync(user, "SweetRole")).ReturnsAsync(identityResult);
            var claims = new ClaimsPrincipal();

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.AddUserToRole("TEST@EMAIL.COM", "SweetRole");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Errors);
            Assert.AreEqual(2, result.Errors.Count());
        }

        [TestMethod]
        public async Task AddUserToRoleSucceeds()
        {
            // Arrange
            ApplicationUser user = new ApplicationUser
            {
                Email = "TEST@EMAIL.COM"
            };
            _mockAccountRepository.Setup(um => um.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
            var identityResult = IdentityResult.Success;
            _mockUserManager.Setup(um => um.AddToRoleAsync(user, "SweetRole")).ReturnsAsync(identityResult);
            var claims = new ClaimsPrincipal();

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.AddUserToRole("TEST@EMAIL.COM", "SweetRole");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Succeeded);
            Assert.IsNotNull(result.Profile);
            _mockAccountRepository.Verify(ar => ar.GetUserAsync("TEST@EMAIL.COM"), Times.Exactly(2));
        }

        [TestMethod]
        public async Task RemoveUserFromRoleFailsWithNoRoleFound()
        {
            // Arrange
            ApplicationUser user = null;
            _mockAccountRepository.Setup(um => um.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
            var claims = new ClaimsPrincipal();

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.RemoveUserFromRole("TEST@EMAIL.COM", "SweetRole");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Errors);
        }

        [TestMethod]
        public async Task RemoveUserFromRoleFailsWithError()
        {
            // Arrange
            ApplicationUser user = new ApplicationUser
            {
                Email = "TEST@EMAIL.COM"
            };
            _mockAccountRepository.Setup(um => um.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
            var err = new IdentityError
            {
                Description = "This is bad",
                Code = "100"
            };
            var identityResult = IdentityResult.Failed(err);
            _mockUserManager.Setup(um => um.RemoveFromRoleAsync(user, "SweetRole")).ReturnsAsync(identityResult);

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.RemoveUserFromRole("TEST@EMAIL.COM", "SweetRole");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Errors);
            Assert.AreEqual(2, result.Errors.Count());
        }

        [TestMethod]
        public async Task RemoveUserFromRoleSucceeds()
        {
            // Arrange
            ApplicationUser user = new ApplicationUser
            {
                Email = "TEST@EMAIL.COM"
            };
            _mockAccountRepository.Setup(um => um.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
            var identityResult = IdentityResult.Success;
            _mockUserManager.Setup(um => um.RemoveFromRoleAsync(user, "SweetRole")).ReturnsAsync(identityResult);

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.RemoveUserFromRole("TEST@EMAIL.COM", "SweetRole");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Succeeded);
            Assert.IsNotNull(result.Profile);
            _mockAccountRepository.Verify(ar => ar.GetUserAsync("TEST@EMAIL.COM"), Times.Exactly(2));
        }

        [TestMethod]
        public async Task GetsOfficeLocations()
        {
            // Arrange
            var locations = new List<OfficeLocation>
            {
                new OfficeLocation
                {
                    Name = "Office 1"
                }
            };
            _mockAccountRepository.Setup(ar => ar.GetOfficeLocations()).ReturnsAsync(locations);

            // Act
            var service = new AccountService(_mockSignInManager.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockAccountRepository.Object);
            var result = await service.GetOfficeLocations();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
        }
    }
}
