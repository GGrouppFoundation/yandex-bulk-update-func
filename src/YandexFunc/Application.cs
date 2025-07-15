using System;
using GarageGroup.Infra;
using GGroupp.Infra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        .With(ResolveIssuesUpdateOption)
        .UseIssuesUpdateHandler();

    private static IssuesUpdateOption ResolveIssuesUpdateOption(IServiceProvider serviceProvider)
    {
        var organizationId = serviceProvider.GetRequiredService<IConfiguration>()["OrganizationId"];

        if (string.IsNullOrWhiteSpace(organizationId))
        {
            throw new InvalidOperationException("OrganizationId configuration parameter must be specified.");
        }

        return new()
        {
            OrganizationId = organizationId
        };
    }
}