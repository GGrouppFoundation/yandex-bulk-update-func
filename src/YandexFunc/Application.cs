using GarageGroup.Infra;
using GGroupp.Infra;
using PrimeFuncPack;

namespace GGroupp.Yandex.BulkUpdate;

public static class Application
{
    [YandexHttpFuncton("BulkUpdateFunction")]
    public static Dependency<IBulkUpdateHandler> UseBulkUpdateHandler()
        =>
        PrimaryHandler.UseStandardSocketsHttpHandler()
        .UseLogging("BulkUpdateApi")
        .UseYandexIamToken("Yandex")
        .UsePollyStandard()
        .UseHttpApi()
        .UseBulkUpdateHandler();
}