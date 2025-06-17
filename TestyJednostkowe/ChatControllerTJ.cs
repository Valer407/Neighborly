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
    public class ChatControllerTJ
    {
        [Fact]
        public void Index_ReturnsViewWithChats_WhenUserIsLoggedIn()
        {
            // Arrange
            var context = DbContextHelper.GetInMemoryDbContext();
            var user1 = new User
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
            };
            var user2 = new User
            {
                UserId = 3,
                FirstName = "Adam",
                LastName = "Nowak",
                IsActive = true,
                AvatarUrl = "kot.png",
                RatingAvg = 4,
                CreatedAt = DateTime.Now,
                About = "O mnie 123",
                Email = "adam.nowak@example.com",
                PasswordHash = "dfdffdsf"
            };
            context.Users.AddRange(user1, user2);
            var listing = new Listings
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
            };
            context.Listings.Add(listing);
            var chat = new Chats { ChatId = 1, User1Id = 1, User2Id = 2, ListingId = 1, CreatedAt = DateTime.UtcNow };
            context.Chats.Add(chat);
            var message = new Messages { MessageId = 1, ChatId = 1, SenderId = 2, Content = "Hej", SentAt = DateTime.UtcNow };
            context.Messages.Add(message);
            context.SaveChanges();

            var controller = new ChatController(context);
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new DummySession();
            httpContext.Session.SetInt32("UserId", 1);
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Act
            var result = controller.Index(null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<MessagesIndexViewModel>(viewResult.Model);
            Assert.Single(model.Chats);
            Assert.Null(model.ActiveChat);
        }
        [Fact]
        public void SendMessage_AddsMessage_WhenTextIsValid()
        {
            // Arrange
            var context = DbContextHelper.GetInMemoryDbContext();
            var user = new User
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
            };
            var chat = new Chats { ChatId = 1, User1Id = 1, User2Id = 2 };
            context.Users.Add(user);
            context.Chats.Add(chat);
            context.SaveChanges();

            var controller = new ChatController(context);
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new DummySession();
            httpContext.Session.SetInt32("UserId", 1);
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Act
            var result = controller.SendMessage(1, "Wiadomość testowa");

            // Assert
            var msg = context.Messages.FirstOrDefault();
            Assert.NotNull(msg);
            Assert.Equal("Wiadomość testowa", msg.Content);
        }
        [Fact]
        public void StartWithUser_CreatesChat_IfNotExists()
        {
            // Arrange
            var context = DbContextHelper.GetInMemoryDbContext();
            var user1 = new User
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
            };
            var user2 = new User
            {
                UserId = 3,
                FirstName = "Adam",
                LastName = "Nowak",
                IsActive = true,
                AvatarUrl = "kot.png",
                RatingAvg = 4,
                CreatedAt = DateTime.Now,
                About = "O mnie 123",
                Email = "adam.nowak@example.com",
                PasswordHash = "dfdffdsf"
            };
            context.Users.AddRange(user1, user2);
            context.SaveChanges();

            var controller = new ChatController(context);
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new DummySession();
            httpContext.Session.SetInt32("UserId", 1);
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Act
            var result = controller.StartWithUser(2);

            // Assert
            Assert.Equal(1, context.Chats.Count());
        }
        [Fact]
        public void RateChat_AddsRating_WhenValid()
        {
            // Arrange
            var context = DbContextHelper.GetInMemoryDbContext();

            var user1 = new User
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
            };
            var user2 = new User
            {
                UserId = 3,
                FirstName = "Adam",
                LastName = "Nowak",
                IsActive = true,
                AvatarUrl = "kot.png",
                RatingAvg = 4,
                CreatedAt = DateTime.Now,
                About = "O mnie 123",
                Email = "adam.nowak@example.com",
                PasswordHash = "dfdffdsf"
            };
            var listing = new Listings
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
            };
            var chat = new Chats { ChatId = 1, User1Id = 1, User2Id = 2, ListingId = 1, ClosedAt = DateTime.UtcNow };

            context.Users.AddRange(user1, user2);
            context.Listings.Add(listing);
            context.Chats.Add(chat);
            context.SaveChanges();

            var controller = new ChatController(context);
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new DummySession();
            httpContext.Session.SetInt32("UserId", 1);
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Act
            var result = controller.RateChat(1, 5, "Bardzo pomocny");

            // Assert
            var rating = context.User_Ratings.FirstOrDefault();
            Assert.NotNull(rating);
            Assert.Equal(5, rating.Score);
            Assert.Equal("Bardzo pomocny", rating.Comment);
        }
        [Fact]
        public void CloseChat_SetsClosedAt_WhenOpen()
        {
            // Arrange
            var context = DbContextHelper.GetInMemoryDbContext();

            var user = new User
            {
                UserId = 3,
                FirstName = "Adam",
                LastName = "Nowak",
                IsActive = true,
                AvatarUrl = "kot.png",
                RatingAvg = 4,
                CreatedAt = DateTime.Now,
                About = "O mnie 123",
                Email = "adam.nowak@example.com",
                PasswordHash = "dfdffdsf"
            };
            var chat = new Chats { ChatId = 1, User1Id = 1, User2Id = 2, ClosedAt = null };

            context.Users.Add(user);
            context.Chats.Add(chat);
            context.SaveChanges();

            var controller = new ChatController(context);
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new DummySession();
            httpContext.Session.SetInt32("UserId", 1);
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Act
            var result = controller.CloseChat(1);

            // Assert
            var updatedChat = context.Chats.First(c => c.ChatId == 1);
            Assert.NotNull(updatedChat.ClosedAt);
        }
        [Fact]
        public void StartFromListing_CreatesChatAndMessage_WhenValid()
        {
            // Arrange
            var context = DbContextHelper.GetInMemoryDbContext();

            var listing = new Listings { ListingId = 1, Title = "Pomoc", UserId = 2 };
            var user1 = new User
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
            };
            var user2 = new User
            {
                UserId = 3,
                FirstName = "Adam",
                LastName = "Nowak",
                IsActive = true,
                AvatarUrl = "kot.png",
                RatingAvg = 4,
                CreatedAt = DateTime.Now,
                About = "O mnie 123",
                Email = "adam.nowak@example.com",
                PasswordHash = "dfdffdsf"
            };

            context.Users.AddRange(user1, user2);
            context.Listings.Add(listing);
            context.SaveChanges();

            var controller = new ChatController(context);
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new DummySession();
            httpContext.Session.SetInt32("UserId", 1);
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Act
            var result = controller.StartFromListing(1, "Cześć, jestem zainteresowany");

            // Assert
            var chat = context.Chats.FirstOrDefault();
            var message = context.Messages.FirstOrDefault();
            Assert.NotNull(chat);
            Assert.NotNull(message);
            Assert.Equal("Cześć, jestem zainteresowany", message.Content);
            Assert.Equal(chat.ChatId, message.ChatId);
        }

    }
}
