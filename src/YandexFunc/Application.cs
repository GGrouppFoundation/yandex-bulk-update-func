using GarageGroup.Infra;
using GGroupp.Infra;
using PrimeFuncPack;

namespace GGroupp.Yandex.IssuesUpdate;

public static class Application
{
    [YandexHttpFuncton("IssuesUpdateFunction")]
    public static Dependency<IIssuesUpdateHandler> UseIssuesUpdateHandler()
        =>
        PrimaryHandler.UseStandardSocketsHttpHandler()
        .UseLogging("IssuesUpdateApi")
        .UseYandexIamToken("Yandex")
        .UsePollyStandard()
        .UseHttpApi()
        .UseIssuesUpdateHandler();
}