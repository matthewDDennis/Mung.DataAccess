using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Munq.DataAccess.Shared
{
    /// <summary>
    /// An in memory Repository, generally used for testing.
    /// </summary>
    /// <typeparam name="TRepository">The type of the Repository to implement.</typeparam>
    /// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
    /// <typeparam name="TKey">The type of the key property in TEntity.</typeparam>
    public abstract class MemoryRepository<TRepository, TEntity, TKey> : IRepositoryExtended<TEntity, TKey>
        where TRepository : IRepository<TEntity, TKey>
        where TEntity     : class
        where TKey        : struct, IEquatable<TKey>
    {
        Dictionary<TKey, TEntity> data;

        IKeyAccessor<TEntity, TKey> _keyAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryRepository{TRepository, TEntity, TKey}"/> class.
        /// </summary>
        public MemoryRepository()
        {
            data         = new Dictionary<TKey, TEntity>();
            _keyAccessor = KeyAccessorFactory.Create<TEntity, TKey>();
        }

        /// <inheritdoc/>
        public virtual Task<IEnumerable<TEntity>> GetAll(int? skip = null, int? take = null)
        {
            return Get(skip: skip, take: take);
        }

        /// <inheritdoc/>
        public virtual async Task<TEntity> GetByID(TKey id)
        {
            return await Task.Run(() =>
            {
                if (data.TryGetValue(id, out TEntity existing))
                {
                    return existing;
                }

                return null;
            });
        }

        /// <inheritdoc/>
        public virtual Task<IEnumerable<TEntity>> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "", // not used in memory repository
            int? skip = null, int? take = null)
        {
            try
            {
                // Get the dbSet from the Entity passed in                
                IQueryable<TEntity> query = BuildQuery(filter, orderBy, includeProperties, skip, take);

                return Task.FromResult(query.AsEnumerable());
            }
            catch 
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public virtual Task<IEnumerable<TProjection>> GetProjection<TProjection>(
            Func<IQueryable<TEntity>, IQueryable<TProjection>> projection,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            int? skip = null, int? take = null)
        {
            try
            {
                IQueryable<TEntity> query = BuildQuery(filter, orderBy, includeProperties, skip, take);
                var projectionQuery = projection(query);

                return Task.FromResult(projectionQuery.AsEnumerable());
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public virtual Task<TEntity> Insert(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), $"{nameof(entity)} is null.");
            }

            var key = _keyAccessor.GetKey(entity);
            if (key.Equals(default(TKey)))
            {
                key = _keyAccessor.NextKey();
                _keyAccessor.SetKey(entity, key);
            }

            if (data.TryGetValue(key, out TEntity existing))
            {
                entity = existing;
            }
            else
            {
                data[key] = entity ?? throw new ArgumentNullException(nameof(entity), $"{nameof(entity)} is null.");
            }

            return Task.FromResult(entity);
        }

        /// <inheritdoc/>
        public virtual Task<TEntity> Update(TEntity entityToUpdate)
        {
            if (entityToUpdate == null)
            {
                throw new ArgumentNullException(nameof(entityToUpdate), $"{nameof(entityToUpdate)} is null.");
            }

            var key = _keyAccessor.GetKey(entityToUpdate);
            if (data.TryGetValue(key, out _))
            {
                data[key] = entityToUpdate;
            }

            return Task.FromResult(entityToUpdate);
        }

        /// <inheritdoc/>
        public virtual Task<bool> Delete(TEntity entityToDelete)
        {
            if (entityToDelete == null)
            {
                throw new ArgumentNullException(nameof(entityToDelete), $"{nameof(entityToDelete)} is null.");
            }

            TKey key = _keyAccessor.GetKey(entityToDelete);
            return Delete(key);
        }

        /// <inheritdoc/>
        public virtual Task<bool> Delete(TKey id)
        {
            if (data.TryGetValue(id, out _))
            {
                data.Remove(id);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        private IQueryable<TEntity> BuildQuery(
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            string includeProperties, int? skip, int? take)
        {
            // Get the dbSet from the Entity passed in                
            IQueryable<TEntity> query = data.Values.AsQueryable();

            // Apply the filter
            if (filter != null)
                query = query.Where(filter);

            // Sort
            if (orderBy != null)
                query = orderBy(query);

            // Skip
            if ((skip ?? -1) > 0)
                query = query.Skip(skip.Value);

            // Take
            if ((take ?? -1) > 0)
                query = query.Take(take.Value);
            return query;
        }
    }
}
