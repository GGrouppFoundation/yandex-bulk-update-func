using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
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
        .MapSuccess(
            BulkUpdateAsync);

    private Task<BulkUpdateOut> BulkUpdateAsync(
        BulkUpdateIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input, cancellationToken)
        .Pipe(
            static @in => new HttpSendIn(
                method: HttpVerb.Post,
                requestUri: "change")
            {
                Body = HttpBody.SerializeAsJson(@in.Query)
            })
        .PipeValue(
            httpApi.SendAsync)
        .Fold(
            static success => new BulkUpdateOut
            {
                IsSuccess = true,
                StatusCode = BaseSuccessStatusCode + (int)success.StatusCode,
                Body = success.Body.DeserializeFromJson<JsonElement?>()
            },
            static failure => new BulkUpdateOut
            {
                IsSuccess = false,
                StatusCode = (int)failure.StatusCode,
                Body = DeserializeBody(failure.Body)
            });
}