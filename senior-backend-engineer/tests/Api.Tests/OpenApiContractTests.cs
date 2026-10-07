using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Api.Tests;

/// <summary>Pins the documented contract: every endpoint declares its success type and each problem-details status.</summary>
public sealed class OpenApiContractTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private const string Problem = "ProblemDetails";

    [Theory]
    [InlineData("/users", "post", "201:CreateUserResponse", "400:" + Problem, "409:" + Problem)]
    [InlineData("/users/{id}", "get", "200:GetUserByIdResponse", "400:" + Problem, "404:" + Problem)]
    [InlineData("/work-items", "post", "201:CreateWorkItemResponse", "400:" + Problem, "422:" + Problem)]
    [InlineData("/work-items", "get", "200:array", "400:" + Problem)]
    public async Task Endpoint_documents_exactly_its_responses(string path, string method, params string[] expected)
    {
        using var doc = JsonDocument.Parse(await factory.CreateClient().GetStringAsync("/swagger/v1/swagger.json"));

        var responses = doc.RootElement.GetProperty("paths").GetProperty(path).GetProperty(method).GetProperty("responses");
        var documented = responses.EnumerateObject().Select(r => $"{r.Name}:{SchemaName(r.Value)}");

        documented.Should().BeEquivalentTo(expected);
    }

    private static string SchemaName(JsonElement response)
    {
        var schema = response.GetProperty("content").EnumerateObject().First().Value.GetProperty("schema");
        return schema.TryGetProperty("$ref", out var reference)
            ? reference.GetString()!.Split('/').Last()
            : schema.GetProperty("type").GetString()!;
    }
}
