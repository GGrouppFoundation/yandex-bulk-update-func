using System;
using GarageGroup.Infra;
using PrimeFuncPack;

namespace GGroupp.Yandex.BulkUpdate;

public static class BulkUpdateDependency
{
    public static Dependency<IBulkUpdateHandler> UseBulkUpdateHandler(this Dependency<IHttpApi> dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Map<IBulkUpdateHandler>(CreateHandler);

        static BulkUpdateHandler CreateHandler(IHttpApi httpApi)
        {
            ArgumentNullException.ThrowIfNull(httpApi);
            return new(httpApi);
        }
    }
}