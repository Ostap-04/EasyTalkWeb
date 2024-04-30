using EasyTalkWeb.Identity.EmailHost;
using EasyTalkWeb.Models.Repositories;
using EasyTalkWeb.Models;
using EasyTalkWeb.Persistance;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyTalkWeb.Controllers;
using EasyTalkWeb.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace EasyTalk.Tests.ControllersTests
{
    public class ProfileControllerTests
    {
        private readonly AppDbContext _dbContextMock;
        private readonly Mock<UserManager<Person>> _userManagerMock;
        private readonly Mock<IMailService> _mailServiceMock;
        private readonly Mock<FreelancerRepository> _freelancerRepositoryMock;
        private readonly Mock<ClientRepository> _clientRepositoryMock;
        private readonly Mock<TechRepository> _techRepositoryMock;
        private readonly Mock<JobPostRepository> _jobPostRepositoryMock;
        private readonly Mock<PersonRepository> _personRepositoryMock;
        private readonly Mock<SignInManager<Person>> _signInManagerMock;

        public ProfileControllerTests()
        {
            _dbContextMock = Mock.Of<AppDbContext>();
            var store = new Mock<IUserStore<Person>>();
            _userManagerMock = new Mock<UserManager<Person>>(store.Object, null, null, null, null, null, null, null, null);
            _mailServiceMock = new Mock<IMailService>();
            _freelancerRepositoryMock = new Mock<FreelancerRepository>(_dbContextMock);
            _clientRepositoryMock = new Mock<ClientRepository>(_dbContextMock);
            _techRepositoryMock = new Mock<TechRepository>(_dbContextMock);
            _jobPostRepositoryMock = new Mock<JobPostRepository>(_dbContextMock);
            _personRepositoryMock = new Mock<PersonRepository>(_dbContextMock);
            _signInManagerMock = new Mock<SignInManager<Person>>(
                _userManagerMock.Object,
                /* IHttpContextAccessor contextAccessor */Mock.Of<IHttpContextAccessor>(),
                /* IUserClaimsPrincipalFactory<TUser> claimsFactory */Mock.Of<IUserClaimsPrincipalFactory<Person>>(),
                /* IOptions<IdentityOptions> optionsAccessor */null!,
                /* ILogger<SignInManager<TUser>> logger */null!,
                /* IAuthenticationSchemeProvider schemes */null!,
                /* IUserConfirmation<TUser> confirmation */null!);
        }
        [Fact]
        public async Task Index_ReturnsViewWithModel()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<Person>>(
                Mock.Of<IUserStore<Person>>(), null, null, null, null, null, null, null, null);

            // Mocking user claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, "john@example.com"),
        new Claim(ClaimTypes.Role, "Freelancer") // Add the necessary roles
    };

            var identity = new ClaimsIdentity(claims, "TestAuthentication");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Mock the User property of ControllerContext
            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };


            // Mocking user retrieval
            var user = new Person
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Location = "Some Location",
                Email = "john@example.com",
            };

            userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            // Mocking freelancer data retrieval
            var freelancer = new Freelancer
            {
                Specialization = "Some Specialization",
                Technologies = new List<Technology>
        {
            new Technology { Name = "Tech 1" },
            new Technology { Name = "Tech 2" }
        }
            };

            _personRepositoryMock.Setup(m => m.GetFreelancer(It.IsAny<Guid>()))
                .ReturnsAsync(freelancer);

            var controller = new ProfileController(userManagerMock.Object, _techRepositoryMock.Object, _personRepositoryMock.Object, _signInManagerMock.Object)
            {
                ControllerContext = controllerContext // Set the mocked ControllerContext
            };

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ProfileViewModel>(viewResult.Model);

            Assert.Equal("John", model.FirstName);
            Assert.Equal("Doe", model.LastName);
            Assert.Equal(new DateOnly(1990, 1, 1), model.DateOfBirth);
            Assert.Equal("Some Location", model.Location);
            Assert.Equal("john@example.com", model.Email);
            Assert.Equal("Some Specialization", model.Specialization);
            Assert.Collection(model.Technologies,
                item => Assert.Equal("Tech 1", item.Name),
                item => Assert.Equal("Tech 2", item.Name)
            );
        }
        [Fact]
        public async Task Index_ReturnsViewWithModel_ForNonFreelancer()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<Person>>(
                Mock.Of<IUserStore<Person>>(), null, null, null, null, null, null, null, null);

            // Mocking user claims without "Freelancer" role
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, "john@example.com"),
        // Role "Freelancer" is not added
    };

            var identity = new ClaimsIdentity(claims, "TestAuthentication");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Mock the User property of ControllerContext
            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

          ;

            // Mocking user retrieval
            var user = new Person
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Location = "Some Location",
                Email = "john@example.com",
            };

            userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var controller = new ProfileController(userManagerMock.Object, _techRepositoryMock.Object, _personRepositoryMock.Object, _signInManagerMock.Object)
            {
                ControllerContext = controllerContext // Set the mocked ControllerContext
            };

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ProfileViewModel>(viewResult.Model);

            // Assert that the model properties are set correctly
            Assert.Equal("John", model.FirstName);
            Assert.Equal("Doe", model.LastName);
            Assert.Equal(new DateOnly(1990, 1, 1), model.DateOfBirth);
            Assert.Equal("Some Location", model.Location);
            Assert.Equal("john@example.com", model.Email);
            Assert.Null(model.Specialization); // Specialization should not be set
            Assert.Null(model.Technologies); // Technologies should not be set
        }
        [Fact]
        public async Task DeleteUser_ReturnsDeletedRedirectResult_WhenUserDeletedSuccessfully()
        {
            var user = new Person
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Location = "Some Location",
                Email = "john@example.com",
            };

            var userManagerMock = new Mock<UserManager<Person>>(
                Mock.Of<IUserStore<Person>>(), null, null, null, null, null, null, null, null);
            userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            // Arrange
            var signInManagerMock = new Mock<SignInManager<Person>>(
                userManagerMock.Object, Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<Person>>(), null, null, null, null);
            signInManagerMock.Setup(m => m.SignOutAsync())
                .Returns(Task.CompletedTask);

            userManagerMock.Setup(m => m.DeleteAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            var controller = new ProfileController(userManagerMock.Object, null, null, signInManagerMock.Object);

            // Act
            var result = await controller.DeleteUser();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Deleted", redirectResult.ActionName);
            Assert.Equal("Profile", redirectResult.ControllerName);
        }
        [Fact]
        public async Task Edit_ReturnsRedirectToAction_WhenEditingClientProfile()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<Person>>(
                Mock.Of<IUserStore<Person>>(), // Mock IUserStore<Person> as a dependency
                null, // Inject IUserPasswordStore<Person> if needed, replace null with its mock
                null, // Inject IOptions<IdentityOptions> if needed, replace null with its mock
                null, // Inject IEnumerable<IUserValidator<Person>> if needed, replace null with its mock
                null, // Inject IEnumerable<IPasswordValidator<Person>> if needed, replace null with its mock
                null, // Inject ILookupNormalizer if needed, replace null with its mock
                null, // Inject IdentityErrorDescriber if needed, replace null with its mock
                null, // Inject IServiceProvider if needed, replace null with its mock
                null // Inject ILogger<UserManager<Person>> if needed, replace null with its mock
            );
            userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                           .ReturnsAsync(new Person()); // Simulate a valid user

            var profileViewModel = new ProfileViewModel
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Location = "Some Location",
                DateOfBirth = new DateOnly(1990, 1, 1),
                // Add other properties as needed
            };

            var personRepositoryMock = new Mock<PersonRepository>(_dbContextMock); // Provide the necessary constructor arguments
            var techRepositoryMock = new Mock<TechRepository>(_dbContextMock); // Provide the necessary constructor arguments
            var signInManagerMock = new Mock<SignInManager<Person>>(
                userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<Person>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<ILogger<SignInManager<Person>>>(),
                Mock.Of<IAuthenticationSchemeProvider>(),
                Mock.Of<IUserConfirmation<Person>>()
            );

            var controller = new ProfileController(userManagerMock.Object, techRepositoryMock.Object, personRepositoryMock.Object, signInManagerMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "Client") })) }
            };

            // Act
            var result = await controller.Edit(profileViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Profile", redirectToActionResult.ControllerName);
        }


        //[Fact]
        //public async Task Edit_ReturnsRedirectToAction_WhenEditingFreelancerProfile()
        //{
        //    // Arrange
        //    var userManagerMock = new Mock<UserManager<Person>>(
        //        Mock.Of<IUserStore<Person>>(), // Mock IUserStore<Person> as a dependency
        //        null, // Inject IUserPasswordStore<Person> if needed, replace null with its mock
        //        null, // Inject IOptions<IdentityOptions> if needed, replace null with its mock
        //        null, // Inject IEnumerable<IUserValidator<Person>> if needed, replace null with its mock
        //        null, // Inject IEnumerable<IPasswordValidator<Person>> if needed, replace null with its mock
        //        null, // Inject ILookupNormalizer if needed, replace null with its mock
        //        null, // Inject IdentityErrorDescriber if needed, replace null with its mock
        //        null, // Inject IServiceProvider if needed, replace null with its mock
        //        null // Inject ILogger<UserManager<Person>> if needed, replace null with its mock
        //    );
        //    userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
        //                   .ReturnsAsync(new Person()); // Simulate a valid user

        //    var profileViewModel = new ProfileViewModel
        //    {
        //        FirstName = "John",
        //        LastName = "Doe",
        //        Email = "john@example.com",
        //        Location = "Some Location",
        //        DateOfBirth = new DateOnly(1990, 1, 1),
        //        // Add other properties as needed
        //    };

        //    var personRepositoryMock = new Mock<PersonRepository>(_dbContextMock); // Provide the necessary constructor arguments
        //    var techRepositoryMock = new Mock<TechRepository>(_dbContextMock); // Provide the necessary constructor arguments
        //    var signInManagerMock = new Mock<SignInManager<Person>>(
        //        userManagerMock.Object,
        //        Mock.Of<IHttpContextAccessor>(),
        //        Mock.Of<IUserClaimsPrincipalFactory<Person>>(),
        //        Mock.Of<IOptions<IdentityOptions>>(),
        //        Mock.Of<ILogger<SignInManager<Person>>>(),
        //        Mock.Of<IAuthenticationSchemeProvider>(),
        //        Mock.Of<IUserConfirmation<Person>>()
        //    );

        //    var controller = new ProfileController(userManagerMock.Object, techRepositoryMock.Object, personRepositoryMock.Object, signInManagerMock.Object);
        //    controller.ControllerContext = new ControllerContext
        //    {
        //        HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "Freelancer") })) }
        //    };

        //    // Add Content-Type header to the request
        //    controller.Request.ContentType = "application/x-www-form-urlencoded";

        //    // Act
        //    var result = await controller.Edit(profileViewModel);

        //    // Assert
        //    var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        //    Assert.Equal("Index", redirectToActionResult.ActionName);
        //    Assert.Equal("Profile", redirectToActionResult.ControllerName);
        //}
        [Fact]
        public async Task Deleted_ReturnsViewResult()
        {
           

            var controller = new ProfileController(_userManagerMock.Object, _techRepositoryMock.Object, _personRepositoryMock.Object, _signInManagerMock.Object);

            // Act
            var result = await controller.Deleted();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName); // Ensure the view name is not specified (defaults to "Deleted")
        }

    }
}
