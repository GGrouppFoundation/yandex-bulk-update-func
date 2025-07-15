using System.Collections.Generic;

namespace GGroupp.Yandex.IssuesUpdate;

public sealed record class IssuesUpdateIn
{
    public string? Query { get; init; }

    public Dictionary<string, string>? Values { get; init; }
}