using System;
using GarageGroup.Infra;
using PrimeFuncPack;

namespace GGroupp.Yandex.IssuesUpdate;

public static class IssuesUpdateDependency
{
    public static Dependency<IIssuesUpdateHandler> UseIssuesUpdateHandler(this Dependency<IHttpApi, IssuesUpdateOption> dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Fold<IIssuesUpdateHandler>(CreateHandler);

        static IssuesUpdateHandler CreateHandler(IHttpApi httpApi, IssuesUpdateOption option)
        {
            ArgumentNullException.ThrowIfNull(httpApi);
            ArgumentNullException.ThrowIfNull(option);
            return new(httpApi, option);
        }
    }
}