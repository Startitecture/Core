Example Usage
===

Evalute with Expressions
---

In this example, we use `Evaluate.Compare()` to compare specific values of two different objects:

```` csharp
var x = new ComparisonClass { Compared = 10, AlsoCompared = "Compare1", NotCompared = 239 };
var y = new ComparisonClass { Compared = 11, AlsoCompared = "Compare1", NotCompared = 94353 };
var actual = Evaluate.Compare(x, y, c => c.Compared, c => c.AlsoCompared);
````
The same can be done for equality:

```` csharp
var actual = Evaluate.Equals(x, y, c => c.Compared, c => c.AlsoCompared);
````

For enumerables of objects, `Evaluate.RecursiveEquals()` will walk the object graph of each item:

```` csharp
var valueA = new FakeTestItem { TestString = "test", TestInt = 1, TestDateTime = DateTime.MinValue };
valueA.AddItem("Test1");
valueA.AddItem("Test2");
valueA.AddItem("Test3");

var valueB = new FakeTestItem { TestString = "test", TestInt = 1, TestDateTime = DateTime.MinValue };
valueB.AddItem("Test1");
valueB.AddItem("Test2");
valueB.AddItem("Test3");

var actual = Evaluate.RecursiveEquals(valueA, valueB);
````

For this to work, `FakeTestItem` must implement `IEquatable<T>` and call `Evalute.Equals()` with expression parameters:
```` csharp
public class FakeTestItem : IEquatable<FakeTestItem>
{
    private static readonly Func<FakeTestItem, object>[] ComparisonProperties =
        {
            item => item.TestInt,
            item => item.TestString,
            item => item.TestDateTime,
            item => item.testList
        };

    private readonly List<string> testList = new List<string>();

    public string TestString { get; set; }

    public int TestInt { get; set; }

    public DateTime TestDateTime { get; set; }

    public IEnumerable<string> TestList => this.testList;

    public static bool operator ==(FakeTestItem valueA, FakeTestItem valueB)
    {
        return EqualityComparer<FakeTestItem>.Default.Equals(valueA, valueB);
    }

    public static bool operator !=(FakeTestItem valueA, FakeTestItem valueB)
    {
        return !(valueA == valueB);
    }

    public override string ToString()
    {
        return $"{this.TestInt}:{this.TestString}:{this.TestDateTime}:{string.Join(";", this.testList)}";
    }

    public override int GetHashCode()
    {
        return Evaluate.GenerateHashCode(this, ComparisonProperties);
    }

    public override bool Equals(object obj)
    {
        return Evaluate.Equals(this, obj);
    }

    public bool Equals(FakeTestItem other)
    {
        return Evaluate.Equals(this, other, ComparisonProperties);
    }
}
````