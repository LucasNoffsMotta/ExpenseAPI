using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using System.Net;
using System.Security.Cryptography;
using UnitTests_ExpenseAPI;

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
                10.0m,
                DateOnly.MaxValue
            ),

            new SummaryExpenseDTO
            (
                20.0m,
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
}
