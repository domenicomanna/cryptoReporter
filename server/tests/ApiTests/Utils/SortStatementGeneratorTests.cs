using System.Linq.Dynamic.Core;
using Api.Common.Attributes;
using Api.Utils;

namespace ApiTests.Sorting;

public class Car
{
    public string Make { get; set; } = string.Empty;

    [SortName("ModelName")]
    public string ModelName { get; set; } = string.Empty;

    [NotSortable]
    public decimal Mileage { get; set; }
}

public class SortStatementGeneratorTests
{
    [Fact]
    public void AnExceptionShouldBeThrownIfTheSortStatementIsEmpty()
    {
        Assert.Throws<InvalidSortStatementException>(() =>
            SortStatementGenerator.GenerateSortStatement("", typeof(Car))
        );
    }

    [Fact]
    public void AnExceptionShouldBeThrownIfThePropertyIsInvalid()
    {
        Assert.Throws<InvalidSortStatementException>(() =>
            SortStatementGenerator.GenerateSortStatement("Xyz", typeof(Car))
        );
    }

    [Fact]
    public void AnExceptionShouldBeThrownIfThePropertyIsNotSortable()
    {
        Assert.Throws<InvalidSortStatementException>(() =>
            SortStatementGenerator.GenerateSortStatement("Mileage", typeof(Car))
        );
    }

    [Fact]
    public void SortingShouldWorkCorrectly()
    {
        var cars = new List<Car>
        {
            new Car { Make = "Honda", ModelName = "Civic" },
            new Car { Make = "Mazda", ModelName = "CX5" },
            new Car { Make = "Mazda", ModelName = "CX7" },
        }.AsQueryable();

        string sortStatement = SortStatementGenerator.GenerateSortStatement("make asc, modelName desc", typeof(Car));

        List<Car> sortedCars = cars.OrderBy(sortStatement).ToList();
        Assert.True(sortedCars[0].Make == "Honda");
        Assert.True(sortedCars[2].Make == "Mazda" && sortedCars[2].ModelName == "CX5");
    }
}
