using EasyTalkWeb.Models.Repositories;
using EasyTalkWeb.Models;
using EasyTalkWeb.Persistance;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using EasyTalkWeb.Controllers;
using EasyTalkWeb.Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using DateOnly = System.DateOnly;

namespace EasyTalk.Tests.ControllersTests
{
    public class ProfileControllerTests
    {
        private readonly AppDbContext _dbContextMock;
        private readonly Mock<TechRepository> _techRepositoryMock;
        private readonly Mock<UserManager<Person>> _userManagerMock;
        private readonly Mock<PersonRepository> _personRepositoryMock;
        private readonly Mock<SignInManager<Person>> _signInManagerMock;

        public ProfileControllerTests()
        {
            _dbContextMock = Mock.Of<AppDbContext>();
            _techRepositoryMock = new Mock<TechRepository>(_dbContextMock);
            _userManagerMock = new Mock<UserManager<Person>>(Mock.Of<IUserStore<Person>>(), null!, null!, null!, null!, null!, null!, null!, null!);
            _signInManagerMock = new Mock<SignInManager<Person>>(
                _userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<Person>>(), null!, null!, null!, null!);
            _personRepositoryMock = new Mock<PersonRepository>(_dbContextMock);
        }

        [Fact]
        public async Task Index_ReturnsProfileViewWithModel()
        {
            var userId = Guid.NewGuid();
            var user = new Person
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Location = "New York",
                Email = "john@example.com"
            };
            var specialization = "Software Development";

            var technologies = new List<Technology>
            {
                new Technology { Id = Guid.NewGuid(), Name = "C#" },
                new Technology { Id = Guid.NewGuid(), Name = "ASP.NET Core" }
            };

            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManagerMock.Setup(m => m.IsInRoleAsync(user, "Freelancer")).ReturnsAsync(true);
            _personRepositoryMock.Setup(m => m.GetFreelancer(It.IsAny<Guid>())).ReturnsAsync(new Freelancer { Specialization = specialization, Technologies = technologies });

            var controller = new ProfileController(_userManagerMock.Object, _techRepositoryMock.Object, _personRepositoryMock.Object, _signInManagerMock.Object);
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()), new Claim(ClaimTypes.Role, "Freelancer") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var result = await controller.Index() as ViewResult;
            
            Assert.NotNull(result);
            Assert.Null(result.ViewName);
            var model = result.Model as ProfileViewModel;
            Assert.NotNull(model);
            Assert.Equal(user.FirstName, model.FirstName);
            Assert.Equal(user.LastName, model.LastName);
            Assert.Equal(user.DateOfBirth, model.DateOfBirth);
            Assert.Equal(user.Location, model.Location);
            Assert.Equal(user.Email, model.Email);
            Assert.Equal(specialization, model.Specialization);

