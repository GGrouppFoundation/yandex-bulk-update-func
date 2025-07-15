using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup;
using GarageGroup.Infra;

namespace GGroupp.Yandex.IssuesUpdate;

partial class IssuesUpdateHandler
{
    public ValueTask<Result<IssuesUpdateOut, Failure<HandlerFailureCode>>> HandleAsync(
        IssuesUpdateIn? input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input, cancellationToken)
        .Pipe(
            Validate)
        .ForwardValue(
            InnerHandleAsync);

    private ValueTask<Result<IssuesUpdateOut, Failure<HandlerFailureCode>>> InnerHandleAsync(
        IssuesUpdateIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input, cancellationToken)
        .Pipe(
            @in => new HttpSendIn(
                method: HttpVerb.Post,
                requestUri: YandexTrackerApiSearchIssuesPostUri)
            {
                Headers = Headers,
                Body = HttpBody.SerializeAsJson(new { @in.Query })
            })
        .PipeValue(
            httpApi.SendAsync)
        .Map(
            static success => success.Body.DeserializeFromJson<FlatArray<IssueJson>>(),
            static failure => failure.ToStandardFailure("Failed to query issue:").MapFailureCode(MapFailureCode))
        .ForwardValue(
            (issues, token) => UpdateIssuesAsync(issues, input, token));

    private ValueTask<Result<IssuesUpdateOut, Failure<HandlerFailureCode>>> UpdateIssuesAsync(
        FlatArray<IssueJson> issues, IssuesUpdateIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            issues, cancellationToken)
        .PipeParallelValue(
            (issue, token) => UpdateIssueAsync(issue, input, token),
            ParallelOption)
        .MapSuccess(
            _ => new IssuesUpdateOut
            {
                Successes = issues.Length
            });

    private ValueTask<Result<Unit, Failure<HandlerFailureCode>>> UpdateIssueAsync(
        IssueJson issue, IssuesUpdateIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            issue, cancellationToken)
        .Pipe(
            issue => new HttpSendIn(
                method: HttpVerb.Patch,
                requestUri: string.Format(YandexTrackerApiIssuesPostUriTemplate, issue.Id))
            {
                Headers = Headers,
                Body = HttpBody.SerializeAsJson(input.Values)
            })
        .PipeValue(
            httpApi.SendAsync)
        .Map(
            Unit.From,
            static failure => failure.ToStandardFailure("Failed to update issue:").MapFailureCode(MapFailureCode));
}