using System;

namespace PedrosCantina.Library.Repositories;

public interface ICrudOperations<T, in TKey>
{
	T? ReadById(TKey id);
	List<T> ReadAll();
	T Create(T entity);
	void Update(T entity);
	void Delete(TKey id);
}
