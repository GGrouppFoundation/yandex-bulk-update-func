using System;
using System.Text.Json;
using GarageGroup.Infra;

namespace GGroupp.Yandex.BulkUpdate;

internal sealed partial class BulkUpdateHandler(IHttpApi httpApi) : IBulkUpdateHandler
{
    private const int BaseSuccessStatusCode = 200;

    private static Result<BulkUpdateIn, Failure<HandlerFailureCode>> Validate(BulkUpdateIn? input)
    {
        if (string.IsNullOrWhiteSpace(input?.Query))
        {
            return Failure.Create(HandlerFailureCode.Persistent, "Input Query must be specified.");
        }

        if (input?.Values is null)
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
}