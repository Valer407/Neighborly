using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Neighborly.Controllers;
using Neighborly.Models.DBModels;
using Xunit;

namespace TestyJednostkowe
{
    public class HomeControllerTj
    {
        [Fact]
        public void Index_ReturnsViewWithCategoriesAndIcons()
        {
            // Arrange
            var context = DbContextHelper.GetInMemoryDbContext();
            context.Categories.AddRange(
                new Categories { CategoryId = 1,Name = "Jedzenie", Icon = "1", IconSvg = "a" },
                new Categories { CategoryId = 2,Name = "Transport", Icon = "2", IconSvg = "b" }
            );
            context.SaveChanges();

            var loggerMock = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(loggerMock.Object, context);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Categories>>(viewResult.Model);
            Assert.All(model, c => Assert.False(string.IsNullOrEmpty(c.IconSvg)));
        }
    }
}
