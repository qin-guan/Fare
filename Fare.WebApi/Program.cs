using System.Collections.Frozen;
using System.ComponentModel;
using System.Text.Json.Serialization;
using Fare.Core;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

var stnData = Environment.GetEnvironmentVariable("STN_DATA");

if (stnData is null)
    ArgumentNullException.ThrowIfNull(stnData);

var stns = Parser.Parse(stnData);
var all = Utilities.GetAllDistances(stns);

app.MapOpenApi();
app.MapScalarApiReference("/");

app.MapGet("/All", [EndpointSummary("Returns all station permutations")]() => TypedResults.Ok(all));
app.MapGet("/Distance", (
    [FromQuery] [Description("Station Name (example: 'Tanjong Pagar')")]
    string from,
    [FromQuery] [Description("Station Name (example: 'Bishan')")]
    string to
) =>
{
    if (!all.TryGetValue($"[{from}][{to}]", out var distance))
    {
        throw new Exception("Invalid station name.");
    }

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

[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(decimal))]
[JsonSerializable(typeof(Response))]
[JsonSerializable(typeof(FrozenDictionary<string, decimal>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext;