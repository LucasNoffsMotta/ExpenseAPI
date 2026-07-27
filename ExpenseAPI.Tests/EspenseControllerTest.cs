using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using UnitTests_ExpenseAPI;
using UnitTests_ExpenseAPI.DTO.ExpensesDTO;
using UnitTests_ExpenseAPI.Repo;

namespace ExpenseAPI.Tests;

public class EspenseControllerTest
{
    private Mock<IBaseRepo<Expense>> _expenseServiceMock;


    public EspenseControllerTest()
    {
        _expenseServiceMock = new Mock<IBaseRepo<Expense>>();
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
                DateOnly.MaxValue,
                "#FFFFFF"
            ),

            new SummaryExpenseDTO
            (
                2,
                "Ifood",
                10.0m,
                DateOnly.MaxValue,
                "#FFFFFF"
            )
        };

        //Return type of Expenses Service
        OkObjectResult baseResponse = new OkObjectResult(models);
        _expenseServiceMock.Setup(um => um.GetAll(null)).Returns(models.ToList);

        ILogger<ExpensesController> dummyLogger = NullLogger<ExpensesController>.Instance;

        //Controller
        ExpensesController controller = new ExpensesController(_expenseServiceMock.Object, dummyLogger);

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

        _expenseServiceMock.Setup(x => x.GetByID(id))
            .Returns(Task.FromResult(expense));


        // 2 .Act
        ILogger<ExpensesController> dummyLogger = NullLogger<ExpensesController>.Instance;
        var controller = new ExpensesController(_expenseServiceMock.Object, dummyLogger);
        var result = await controller.GetByID(id);


        // 3 .Assert
        if (id < models.Count)
        {
            var okResult = Assert.IsType<OkObjectResult>(result);
            var item = Assert.IsType<SummaryExpenseDTO>(okResult.Value);
            Assert.Equal((models.Where(m => m.ID == id).First()).Value, item.Valor);
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
        _expenseServiceMock.Setup(s => s.Create(ExpenseMappings.ExpenseDtoToModel(createDTO))).ReturnsAsync(true);


        // 2. Act
        ILogger<ExpensesController> dummyLogger = NullLogger<ExpensesController>.Instance;
        var controller = new ExpensesController(_expenseServiceMock.Object, dummyLogger);
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
        _expenseServiceMock.Setup(s=> s.Delete(id)).ReturnsAsync(isIDValid);


        //Act
        ILogger<ExpensesController> dummyLogger = NullLogger<ExpensesController>.Instance;
        var controller = new ExpensesController(_expenseServiceMock.Object, dummyLogger);
        var response = await controller.Delete(id);

        //Assert
        if (isIDValid) Assert.IsType<OkResult>(response);

        else
        {
            Assert.IsType<BadRequestResult>(response);
        }
    }
}




