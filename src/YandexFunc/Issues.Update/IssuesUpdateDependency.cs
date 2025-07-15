using System;
using GarageGroup.Infra;
using PrimeFuncPack;

namespace GGroupp.Yandex.IssuesUpdate;

public static class IssuesUpdateDependency
{
    public static Dependency<IIssuesUpdateHandler> UseIssuesUpdateHandler(this Dependency<IHttpApi> dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Map<IIssuesUpdateHandler>(CreateHandler);

        static IssuesUpdateHandler CreateHandler(IHttpApi httpApi)
        {
            ArgumentNullException.ThrowIfNull(httpApi);
            return new(httpApi);
        }
    }
}