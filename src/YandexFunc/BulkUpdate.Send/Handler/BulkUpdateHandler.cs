using System;
using System.Collections.Generic;
using System.Text.Json;
using GarageGroup;
using GarageGroup.Infra;

namespace GGroupp.Yandex.BulkUpdate;

internal sealed partial class BulkUpdateHandler(IHttpApi httpApi) : IBulkUpdateHandler
{
    private const string OrganizationId = "bpfc8bo9bqkku0jkif8q";

    private static readonly Uri YandexTrackerApiSearchIssuesPostUri
        =
        new("https://api.tracker.yandex.net/v3/issues/_search");

    private static readonly string YandexTrackerApiIssuesPostUri
        =
        "https://api.tracker.yandex.net/v3/issues/";

    private static readonly PipelineParallelOption ParallelOption
        =
        new()
        {
            DegreeOfParallelism = 4,
            FailureAction = PipelineParallelFailureAction.Stop
        };

    private static Result<BulkUpdateIn, Failure<HandlerFailureCode>> Validate(BulkUpdateIn? input)
    {
        if (string.IsNullOrWhiteSpace(input?.Query))
        {
            return Failure.Create(HandlerFailureCode.Persistent, "Input Query must be specified.");
        }

        if (input.Values is null)
        {
            return Failure.Create(HandlerFailureCode.Persistent, "Input Values must be specified.");
        }

        return Result.Success(input);
    }

    private static object? DeserializeBody(HttpBody body)
    {
        if (body.Content is null)
        {
            return null;
        }

        if (body.Type.IsJsonMediaType(false))
        {
            return body.DeserializeFromJson<JsonElement?>();
        }

        return body.Content.ToString();
    }

    private static FlatArray<KeyValuePair<string, string>> BuildHeader(string organizationId)
        =>
        [new("X-Cloud-Org-ID", organizationId)];

    private sealed record class Issue
    {
        public string? Id { get; init; }
    }

    public sealed record class UpdateResult
    {
        public required string IssueId { get; init; }

        public required bool IsSuccess { get; init; }

        public object? Body { get; init; }
    }
}