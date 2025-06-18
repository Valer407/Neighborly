using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Neighborly.Controllers;
using Neighborly.Models.DBModels;
using Neighborly.Models;
using Xunit;

namespace TestyJednostkowe
{
    public class AdminControllerTJ
    {
        [Fact]
        public void Index_ReturnsViewWithAllData()
        {
            var context = DbContextHelper.GetInMemoryDbContext();
            context.Users.Add(new User { UserId = 1, FirstName = "Jan", LastName = "Kowalski", Email = "jan@x.pl", IsActive = true });
            context.Categories.Add(new Categories { CategoryId = 1, Name = "Test", Icon = "" });
            context.Listings.Add(new Listings
            {
                ListingId = 1,
                Title = "Tytuł",
                UserId = 1,
                CategoryId = 1,
                CreatedAt = DateTime.UtcNow,
                Status = "aktywne"
            });
            context.Reports.Add(new Reports
            {
                ReportId = 1,
                ListingId = 1,
                ReporterId = 1,
                Reason = "Spam",
                Status = "Nowe",
                CreatedAt = DateTime.UtcNow
            });
            context.SaveChanges();

            var controller = new AdminController(context);
            var result = controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AdminPanelViewModel>(view.Model);
            Assert.Single(model.Listings);
            Assert.Single(model.Users);
            Assert.Single(model.Categories);
            Assert.Single(model.Reports);
        }

        [Fact]
        public void DeleteListing_RemovesListing_WhenIdIsValid()
        {
            var context = DbContextHelper.GetInMemoryDbContext();
            context.Listings.Add(new Listings { ListingId = 1, Title = "Do usunięcia" });
            context.SaveChanges();

            var controller = new AdminController(context);
            var result = controller.DeleteListing(1);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Empty(context.Listings.ToList());
        }

        [Fact]
        public void ToggleUser_TogglesIsActiveStatus()
        {
            var context = DbContextHelper.GetInMemoryDbContext();
            context.Users.Add(new User { UserId = 1, FirstName = "Test", LastName = "Użytkownik", IsActive = true });
            context.SaveChanges();

            var controller = new AdminController(context);
            var result = controller.ToggleUser(1);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.False(context.Users.First().IsActive);
        }

        [Fact]
        public void DeleteReport_RemovesReport_WhenIdIsValid()
        {
            var context = DbContextHelper.GetInMemoryDbContext();
            context.Reports.Add(new Reports { ReportId = 1, Reason = "Spam" });
            context.SaveChanges();

            var controller = new AdminController(context);
            var result = controller.DeleteReport(1);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Empty(context.Reports.ToList());
        }

        [Fact]
        public void AddCategory_AddsCategory_WhenModelIsValid()
        {
            var context = DbContextHelper.GetInMemoryDbContext();
            var controller = new AdminController(context);
            var model = new CategoryViewModel { Name = "Nowa", IconSvg = "svg", Id = "5" };

            var result = controller.AddCategory(model);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Single(context.Categories);
            Assert.Equal("Nowa", context.Categories.First().Name);
        }

        [Fact]
        public void AddCategory_ReturnsPartial_WhenModelIsInvalid()
        {
            var context = DbContextHelper.GetInMemoryDbContext();
            var controller = new AdminController(context);
            controller.ModelState.AddModelError("Name", "Wymagane");

            var result = controller.AddCategory(new CategoryViewModel());

            var partial = Assert.IsType<PartialViewResult>(result);
            Assert.Equal("_AddCategoryModal", partial.ViewName);
        }
    }
}
