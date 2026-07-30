using DocumentFormat.OpenXml.Drawing.Diagrams;
using ExpenseAPI.Tests.Fixtures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Linq.Expressions;
using UnitTests_ExpenseAPI;
using UnitTests_ExpenseAPI.DTO.CategoryDTO;
using UnitTests_ExpenseAPI.DTO.ExpensesDTO;
using UnitTests_ExpenseAPI.Repo;

namespace ExpenseAPI.Tests.Controller;

public class EspenseControllerTest
{
    private Mock<IBaseRepo<Expense>> _expenseServiceMock;
    private readonly ExpensesController controller;


    public EspenseControllerTest()
    {
        _expenseServiceMock = new Mock<IBaseRepo<Expense>>();
        ILogger<ExpensesController> dummyLogger = NullLogger<ExpensesController>.Instance;
        controller = new ExpensesController(_expenseServiceMock.Object, dummyLogger);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithExpenseList()
    {
        // 1 .Arrange
        _expenseServiceMock.Setup(s => s.GetAll(
                    It.IsAny<Expression<Func<Expense, bool>>>(),
                    "Category")).ReturnsAsync(ExpenseFixture.DefaultExpenseList);

        // 2 .Act
        var expensesListResponse = await controller.GetAll();

        //3. Assert

        //Test response type
        var okResult = Assert.IsType<OkObjectResult>(expensesListResponse);
        Assert.IsType<List<SummaryExpenseDTO>>(okResult.Value);

        _expenseServiceMock.Verify(s => s.GetAll(null, "category"), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task GetById_ReturnsOk_WhenExpenseExists(int id)
    {
        // 1 .Arrange
        var expense = ExpenseFixture.DefaultExpenseList.FirstOrDefault(m => m.ID == id);

        _expenseServiceMock.Setup(x => x.GetByID(id))
            .ReturnsAsync(expense);


        // 2 .Act
        var result = await controller.GetByID(id);


        // 3 .Assert
        if (ExpenseFixture.DefaultExpenseList.Any(e => e.ID == id))
        {
            var okResult = Assert.IsType<OkObjectResult>(result);
            var item = Assert.IsType<SummaryExpenseDTO>(okResult.Value);
        }

        else
        {
            Assert.IsType<NotFoundResult>(result);
        }

        _expenseServiceMock.Verify(x => x.GetByID(id), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Create_ReturnsOk_WhenServiceCreatesExpense(bool createSucceeded)
    {
        // 1. Arrange
        Expense returnModel = createSucceeded ? ExpenseFixture.DefaultExpense(1, DateOnly.MinValue) : null;

        _expenseServiceMock
                .Setup(s => s.Create(It.IsAny<Expense>()))
                .ReturnsAsync(returnModel);


        // 2. Act
        var response = await controller.Create(ExpenseFixture.CreateExpenseDTO);

        // 3. Assert

        if (createSucceeded)
        {
            var result = Assert.IsType<OkObjectResult>(response);
            var created = Assert.IsType<SummaryExpenseDTO>(result.Value);
        }

        else
        {
            var result = Assert.IsType<BadRequestObjectResult>(response);
            var wrongDto = Assert.IsType<CreateExpenseDTO>(result.Value);
        }

        _expenseServiceMock.Verify(s => s.Create(It.IsAny<Expense>()), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(20)]
    public async Task GetByMonth_ReturnsOk_WhenMonthIsValid(int month)
    {
        //1.Arrange
        _expenseServiceMock.Setup(s => s.GetAll(
            It.IsAny<Expression<Func<Expense, bool>>>(),
            "Category")).ReturnsAsync(ExpenseFixture.DefaultExpenseList);

        //2.Act
        var response = await controller.GetByMonth(month);

        //3.Assert

        if (month < 1 || month > 12)
        { 
            Assert.IsType<BadRequestObjectResult>(response);
            _expenseServiceMock.Verify(
                s => s.GetAll(It.IsAny<Expression<Func<Expense, bool>>>(), "Category"),
                Times.Never);
        }

        else
        {
            Assert.IsType<OkObjectResult>(response);
            _expenseServiceMock.Verify(s => s.GetAll(It.IsAny<Expression<Func<Expense, bool>>>(), "Category"), Times.Once);
        }
    }



    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Delete_ReturnsOk_WhenDeleteSucceeds(bool isIDValid)
    {
        //Arrange
        int id = 1;
        _expenseServiceMock.Setup(s=> s.Delete(id)).ReturnsAsync(isIDValid);


        //Act
        var response = await controller.Delete(id);

        //Assert
        if (isIDValid) Assert.IsType<OkResult>(response);

        else
        {
            Assert.IsType<BadRequestResult>(response);
        }

        _expenseServiceMock.Verify(s => s.Delete(id), Times.Once);
    }
}




