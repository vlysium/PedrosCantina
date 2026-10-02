using System;

namespace PedrosCantina.Library.Repositories;

public interface ICrudOperations<T>
{
	T? ReadById(int id);
	List<T> ReadAll();
	T Create(T entity);
	T Update(T entity);
	T Delete(int id);
}
