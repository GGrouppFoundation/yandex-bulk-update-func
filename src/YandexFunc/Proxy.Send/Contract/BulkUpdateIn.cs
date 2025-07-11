using System.Collections.Generic;

namespace GGroupp.Yandex.BulkUpdate;

public sealed record class BulkUpdateIn
{
    public string? Query { get; init; }

    public Dictionary<string, string>? Values { get; init; }
}