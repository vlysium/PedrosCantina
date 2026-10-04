using System;

namespace PedrosCantina.Library.Repositories;

public interface ICrudOperations<T, in TKey> : IReadOperations<T, TKey>, IWriteOperations<T, TKey>
{
}
