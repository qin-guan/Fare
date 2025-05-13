using System.Text.Json.Serialization;
using Fare.Core;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();
var stnData = Environment.GetEnvironmentVariable("STN_DATA");

if (stnData is null)
    ArgumentNullException.ThrowIfNull(stnData);

var stns = Parser.Parse(stnData);

app.MapGet("/Distance", ([FromQuery] string from, [FromQuery] string to) =>
{
    var distance = Utilities.GetDistance(stns, s => s.Name == from, s => s.Name == to);
    return TypedResults.Ok(
        new Response(
            distance,
            Constants.StudentFare(distance),
            Constants.SeniorFare(distance),
            Constants.AdultFare(distance)
        )
    );
});

app.Run();

public record Response(decimal Distance, decimal Student, decimal Senior, decimal Adult);

[JsonSerializable(typeof(Response))]
internal partial class AppJsonSerializerContext : JsonSerializerContext;