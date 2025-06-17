using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighborly.Controllers;
using Neighborly.Models.DBModels;
using Neighborly.Models;
using Xunit;

namespace TestyJednostkowe
{
    public class ProfileControllerTJ
    {
        [Fact]
        public void Index_ReturnsViewWithCorrectData_WhenUserExists()
        {
            // Arrange
            var context = DbContextHelper.GetInMemoryDbContext();

            var city = new Cities { CityId = 1, Name = "Miasto" };
            var district = new Distircts { DistrictId = 1, Name = "Dzielnica" };
            var listingType = new Listing_types { ListingTypeId = 1, Name = "Oferuję pomoc" };
            var category = new Categories { CategoryId = 1, Name = "Kategoria", Icon = "v", IconSvg = "a" };

            var user = new User
            {
                UserId = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                IsActive = true,
                City = city,
                District = district,
                AvatarUrl = "avatar.png",
                RatingAvg = 4,
                CreatedAt = DateTime.Now,
                About = "O mnie",
                Email = "jan.kowalski@example.com",
                PasswordHash = "hash123"
            };

            var rater = new User
            {
                UserId = 2,
                FirstName = "Kuba",
                LastName = "Nowak",
                IsActive = true,
                City = city,
                District = district,
                AvatarUrl = "kot.png",
                RatingAvg = 3,
                CreatedAt = DateTime.Now,
                About = "O mnie",
                Email = "kuba.nowak@example.com",
                PasswordHash = "dsfsdfs"
            };

            context.Cities.Add(city);
            context.Districts.Add(district);
            context.Listing_Types.Add(listingType);
            context.Categories.Add(category);
            context.Users.AddRange(user, rater);

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
                City = city,
                District = district,
                ListingType = listingType,
                Latitude = 0,
                Longitude = 0
            });

            context.User_Ratings.Add(new User_ratings
            {
                RateeId = 1,
                RaterId = 2,
                Score = 5,
                Comment = "Super!",
                CategoryId = 1,
                CreatedAt = DateTime.Now
            });

            context.SaveChanges();

            var controller = new ProfileController(context);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new DummySession();
            httpContext.Session.SetInt32("UserId", 1);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            // Act
            var result = controller.Index(null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ProfileViewModel>(viewResult.Model);

            Assert.Equal(1, model.User.Id);
            Assert.Equal("Jan Kowalski", model.User.Name);
            Assert.Single(model.Listings);
            Assert.Single(model.Reviews);
        }
    }
}
