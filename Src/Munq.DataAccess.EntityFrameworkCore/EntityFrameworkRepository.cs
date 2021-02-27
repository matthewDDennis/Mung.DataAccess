/*
 * Base on code from Carl Franklin's Blazor Train series Episode 16 
 * Data Access (API/DataManager/EF) https://youtu.be/VaX73p3JfV4
 * Code available at http://blazordeskshow.com/blazortrainfiles/CompleteData.zip
 * 
 * I have modified to use TEntity<TKey> instead of TEntity which just uses an object.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Munq.DataAccess.Shared;

namespace Munq.DataAccess.EntityFrameworkCore
{
    /// <summary>
    /// An Entity Framework repository base class.
    /// </summary>
	/// <typeparam name="TRepository">The type of the Repository to implement.</typeparam>
    /// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
    /// <typeparam name="TKey">The type of the key property in TEntity.</typeparam>
    /// <typeparam name="TDataContext">The type of the DbContext used by the repository.</typeparam>
    public abstract class EntityFrameworkRepository<TRepository, TEntity, TKey, TDataContext> : IRepositoryExtended<TEntity, TKey>
        where TRepository  : IRepository<TEntity, TKey>
        where TEntity      : class
        where TKey         : struct, IEquatable<TKey>
        where TDataContext : DbContext
    {
        private readonly TDataContext   _context;
        private readonly DbSet<TEntity> _dbSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkRepository{TRepository, TEntity, TKey, TDataContext}"/> class
        /// </summary>
        /// <param name="dataContext">The type of <see cref="DbContext"/> that will be used by the repository./></param>
        public EntityFrameworkRepository(TDataContext dataContext)
        {
            _context = dataContext;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            _dbSet   = _context.Set<TEntity>();
        }

        /// <inheritdoc/>
        public virtual Task<IEnumerable<TEntity>> GetAll(int? skip = null, int? take = null)
        {
            return Get(skip: skip, take: take);
        }

        /// <inheritdoc/>
        public virtual Task<TEntity> GetByID(TKey id)
        {
            return _dbSet.FindAsync(id).AsTask();
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TEntity>> Get(
            Expression<Func<TEntity, bool>> filter = null, 
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, 
            string includeProperties = "", 
            int? skip = null, 
            int? take = null)
        {
            try
            {
                IQueryable<TEntity> query = BuildQuery(filter, orderBy, includeProperties, skip, take);

                return await query.ToListAsync();
            }
            catch 
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TProjection>> GetProjection<TProjection>(
            Func<IQueryable<TEntity>, IQueryable<TProjection>> projection,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            int? skip = null, 
            int? take = null)
        {
            try
            {
                IQueryable<TEntity> query = BuildQuery(filter, orderBy, includeProperties, skip, take);
                var projectionQuery       = projection(query);

                return await projectionQuery.ToListAsync();
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public virtual async Task<TEntity> Insert(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <inheritdoc/>
        public virtual async Task<TEntity> Update(TEntity entityToUpdate)
        {
            var dbSet = _context.Set<TEntity>();
            dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entityToUpdate;
        }
        /// <inheritdoc/>
        public virtual async Task<bool> Delete(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
            return await _context.SaveChangesAsync() >= 1;
        }

        /// <inheritdoc/>
        public virtual async Task<bool> Delete(TKey id)
        {
            TEntity entityToDelete = await _dbSet.FindAsync(id);

            if (!(entityToDelete is null))
                return await Delete(entityToDelete);

            return false;
        }

        private IQueryable<TEntity> BuildQuery(
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            string includeProperties, int? skip, int? take)
        {
            // Get the dbSet from the Entity passed in                
            IQueryable<TEntity> query = _dbSet;

            // Apply the filter
            if (filter != null)
                query = query.Where(filter);

            // Include the specified properties
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

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
