using System;

namespace GGroupp.Yandex.BulkUpdate;

public sealed record class YandexIamTokenOption
{
    public required string PassportOauthToken { get; init; }

    public required TimeSpan ExpirationDelta { get; init; }
}