            Assert.NotNull(model.Technologies);
            Assert.Equal(technologies.Count, model.Technologies.Count);
            var modelLechnologies = model.Technologies.ToList();
            for (int i = 0; i < technologies.Count; i++)
            {
                Assert.Equal(technologies[i].Name, modelLechnologies[i].Name);
            }
        }

        [Fact]
        public async Task Edit_ReturnsCorrectViewModel()
        {
            var userId = Guid.NewGuid();
            var user = new Person
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Location = "New York",
                Email = "john@example.com"
            };

            var technologies = new List<Technology>
            {
                new Technology { Id = Guid.NewGuid(), Name = "C#" },
                new Technology { Id = Guid.NewGuid(), Name = "ASP.NET Core" }
            };

            var freelancer = new Freelancer
            {
                PersonId = userId,
                Specialization = "Software Development",
                Technologies = technologies
            };

            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManagerMock.Setup(m => m.IsInRoleAsync(user, "Freelancer")).ReturnsAsync(true);
            _personRepositoryMock.Setup(m => m.GetPersonWithTechnologiesById(userId)).ReturnsAsync(user);
            _techRepositoryMock.Setup(m => m.GetAllAsync()).ReturnsAsync(technologies);
            _personRepositoryMock.Setup(m => m.GetFreelancer(userId)).ReturnsAsync(freelancer);

            var controller = new ProfileController(_userManagerMock.Object, _techRepositoryMock.Object, _personRepositoryMock.Object, _signInManagerMock.Object);
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()), new Claim(ClaimTypes.Role, "Freelancer") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var result = await controller.Edit() as ViewResult;

            Assert.NotNull(result);
            Assert.Null(result.ViewName);
            var model = result.Model as ProfileViewModel;
            Assert.NotNull(model);
            Assert.Equal(user.FirstName, model.FirstName);
            Assert.Equal(user.LastName, model.LastName);
            Assert.Equal(user.DateOfBirth, model.DateOfBirth);
            Assert.Equal(user.Location, model.Location);
            Assert.Equal(user.Email, model.Email);
            Assert.NotNull(model.Technologies);
            Assert.Equal(technologies.Count, model.Technologies.Count);

            Assert.Equal(technologies.Count, model.SelectedTechnologiesData.Count);
            foreach (var technology in technologies)
            {
                Assert.Contains(technology.Name, model.SelectedTechnologiesData);
            }
            Assert.Equal(freelancer.Specialization, model.Specialization);
        }

        [Fact]
        public async Task DeleteUser_WhenUserNotFound_ReturnsNotFound()
        {
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((Person)null);

            var controller = new ProfileController(_userManagerMock.Object, _techRepositoryMock.Object, _personRepositoryMock.Object, _signInManagerMock.Object);

            var result = await controller.DeleteUser();

            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_WhenDeletionSucceeds_RedirectsToDeleted()
        {
            var userId = Guid.NewGuid();
            var user = new Person { Id = userId };

            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManagerMock.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

            var controller = new ProfileController(_userManagerMock.Object, _techRepositoryMock.Object, _personRepositoryMock.Object, _signInManagerMock.Object);

            var result = await controller.DeleteUser();

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Deleted", redirectToActionResult.ActionName);
            Assert.Equal("Profile", redirectToActionResult.ControllerName);
        }

        [Fact]
        public async Task DeleteUser_WhenDeletionFails_ReturnsViewWithErrors()
        {
            var userId = Guid.NewGuid();
            var user = new Person { Id = userId };
            var errorDescription = "Deletion failed";

            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManagerMock.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = errorDescription }));

            var controller = new ProfileController(_userManagerMock.Object, _techRepositoryMock.Object, _personRepositoryMock.Object, _signInManagerMock.Object);

            var result = await controller.DeleteUser();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
            Assert.Equal(1, viewResult.ViewData.ModelState.ErrorCount);
            Assert.Equal(errorDescription, viewResult.ViewData.ModelState[string.Empty].Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task Deleted_ReturnsViewResult()
        {
            var controller = new ProfileController(_userManagerMock.Object, _techRepositoryMock.Object, _personRepositoryMock.Object, _signInManagerMock.Object);

            var result = await controller.Deleted();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task Edit_WithFreelancerRole_ReturnsRedirectToActionResult()
        {
            var controller = new ProfileController(_userManagerMock.Object, _techRepositoryMock.Object, _personRepositoryMock.Object, _signInManagerMock.Object);
            var userId = Guid.NewGuid();
            var user = new Person { Id = userId };

            var person = new Person
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Location = "New York",
                Freelancer = new Freelancer
                {
                    FreelancerId = Guid.NewGuid(),
                    Specialization = "Software Development",
                    Rate = 50, // Set your desired rate here
                    CreatedDate = DateTime.UtcNow,
                    Technologies = new List<Technology>
                    {
                        new Technology { Id = Guid.NewGuid(), Name = "Tech1" },
                        new Technology { Id = Guid.NewGuid(), Name = "Tech2" },
                        new Technology { Id = Guid.NewGuid(), Name = "Tech3" },
                        new Technology { Id = Guid.NewGuid(), Name = "Tech4" }
                    }
                }
            };
            var formData = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "firstName", "John" },
                { "lastName", "Doe" },
                { "email", "john.doe@example.com" },
                { "location", "New York" },
                { "dateOfBirth", "1990-01-01" },
                { "specialization", "Software Development" },
                { "selectedTechnologies", "Tech1, Tech2, Tech3" }
            });

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, "Freelancer")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _personRepositoryMock.Setup(m => m.GetPersonWithTechnologiesById(userId)).ReturnsAsync(person);
            _techRepositoryMock.Setup(m => m.GetTechnologyWithFreelancerByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new Technology() { Name = "Tech2" });
            var httpContext = new DefaultHttpContext { User = claimsPrincipal };
            httpContext.Request.Form = formData;
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            var result = await controller.Edit(new ProfileViewModel());

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Profile", redirectToActionResult.ControllerName);
        }

        [Fact]
        public async Task Edit_WhenUserIsClient_UpdatesUserAndRedirectsToIndex()
        {
            var userId = Guid.NewGuid();
            var user = new Person
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Location = "New York",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            var editProfileViewModel = new ProfileViewModel
            {
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName",
                Email = "updated@example.com",
                Location = "Updated Location",
                DateOfBirth = new DateOnly(1995, 1, 1),
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, "Client")
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<Person>())).ReturnsAsync(IdentityResult.Success);

            var controller = new ProfileController(_userManagerMock.Object, _techRepositoryMock.Object, _personRepositoryMock.Object, _signInManagerMock.Object);

            controller.TempData = new TempDataDictionary(new Microsoft.AspNetCore.Http.DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var result = await controller.Edit(editProfileViewModel);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Profile", redirectToActionResult.ControllerName);

            _userManagerMock.Verify(m => m.UpdateAsync(user), Times.Once);
        }
    }
}
