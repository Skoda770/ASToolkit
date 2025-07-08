using ASToolkit.Storage.Enums;
using ASToolkit.Storage.Interfaces;

namespace ASToolkit.Storage.Infrastructure;

public class StorageFactory(IEnumerable<IStorage> storages)
{
    public IStorage GetStorage(StorageType type)
        => storages.FirstOrDefault(storage => storage.Type == type)
           ?? throw new ArgumentException($"Invalid storage type: {type}", nameof(type));
}