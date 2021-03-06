using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Munq.DataAccess.Shared
{
    /// <summary>
    /// This class wraps a repository with a Manager.  You can add functionality independent of the 
    /// specific backend, and replace the backend through DI.
    /// </summary>
    /// <remarks>All the methods are virtual so that they can be overriden to add functionality before and 
    /// after calling the base class.</remarks>
	/// <typeparam name="TRepository">The type of the Repository to implement.</typeparam>
	/// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
	/// <typeparam name="TKey">The type of the key property in TEntity.</typeparam>
    public abstract class Manager<TRepository, TEntity, TKey> : IRepository<TEntity, TKey>
        where TRepository : IRepository<TEntity, TKey>
        where TEntity     : class
        where TKey        : IEquatable<TKey>

    {
        private readonly IRepository<TEntity, TKey> _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="Manager{TRepository, TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="repository">The repository to be used by the Manager.</param>
        public Manager(IRepository<TEntity,TKey> repository)
        {
            _repository = repository;
        }

        /// <inheritdoc/>
        public virtual Task<bool> Delete(TEntity entityToDelete)
        {
            return _repository.Delete(entityToDelete);
        }

        /// <inheritdoc/>
        public virtual Task<bool> Delete(TKey id)
        {
            return _repository.Delete(id);
        }

        /// <inheritdoc/>
        public virtual Task<IEnumerable<TEntity>> GetAll(int? skip = null, int? take = null)
        {
            return _repository.GetAll(skip: skip, take: take);
        }

        /// <inheritdoc/>
        public virtual Task<TEntity> GetByID(TKey id)
        {
            return _repository.GetByID(id);
        }

        /// <inheritdoc/>
        public virtual Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "", int? skip = null, int? take = null)
        {
            if (_repository is IRepositoryExtended<TEntity, TKey> extendedRepository)
                return extendedRepository.Get(filter, orderBy, includeProperties, skip, take);
            else
                throw new NotImplementedException("The repository does not implement IRepositoryGenericGet");
        }

        /// <inheritdoc/>
        public virtual Task<IEnumerable<TProjection>> GetProjection<TProjection>(
            Func<IQueryable<TEntity>, IQueryable<TProjection>> projection,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "", int? skip = null, int? take = null)
        {
            if (_repository is IRepositoryExtended<TEntity, TKey> extendedRepository)
                return extendedRepository.GetProjection(projection, filter, orderBy, includeProperties, skip, take);
            else
                throw new NotImplementedException("The repository does not implement IRepositoryGenericGet");
        }

        /// <inheritdoc/>
        public virtual Task<TEntity> Insert(TEntity entity)
        {
            return _repository.Insert(entity);
        }

        /// <inheritdoc/>
        public virtual Task<TEntity> Update(TEntity entityToUpdate)
        {
            return _repository.Update(entityToUpdate);
        }
    }
}
