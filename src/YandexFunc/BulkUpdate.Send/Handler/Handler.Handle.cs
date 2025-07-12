using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup;
using GarageGroup.Infra;

namespace GGroupp.Yandex.BulkUpdate;

partial class BulkUpdateHandler
{
    public ValueTask<Result<BulkUpdateOut, Failure<HandlerFailureCode>>> HandleAsync(
        BulkUpdateIn? input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input, cancellationToken)
        .Pipe(
            Validate)
        .ForwardValue(
            InnerHandleAsync);

    private ValueTask<Result<BulkUpdateOut, Failure<HandlerFailureCode>>> InnerHandleAsync(
        BulkUpdateIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input, cancellationToken)
        .Pipe(
            static @in => new HttpSendIn(
                method: HttpVerb.Post,
                requestUri: YandexTrackerApiSearchIssuesPostUri.ToString())
            {
                Headers = BuildHeader(OrganizationId),
                Body = HttpBody.SerializeAsJson(new { @in.Query })
            })
        .PipeValue(
            httpApi.SendAsync)
        .MapFailure(
            static failure => failure.ToStandardFailure("Failed to query issues:").WithFailureCode(HandlerFailureCode.Persistent))
        .MapSuccess(
            static success => success.Body.DeserializeFromJson<FlatArray<Issue>>())
        .ForwardParallelValue(
            (issue, token) => UpdateIssueAsync(issue, input, token),
            ParallelOption)
        .MapSuccess(
            static success => new BulkUpdateOut
            {
                Successes = success.Filter(item => item.IsSuccess is true).Length
            });

    private ValueTask<Result<UpdateResult, Failure<HandlerFailureCode>>> UpdateIssueAsync(
        Issue issue, BulkUpdateIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            issue, cancellationToken)
        .Pipe(
            issue => new HttpSendIn(
                method: HttpVerb.Patch,
                requestUri: YandexTrackerApiIssuesPostUri.ToString() + issue.Id.OrEmpty())
            {
                Headers = BuildHeader(OrganizationId),
                Body = HttpBody.SerializeAsJson(input.Values)
            })
        .PipeValue(
            httpApi.SendAsync)
        .MapSuccess(
            success => new UpdateResult
            {
                IssueId = issue.Id.OrEmpty(),
                IsSuccess = true
            })
        .MapFailure(
            static failure => failure.ToStandardFailure("Failed to update issue:").WithFailureCode(HandlerFailureCode.Persistent));
}