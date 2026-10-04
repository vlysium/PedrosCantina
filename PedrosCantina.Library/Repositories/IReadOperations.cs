using System;

namespace PedrosCantina.Library.Repositories;

/// <summary>
/// Defines the basic read operations for a given type.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
/// <typeparam name="TKey">The type of the key.</typeparam>
public interface IReadOperations<T, in TKey>
{
	T? ReadById(TKey id);
	List<T> ReadAll();
}
