using System;
using System.Text.Json;
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
            static failure => Failure.Create(HandlerFailureCode.Persistent, failure.Body.ToString()))
        .MapSuccess(
            @in => DeserializeBodyAsArray(@in.Body))
        .ForwardParallelValue(
            (issue, token) => UpdateIssueAsync(issue, input, token),
            ParallelOption)
        .MapSuccess(
            static success => new BulkUpdateOut
            {
                IsSuccess = true,
                Successes = success.Filter(item => item.IsSuccess is true).Length
            });

    private ValueTask<Result<UpdateResult, Failure<HandlerFailureCode>>> UpdateIssueAsync(
        Issue issue, BulkUpdateIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            issue, cancellationToken)
        .Pipe(
            @in => new HttpSendIn(
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
        .Recover(
            failure => failure.StatusCode switch
            {
                HttpFailureCode.BadRequest => new UpdateResult
                {
                    IssueId = issue.Id.OrEmpty(),
                    IsSuccess = false,
                    Body = DeserializeBody(failure.Body)
                },
                _ => failure
            })
        .MapFailure(
            static failure => Failure.Create(HandlerFailureCode.Transient, "Internal service error"));
}