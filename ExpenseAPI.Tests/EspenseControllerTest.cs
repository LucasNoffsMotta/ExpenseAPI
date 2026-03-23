using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using System.Net;
using System.Security.Cryptography;
using UnitTests_ExpenseAPI;
using UnitTests_ExpenseAPI.DTO.ExpensesDTO;
using UnitTests_ExpenseAPI.Services.Expense;

namespace ExpenseAPI.Tests;

public class EspenseControllerTest
{
    private Mock<IExpenseService> _expenseServiceMock;


    public EspenseControllerTest()
    {
        _expenseServiceMock = new Mock<IExpenseService>();
    }

    [Fact]
    public async Task GetAll_ActionExecutes_CheckResultType_ReturnExpensesDTO()
    {
        // 1 .Arrange
        List<SummaryExpenseDTO> models = new List<SummaryExpenseDTO>()
        {
            new SummaryExpenseDTO
            (
                1,
                "Ifood",
                10.0m,
                DateOnly.MaxValue
            ),

            new SummaryExpenseDTO
            (
                2,
                "Ifood",
                10.0m,
                DateOnly.MaxValue
            )
        };

        //Return type of Expenses Service
        OkObjectResult baseResponse = new OkObjectResult(models);
        _expenseServiceMock.Setup(um => um.GetAll()).ReturnsAsync(models.ToList);

        //Controller
        ExpensesController controller = new ExpensesController(_expenseServiceMock.Object);

        // 2 .Act
        var expensesListResponse = await controller.GetAll();

        //3. Assert

        //Test response type
        var okResult = Assert.IsType<OkObjectResult>(expensesListResponse);
        Assert.IsType<List<SummaryExpenseDTO>>(okResult.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GetById_ActionExecutes_CheckResultType_ReturnSingleObject(int id)
    {
        // 1 .Arrange
        List<Expense> models = new List<Expense>()
        {
            new Expense
            {
             ID = 0,
             Value=10.0m,
             Date = DateOnly.MaxValue
            },

            new Expense
            {
             ID = 1,
             Value=90.0m,
             Date = DateOnly.MaxValue
            },
        };

        var expense = models.FirstOrDefault(m => m.ID == id);


        _expenseServiceMock.Setup(x => x.GetById(id))
            .ReturnsAsync(expense == null ? null : ExpenseMappings.ExpenseModelToSummaryDTO(expense));


        // 2 .Act
        var controller = new ExpensesController(_expenseServiceMock.Object);
        var result = await controller.GetByID(id);


        // 3 .Assert
        if (id < models.Count)
        {
            var okResult = Assert.IsType<OkObjectResult>(result);
            var item = Assert.IsType<SummaryExpenseDTO>(okResult.Value);
            Assert.Equal((models.Where(m => m.ID == id).First()).Value, item.Value);
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

        //Valid Model
        var createDTO = new CreateExpenseDTO(1, 10.0m, DateOnly.MaxValue);
        var model = ExpenseMappings.ExpenseDtoToModel(createDTO);

        //Service
        _expenseServiceMock.Setup(s => s.Create(createDTO)).ReturnsAsync(true);


        // 2. Act
        var controller = new ExpensesController(_expenseServiceMock.Object);
        var response = await controller.Create(createDTO);

        // 3. Assert
        var result = Assert.IsType<OkObjectResult>(response);
        var created = Assert.IsType<CreateExpenseDTO>(result.Value);
    }


    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Delete_ActionExecute_CheckResultType(bool isIDValid)
    {

        //Arrange
        int id = 1;
        _expenseServiceMock.Setup(s=> s.DeleteByID(id)).ReturnsAsync(isIDValid);


        //Act
        var controller = new ExpensesController(_expenseServiceMock.Object);
        var response = await controller.Delete(id);

        //Assert
        if (isIDValid) Assert.IsType<OkResult>(response);

        else
        {
            Assert.IsType<BadRequestResult>(response);
        }
    }
}




