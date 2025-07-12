namespace GGroupp.Yandex.BulkUpdate;

public sealed record class BulkUpdateOut
{
    public required bool IsSuccess { get; init; }

    public int Successes { get; init; }
}