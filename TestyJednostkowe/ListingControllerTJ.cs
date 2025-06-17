using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Neighborly.Controllers;
using Neighborly.Data;
using Neighborly.Models.DBModels;
using Neighborly.Models;
using Xunit;
using Microsoft.AspNetCore.Http;

namespace TestyJednostkowe
{
    public class ListingControllerTJ
    {
        private readonly DbContextOptions<NeighborlyContext> _options;
        private readonly Mock<IWebHostEnvironment> _envMock;
        private readonly Mock<IConfiguration> _configMock;

        public ListingControllerTJ()
        {
            _options = new DbContextOptionsBuilder<NeighborlyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // nowa baza dla każdego testu
                .Options;

            _envMock = new Mock<IWebHostEnvironment>();
            _envMock.Setup(e => e.WebRootPath).Returns(Directory.GetCurrentDirectory());

            _configMock = new Mock<IConfiguration>();
            _configMock.Setup(c => c["GoogleMaps:ApiKey"]).Returns("FAKE_API_KEY");
        }
        [Fact]
        public void Index_FiltersListingsBySearchQuery()
        {
            using var context = new NeighborlyContext(_options);
            context.Listing_Types.Add(new Listing_types { ListingTypeId = 1, Name = "Oferuję pomoc" });
            context.Cities.Add(new Cities { CityId = 1, Name = "Miasto" });
            context.Districts.Add(new Distircts { DistrictId = 1, Name = "Dzielnica", CityId = 1 });
            context.Users.Add(new User
            {
                UserId = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                IsActive = true,
                AvatarUrl = "avatar.png",
                RatingAvg = 4,
                CreatedAt = DateTime.Now,
                About = "O mnie",
                Email = "jan.kowalski@example.com",
                PasswordHash = "hash123"
            });
            context.Categories.Add(new Categories { CategoryId = 1, 
                Name = "Pomoc",
                Icon = "test-icon.png",
                IconSvg = "<svg></svg>"
            });
            context.Listings.Add(new Listings
            {
                ListingId = 1,
                UserId = 1,
                CityId = 1,
                DistrictId = 1,
                ListingTypeId = 1,
                Title = "Oferta 1",
                Description = "Opis oferty",
                CreatedAt = DateTime.Now,
                Latitude = 0,
                Longitude = 0
            });
            context.SaveChanges();

            var controller = new ListingsController(context, _envMock.Object, _configMock.Object);
            var result = controller.Index("Oferta 1", null, null) as ViewResult;

            var model = result?.Model as ListingsIndexViewModel;
            Assert.Single(model.Listings);
        }
        [Fact]
        public void Details_ReturnsNotFound_WhenListingDoesNotExist()
        {
            using var context = new NeighborlyContext(_options);
            var controller = new ListingsController(context, _envMock.Object, _configMock.Object);
            var result = controller.Details(999);

            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public void Favorite_AddsListingToFavourites()
        {
            using var context = new NeighborlyContext(_options);
            context.Users.Add(new User
            {
                UserId = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                IsActive = true,
                AvatarUrl = "avatar.png",
                RatingAvg = 4,
                CreatedAt = DateTime.Now,
                About = "O mnie",
                Email = "jan.kowalski@example.com",
                PasswordHash = "hash123"
            });
            context.Listings.Add(new Listings
            {
                ListingId = 1,
                UserId = 1,
                CityId = 1,
                DistrictId = 1,
                ListingTypeId = 1,
                Title = "Oferta 1",
                Description = "Opis oferty",
                CreatedAt = DateTime.Now,
                Latitude = 0,
                Longitude = 0,
            });
            context.SaveChanges();

            var controller = new ListingsController(context, _envMock.Object, _configMock.Object);
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new DummySession();
            httpContext.Session.SetInt32("UserId", 1);
            controller.ControllerContext.HttpContext = httpContext;

            var result = controller.Favorite(1) as JsonResult;
            Assert.NotNull(result);

            var prop = result.Value.GetType().GetProperty("isFavorite");
            Assert.NotNull(prop);

            bool isFavorite = (bool)prop.GetValue(result.Value);
            Assert.True(isFavorite);
        }
        [Fact]
        public void Delete_RemovesListing_IfUserIsOwner()
        {
            using var context = new NeighborlyContext(_options);
            context.Users.Add(new User
            {
                UserId = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                IsActive = true,
                AvatarUrl = "avatar.png",
                RatingAvg = 4,
                CreatedAt = DateTime.Now,
                About = "O mnie",
                Email = "jan.kowalski@example.com",
                PasswordHash = "hash123"
            });
            context.Listings.Add(new Listings
            {
                ListingId = 1,
                UserId = 1,
                CityId = 1,
                DistrictId = 1,
                ListingTypeId = 1,
                Title = "Oferta 1",
                Description = "Opis oferty",
                CreatedAt = DateTime.Now,
                Latitude = 0,
                Longitude = 0
            });
            context.Favourites.Add(new Favourites { ListingId = 1, UserId = 1 });
            context.Listing_Images.Add(new Listing_images { ListingId = 1, Url = "/uploads/listings/test.jpg" });
            context.SaveChanges();

            var fakePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "listings");
            Directory.CreateDirectory(fakePath);
            File.Create(Path.Combine(fakePath, "test.jpg")).Dispose();

            var controller = new ListingsController(context, _envMock.Object, _configMock.Object);
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new DummySession();
            httpContext.Session.SetInt32("UserId", 1);
            controller.ControllerContext.HttpContext = httpContext;

            var result = controller.Delete(1);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Empty(context.Listings.ToList());
        }
        [Fact]
        public void GetReportModal_ReturnsPartialViewWithCorrectModel()
        {
            using var context = new NeighborlyContext(_options);
            var controller = new ListingsController(context, _envMock.Object, _configMock.Object);

            var result = controller.GetReportModal(5) as PartialViewResult;

            Assert.NotNull(result);
            Assert.Equal("_ReportModal", result.ViewName);
            var model = result.Model as ReportViewModel;
            Assert.Equal(5, model.ListingId);
        }
    }

}
