using Api.Common.Attributes;
using Api.Utils;
using System.Linq.Dynamic.Core;

namespace ApiTests.Sorting;

public class Car
{
    public string Make { get; set; } = string.Empty;

    [SortName("ModelName")]
    public string ModelName { get; set; } = string.Empty;

    [NotSortable]
    public decimal Mileage { get; set; }
}

[TestClass]
public class SortStatementGeneratorTests
{
    [TestMethod]
    public void AnExceptionShouldBeThrownIfTheSortStatementIsEmpty()
    {
        Assert.ThrowsException<InvalidSortStatementException>(
            () => SortStatementGenerator.GenerateSortStatement("", typeof(Car))
        );
    }

    [TestMethod]
    public void AnExceptionShouldBeThrownIfThePropertyIsInvalid()
    {
        Assert.ThrowsException<InvalidSortStatementException>(
            () => SortStatementGenerator.GenerateSortStatement("Xyz", typeof(Car))
        );
    }

    [TestMethod]
    public void AnExceptionShouldBeThrownIfThePropertyIsNotSortable()
    {
        Assert.ThrowsException<InvalidSortStatementException>(
            () => SortStatementGenerator.GenerateSortStatement("Mileage", typeof(Car))
        );
    }

    [TestMethod]
    public void SortingShouldWorkCorrectly()
    {
        var cars = new List<Car>
        {
            new Car { Make = "Honda", ModelName = "Civic", },
            new Car { Make = "Mazda", ModelName = "CX5", },
            new Car { Make = "Mazda", ModelName = "CX7", },
        }.AsQueryable();

        string sortStatement = SortStatementGenerator.GenerateSortStatement("make asc, modelName desc", typeof(Car));

        List<Car> sortedCars = cars.OrderBy(sortStatement).ToList();
        Assert.IsTrue(sortedCars[0].Make == "Honda");
        Assert.IsTrue(sortedCars[2].Make == "Mazda" && sortedCars[2].ModelName == "CX5");
    }
}
