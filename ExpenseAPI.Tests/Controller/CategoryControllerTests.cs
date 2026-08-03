using ExpenseAPI.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Linq.Expressions;
using UnitTests_ExpenseAPI;
using UnitTests_ExpenseAPI.Controllers;
using UnitTests_ExpenseAPI.DTO.CategoryDTO;
using UnitTests_ExpenseAPI.Models;
using UnitTests_ExpenseAPI.Repo;
using Xunit;

namespace ExpenseAPI.Tests.Controller;

public class CategoryControllerTest
{
    private readonly Mock<IBaseRepo<Transaction>> _categoryServiceMock;
    private readonly CategoryController controller;

    public CategoryControllerTest()
    {
        _categoryServiceMock = new Mock<IBaseRepo<Category>>();
        var dummyLogger = NullLogger<CategoryController>.Instance;
        controller = new CategoriesController(_categoryServiceMock.Object, dummyLogger);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithCategoryList()
    {
        // Arrange
        _categoryServiceMock
            .Setup(s => s.GetAll(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<string[]>()))
            .ReturnsAsync(CategoryFixture.DefaultCategoryList);

        // Act
        var response = await controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(response);
        Assert.IsType<List<SummaryCategoryDTO>>(okResult.Value);

        _categoryServiceMock.Verify(
            s => s.GetAll(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<string[]>()),
            Times.Once);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(99)]
    public async Task GetById_ReturnsOk_WhenCategoryExists(int id)
    {
        // Arrange
        var category = CategoryFixture.DefaultCategory(id);
        _categoryServiceMock.Setup(s => s.GetByID(id)).ReturnsAsync(category);

        // Act
        var result = await controller.GetByID(id);

        // Assert
        if (CategoryFixture.DefaultCategoryList.Any(c => c.ID == id))
        {
            var okResult = Assert.IsType<OkObjectResult>(result);
            var item = Assert.IsType<SummaryCategoryDTO>(okResult.Value);
        }
        else
        {
            Assert.IsType<NotFoundResult>(result);
        }

        _categoryServiceMock.Verify(s => s.GetByID(id), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Create_ReturnsOk_WhenServiceCreatesCategory(bool createSucceeded)
    {
        // Arrange
        Category returnModel = createSucceeded ? CategoryFixture.DefaultCategory(1) : null;
        _categoryServiceMock
            .Setup(s => s.Create(It.IsAny<Category>()))
            .ReturnsAsync(returnModel);

        // Act
        var response = await controller.Create(CategoryFixture.CreateCategoryDTO);

        // Assert
        if (createSucceeded)
        {
            var result = Assert.IsType<OkObjectResult>(response);
            var created = Assert.IsType<SummaryCategoryDTO>(result.Value);
        }
        else
        {
            var result = Assert.IsType<BadRequestObjectResult>(response);
            var wrongDto = Assert.IsType<CreateCategoryDTO>(result.Value);
        }

        _categoryServiceMock.Verify(s => s.Create(It.IsAny<Category>()), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Delete_ReturnsOk_WhenDeleteSucceeds(bool deleteSucceeded)
    {
        // Arrange
        int id = 1;
        _categoryServiceMock.Setup(s => s.Delete(id)).ReturnsAsync(deleteSucceeded);

        // Act
        var response = await controller.Delete(id);

        // Assert
        if (deleteSucceeded) Assert.IsType<OkResult>(response);
        else Assert.IsType<BadRequestResult>(response);

        _categoryServiceMock.Verify(s => s.Delete(id), Times.Once);
    }
}