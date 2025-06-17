using Moq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.InMemory;
using Xunit;
using Neighborly.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighborly.Models.DBModels;
using Neighborly.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Neighborly.Data;


namespace TestyJednostkowe
{
    public class AccountControllerTJ
    {
        [Fact]
        public void MyAccount_ReturnsViewWithModel_WhenUserIsLoggedIn() //test jednostokwy dla metody MyAccount
        {
            var context = DbContextHelper.GetInMemoryDbContext();
            var envMock = new Mock<IWebHostEnvironment>();
            var controller = new AccountController(context, envMock.Object);
            var userId = 1;

            context.Users.Add(new User
            {
                UserId = userId,
                FirstName = "Test",
                LastName = "Testowy",
                Email = "test@example.com",
                PasswordHash = "hashed_password",
                AvatarUrl = "",
                IsActive = true
            });
            context.Listings.Add(new Listings
            {
                ListingId = 1,
                UserId = userId,
                Title = "Test listing",
                Description = "Opis testowej oferty",
                CreatedAt = DateTime.UtcNow,
                City = new Cities { Name = "Białystok" },
                District = new Distircts { Name = "Centrum" },
                ListingType = new Listing_types { Name = "Oferuję pomoc" }
            });
            context.SaveChanges();

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new DummySession();
            httpContext.Session.SetInt32("UserId", userId);
            controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = controller.MyAccount();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<MyAccountViewModel>(viewResult.Model);
            Assert.Single(model.Listings);
        }
        [Fact]
        public void Profile_ReturnsViewWithCorrectUser_WhenIdIsValid() //test jednostokwy dla metody Profile
        {
            var context = DbContextHelper.GetInMemoryDbContext();
            var env = new Mock<IWebHostEnvironment>();
            var controller = new AccountController(context, env.Object);

            context.Users.Add(new User
            {
                UserId = 2,
                FirstName = "Anna",
                LastName = "Nowak",
                Email = "anna.nowak@example.com",
                PasswordHash = "hashed_password",
                City = new Cities { Name = "Gdańsk" },
                AvatarUrl = "",
                District = new Distircts { Name = "Wrzeszcz" }
            });
            context.SaveChanges();

            var result = controller.Profile(2);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProfileViewModel>(view.Model);
            Assert.Equal("Anna Nowak", model.User.Name);
            Assert.Equal("Gdańsk", model.User.City);
        }
        [Fact]
        public void EditProfile_ReturnsViewWithUserModel_WhenUserIsLoggedIn() //test jednostokwy dla metody EditProfile
        {
            var context = DbContextHelper.GetInMemoryDbContext();
            var controller = new AccountController(context, new Mock<IWebHostEnvironment>().Object);

            context.Users.Add(new User
            {
                UserId = 5,
                FirstName = "Jan",
                LastName = "Kowalski",
                Email = "jan@example.com",
                PasswordHash = "hashed_password",
                City = new Cities { Name = "Warszawa" },
                AvatarUrl = "",
                District = new Distircts { Name = "Mokotów" }
            });
            context.SaveChanges();

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new DummySession();
            httpContext.Session.SetInt32("UserId", 5);
            controller.ControllerContext.HttpContext = httpContext;

            var result = controller.EditProfile();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<EditProfileViewModel>(viewResult.Model);
            Assert.Equal("Jan", model.FirstName);
        }
        [Fact]
        public void Login_ValidCredentials_RedirectsToHome() //3 testy do metody login
        {
            var context = DbContextHelper.GetInMemoryDbContext();
            var env = new Mock<IWebHostEnvironment>();
            var controller = new AccountController(context, env.Object);

            var user = new User
            {
                UserId = 10,
                Email = "test@valid.com",
                PasswordHash = new PasswordHasher<User>().HashPassword(null!, "pass123"),
                FirstName = "Test",
                LastName = "User",
                IsActive = true
            };
            context.Users.Add(user);
            context.SaveChanges();

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new DummySession();
            controller.ControllerContext.HttpContext = httpContext;

            var result = controller.Login("test@valid.com", "pass123", false);

            var redirect = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/", redirect.Url);
        }
        [Fact]
        public void Login_InvalidCredentials_ReturnsViewWithError()
        {
            var context = DbContextHelper.GetInMemoryDbContext();
            var env = new Mock<IWebHostEnvironment>();
            var controller = new AccountController(context, env.Object);

            var user = new User
            {
                UserId = 11,
                Email = "wrong@user.com",
                PasswordHash = new PasswordHasher<User>().HashPassword(null!, "correct"),
                IsActive = true
            };
            context.Users.Add(user);
            context.SaveChanges();

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new DummySession();
            controller.ControllerContext.HttpContext = httpContext;

            var result = controller.Login("wrong@user.com", "wrongpass", false);

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }
        [Fact]
        public void Login_MissingData_ReturnsViewWithModelError()
        {
            var context = DbContextHelper.GetInMemoryDbContext();
            var env = new Mock<IWebHostEnvironment>();
            var controller = new AccountController(context, env.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new DummySession();
            controller.ControllerContext.HttpContext = httpContext;

            var result = controller.Login("", "", false);

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }
        [Fact]
        public void Register_EmailExists_ReturnsViewWithModelError()
        {
            var context = DbContextHelper.GetInMemoryDbContext();
            var env = new Mock<IWebHostEnvironment>();
            var controller = new AccountController(context, env.Object);

            context.Users.Add(new User { Email = "duplikat@mail.com" });
            context.SaveChanges();

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new DummySession();
            controller.ControllerContext.HttpContext = httpContext;

            var result = controller.Register("Ala", "Nowak", "duplikat@mail.com", "123", "123", true);

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }
        [Fact]
        public void Register_DisagreeToTerms_ReturnsViewWithModelError()
        {
            var context = DbContextHelper.GetInMemoryDbContext();
            var env = new Mock<IWebHostEnvironment>();
            var controller = new AccountController(context, env.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new DummySession();
            controller.ControllerContext.HttpContext = httpContext;

            var result = controller.Register("Kasia", "Z", "kasia@z.pl", "aaa", "aaa", false);

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }
        [Fact]
        public void Logout_ClearsSessionAndRedirects()
        {
            var context = DbContextHelper.GetInMemoryDbContext();
            var env = new Mock<IWebHostEnvironment>();
            var controller = new AccountController(context, env.Object);

            var httpContext = new DefaultHttpContext();
            var session = new DummySession();
            session.SetInt32("UserId", 99);
            httpContext.Session = session;
            controller.ControllerContext.HttpContext = httpContext;

            var result = controller.Logout();

            Assert.Null(httpContext.Session.GetInt32("UserId"));
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
        }

    }
}
