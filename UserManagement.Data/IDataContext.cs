using System;
using System.Linq;
using System.Linq.Expressions;

namespace UserManagement.Data;

public interface IDataContext
{
    /// <summary>
    /// Detaches a current entry to avoid double tracking by accident
    /// </summary>
    /// <param name="entry">The entry to be detached</param>
    public void DetachEntry(object entry);

    /// <summary>
    /// Get a list of items
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    IQueryable<TEntity> GetAll<TEntity>(params Expression<Func<TEntity, object>>[] includes) where TEntity : class;

    /// <summary>
    /// Create a new item
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    void Create<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Uodate an existing item matching the ID
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    void Update<TEntity>(TEntity entity) where TEntity : class;

    void Delete<TEntity>(TEntity entity) where TEntity : class;
}
