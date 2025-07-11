namespace GGroupp.Yandex.BulkUpdate;

public sealed record class BulkUpdateOut
{
    public required bool IsSuccess { get; init; }

    public required int StatusCode { get; init; }

    public object? Body { get; init; }
}