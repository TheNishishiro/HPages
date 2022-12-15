
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

public class GenericRepository<T> where T : class
{
	// Generic repository for reading and writing to the database using entity framework core.
	private readonly DbContext _context;
	private readonly DbSet<T> _dbSet;
	public GenericRepository(DbContext context)
	{
		_context = context;
		_dbSet = _context.Set<T>();
	}
	public void Add(T entity)
	{
		_dbSet.Add(entity);
	}
	public void Delete(T entity)
	{
		_dbSet.Remove(entity);
	}
	public void Delete(int id)
	{
		var entity = _dbSet.Find(id);
		_dbSet.Remove(entity);
	}
	public void Delete(Expression<Func<T, bool>> predicate)
	{
		var entities = _dbSet.Where(predicate);
		_dbSet.RemoveRange(entities);
	}
	public void Edit(T entity)
	{
		_dbSet.Update(entity);
	}
	public IEnumerable<T> GetAll()
	{
		return _dbSet.ToList();
	}
	public T GetById(int id)
	{
		return _dbSet.Find(id);
	}
	public IEnumerable<T> GetMany(Expression<Func<T, bool>> predicate)
	{
		return _dbSet.Where(predicate).ToList();
	}
	public IEnumerable<T> GetMany(Expression<Func<T, bool>> predicate, string include)
	{
		return _dbSet.Where(predicate).Include(include).ToList();
	}
	public IEnumerable<T> GetMany(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
	{
		var query = _dbSet.Where(predicate);
		foreach (var include in includes)
		{
			query = query.Include(include);
		}
		return query.ToList();
	}
}