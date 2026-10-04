namespace PedrosCantina.Library.Repositories;

/// <summary>
/// Defines the basic write operations for a given type.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
/// <typeparam name="TKey">The type of the key.</typeparam>
public interface IWriteOperations<T, in TKey>
{
	T Create(T entity);
	void Update(T entity);
	void Delete(TKey id);
}
