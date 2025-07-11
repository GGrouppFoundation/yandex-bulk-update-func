using GarageGroup.Infra;
using GGroupp.Infra;
using PrimeFuncPack;

namespace GGroupp.Yandex.BulkUpdate;

public static class Application
{
    [YandexHttpFuncton("BulkUpdateFunc")]
    public static Dependency<IBulkUpdateHandler> UseBulkUpdateHandler()
        =>
        PrimaryHandler.UseStandardSocketsHttpHandler()
        .UseLogging("BulkUpdateApi")
        .UseYandexIamToken("YANDEX_OAUTH_TOKEN")
        .UsePollyStandard()
        .UseHttpApi()
        .UseBulkUpdateHandler();
}