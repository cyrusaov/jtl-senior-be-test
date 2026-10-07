using System.Net;
using System.Net.Http.Json;

namespace Api.Tests;

/// <summary>Asserts the agreed error contract: RFC 7807 problem details carrying a stable error code.</summary>
internal static class ProblemDetailsAssertions
{
    public static async Task ShouldBeProblemAsync(
        this HttpResponseMessage response, HttpStatusCode status, string field, string? code = null)
    {
        response.StatusCode.Should().Be(status);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");

        var problem = await response.Content.ReadFromJsonAsync<Problem>();
        problem!.Status.Should().Be((int)status);
        problem.Errors.Should().ContainSingle(e => e.Name == field && (code == null || e.Code == code));
    }

    private sealed record Problem(int Status, IReadOnlyList<ProblemError> Errors);

    private sealed record ProblemError(string Name, string Reason, string? Code);
}
