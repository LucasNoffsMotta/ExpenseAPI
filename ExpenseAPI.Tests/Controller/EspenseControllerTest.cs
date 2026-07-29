using DocumentFormat.OpenXml.Drawing.Diagrams;
using ExpenseAPI.Tests.Fixtures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
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
    public async Task GetAll_ActionExecutes_CheckResultType_ReturnExpensesDTO()
    {
        //Arrange
        OkObjectResult baseResponse = new OkObjectResult(ExpenseFixture.DefaultExpenseList.Select(m => ExpenseMappings.ExpenseModelToSummaryDTO(m)));
        _expenseServiceMock.Setup(um => um.GetAll(null)).ReturnsAsync(ExpenseFixture.DefaultExpenseList);

        // 2 .Act
        var expensesListResponse = await controller.GetAll();

        //3. Assert

        //Test response type
        var okResult = Assert.IsType<OkObjectResult>(expensesListResponse);
        Assert.IsType<List<SummaryExpenseDTO>>(okResult.Value);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task GetById_ActionExecutes_CheckResultType_ReturnSingleObject(int id)
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

            Assert.Equal(
                ExpenseMappings.ExpenseModelToSummaryDTO(ExpenseFixture.DefaultExpenseList.Where(m => m.ID == id).First()).Valor, item.Valor);
        }

        else
        {
            Assert.IsType<NotFoundResult>(result);
        }
    }

    [Fact]
    public async Task Create_ActionExecute_CheckResultType()
    {
        // 1 .Arrange
        var returnModel = ExpenseMappings.ExpenseDtoToModel(ExpenseFixture.CreateExpenseDTO);

        UnitTests_ExpenseAPI.Models.Category mockCat = new UnitTests_ExpenseAPI.Models.Category
        {
            ID = 1,
            Description = "Test",
            HexadecimalColor = "xxxxx"
        };

        returnModel.Category = mockCat;

        //Service
        _expenseServiceMock
                .Setup(s => s.Create(It.IsAny<Expense>()))
                .ReturnsAsync(returnModel);


        // 2. Act
        var response = await controller.Create(ExpenseFixture.CreateExpenseDTO);

        // 3. Assert
        var result = Assert.IsType<OkObjectResult>(response);
        var created = Assert.IsType<SummaryExpenseDTO>(result.Value);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(20)]
    public async Task GetByMonth_ActionExecute_CheckResultType(int month)
    {
        //1.Arrange
        _expenseServiceMock.Setup(s => s.GetAll(e => e.Date.Month == month, "Category")).ReturnsAsync(ExpenseFixture.DefaultExpenseList);

        //2.Act
        var response = await controller.GetByMonth(month);

        //3.Assert

        if (month < 1 || month > 12)
        { 
            Assert.IsType<BadRequestObjectResult>(response);
        }

        else
        {
            Assert.IsType<OkObjectResult>(response);
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    [InlineData(11)]
    [InlineData(12)]
    [InlineData(0)]
    [InlineData(20)]
    public async Task GetByMonth_ActionExecute_CheckReturnData(int month)
    {
        //1.Arrange
        _expenseServiceMock.Setup(s => s.GetAll(e => e.Date.Month == month, "Category")).ReturnsAsync(ExpenseFixture.DefaultExpenseList);

        //2.Act
        var response = await controller.GetByMonth(month);

        //3.Assert
        if (month >= 1 && month <= 12)
        {
            var okResult = response as OkObjectResult;
            var responseList = okResult!.Value;
            Assert.IsType<List<SumaryCategoryDTO>>(responseList);

            foreach (var v in responseList as List<SummaryExpenseDTO>)
            {
                Assert.True(v.Data!.Value.Month == month);
            }
        }
    }


    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Delete_ActionExecute_CheckResultType(bool isIDValid)
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
    }
}




