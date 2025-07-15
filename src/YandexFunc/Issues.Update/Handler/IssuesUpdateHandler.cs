using System;
using System.Collections.Generic;
using GarageGroup;
using GarageGroup.Infra;

namespace GGroupp.Yandex.IssuesUpdate;

internal sealed partial class IssuesUpdateHandler(IHttpApi httpApi, IssuesUpdateOption option) : IIssuesUpdateHandler
{
    private const string YandexTrackerApiSearchIssuesPostUri
        =
        "https://api.tracker.yandex.net/v3/issues/_search";

    private const string YandexTrackerApiIssuesPostUriTemplate
        =
        "https://api.tracker.yandex.net/v3/issues/{0}";

    private static readonly PipelineParallelOption ParallelOption
        =
        new()
        {
            DegreeOfParallelism = 4,
            FailureAction = PipelineParallelFailureAction.Stop
        };

    private readonly FlatArray<KeyValuePair<string, string>> Headers
        =
        [new("X-Cloud-Org-ID", option.OrganizationId)];

    private static Result<IssuesUpdateIn, Failure<HandlerFailureCode>> Validate(IssuesUpdateIn? input)
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

    private sealed record class IssueJson
    {
        public string? Id { get; init; }
    }

    private static HandlerFailureCode MapFailureCode(HttpFailureCode failureCode)
        =>
        failureCode switch
        {
            HttpFailureCode.Conflict or HttpFailureCode.InternalServerError => HandlerFailureCode.Transient,
            _ => HandlerFailureCode.Persistent
        };
}