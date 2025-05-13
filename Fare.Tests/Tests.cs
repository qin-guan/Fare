using Fare.Core;
using Fare.Tests.Data;

namespace Fare.Tests;

public class Tests
{
    [Test]
    [DataGenerator]
    public async Task CustomDataGenerator(string data)
    {
        var output = Parser.Parse(data);
        var dist = Utilities.GetDistance(
            output,
            n => n.Name == "TE8",
            n => n.Name == "Bishan"
        );
    }
}