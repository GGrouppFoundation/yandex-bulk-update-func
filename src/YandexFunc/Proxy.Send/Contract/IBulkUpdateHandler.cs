using GarageGroup.Infra;

namespace GGroupp.Yandex.BulkUpdate;

public interface IBulkUpdateHandler : IHandler<BulkUpdateIn, BulkUpdateOut>;