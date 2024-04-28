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
using Microsoft.AspNetCore.Http;

namespace EasyTalk.Tests.ControllersTests
{
    public class LoginControllerTests
    {
        private readonly AppDbContext _dbContextMock;
        private readonly Mock<UserManager<Person>> _userManagerMock;
        private readonly Mock<IMailService> _mailServiceMock;
        private readonly Mock<SignInManager<Person>> _signInManagerMock;


        public LoginControllerTests()
        {
            _dbContextMock = Mock.Of<AppDbContext>();
            var store = new Mock<IUserStore<Person>>();
            _userManagerMock = new Mock<UserManager<Person>>(store.Object, null, null, null, null, null, null, null, null);
            _mailServiceMock = new Mock<IMailService>();

            var userManagerMock = new Mock<UserManager<Person>>(
                /* IUserStore<TUser> store */Mock.Of<IUserStore<User>>(),
                /* IOptions<IdentityOptions> optionsAccessor */null,
                /* IPasswordHasher<TUser> passwordHasher */null,
                /* IEnumerable<IUserValidator<TUser>> userValidators */null,
                /* IEnumerable<IPasswordValidator<TUser>> passwordValidators */null,
                /* ILookupNormalizer keyNormalizer */null,
                /* IdentityErrorDescriber errors */null,
                /* IServiceProvider services */null,
                /* ILogger<UserManager<TUser>> logger */null);

            var signInManagerMock = new Mock<SignInManager<Person>>(
                userManagerMock.Object,
                /* IHttpContextAccessor contextAccessor */Mock.Of<IHttpContextAccessor>(),
                /* IUserClaimsPrincipalFactory<TUser> claimsFactory */Mock.Of<IUserClaimsPrincipalFactory<User>>(),
                /* IOptions<IdentityOptions> optionsAccessor */null,
                /* ILogger<SignInManager<TUser>> logger */null,
                /* IAuthenticationSchemeProvider schemes */null,
                /* IUserConfirmation<TUser> confirmation */null);


        }


    }
}
