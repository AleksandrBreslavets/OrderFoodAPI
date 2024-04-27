using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq.EntityFrameworkCore;
using Moq;
using OrderFoodAPIWebApp.Controllers;
using OrderFoodAPIWebApp.Models;

namespace CategoriesTests
{
    public class CategoriesControllerTests
    {
        //Comment MakeIncludes() in GetCategory
        [Fact]
        public async Task GetCategoryActionResult_ReturnsNotFoundObjectResultForNonexistentCategory()
        {
            //Arrange
            int testId = 111;

            var mockContext = new Mock<FoodOrderAPIContext>();
            mockContext.Setup(c => c.Categories.FindAsync(testId))
                .ReturnsAsync((Category)null);
            var controller = new CategoriesController(mockContext.Object);
            
            //Act
            var res = await controller.GetCategory(testId);

            //Assert
            var actionResult = Assert.IsType<ActionResult<Category>>(res);
            Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetCategoryActionResult_ReturnsOkObjectResultIfCategoryExists()
        {
            //Arrange
            int testId = 1;
            var mockContext = new Mock<FoodOrderAPIContext>();
            mockContext.Setup(c => c.Categories.FindAsync(testId))
                .ReturnsAsync(TestDataHelper.GetFakeCategoriesList().Find(cat=>cat.Id==testId) ?? new Category());

            var controller = new CategoriesController(mockContext.Object);

            //Act
            var res = await controller.GetCategory(testId);

            //Assert
            var actionResult = Assert.IsType<ActionResult<Category>>(res);
            Assert.IsType<OkObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task Test3()
        {
            //Arrange
            int testId = 1;
            var cat = TestDataHelper.GetFakeCategoriesList().Find(cat => cat.Id == testId) ?? new Category();
            var mockContext = new Mock<FoodOrderAPIContext>();
            mockContext.Setup(c => c.Categories.FindAsync(testId))
                .ReturnsAsync(cat);

            var controller = new CategoriesController(mockContext.Object);
    
            //Act
            var res = await controller.DeleteCategory(testId);

            //Assert
            mockContext.Verify(c => c.Categories.Remove(cat), Times.Once);
        }
    }
}   