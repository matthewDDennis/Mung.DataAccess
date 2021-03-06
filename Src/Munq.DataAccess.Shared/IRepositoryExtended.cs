/*
 * Base on code from Carl Franklin's Blazor Train series Episode 16 
 * Data Access (API/DataManager/EF) https://youtu.be/VaX73p3JfV4
 * Code available at http://blazordeskshow.com/blazortrainfiles/CompleteData.zip
 * 
 * I have modified to use IRepository<TEntity, TKey> instead of IRepository<TEntity> 
 * which just uses an object.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Munq.DataAccess.Shared
{
    /// <summary>
    /// Extends the <see cref="IRepository{TEntity, TKey}"/> interface to add
    /// methods to customize the query to get entities from the entity store,
    /// including one to project the results into a different type.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
    /// <typeparam name="TKey">The type of the key property in TEntity.</typeparam>
    public interface IRepositoryExtended<TEntity, TKey> :IRepository<TEntity, TKey> 
        where TEntity : class
        where TKey    : IEquatable<TKey>
    {
        /// <summary>
        /// Asynchronously gets a <see cref="IEnumerable{TEntity}"/> from the entity store using a customized query.
        /// </summary>
        /// <param name="filter">An optional expression used to filter the results,</param>
        /// <param name="orderBy">An optional function used to order the results.</param>
        /// <param name="includeProperties">An optional comma delimited list of related entities to include in the results.</param>
        /// <param name="skip">An optional number of entities to skip from the start of the results.</param>
        /// <param name="take">An optional limit of the number of entities returned in the results.</param>
        /// <returns>A <see cref="Task"/> with an <see cref="IEnumerable{TEntity}"/> result.</returns>
        Task<IEnumerable<TEntity>> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            int? skip = null, int? take = null);

        /// <summary>
        /// Asynchronously gets a <see cref="IEnumerable{TProjection}"/> from the entity store using a customized query
        /// and projects it .
        /// </summary>
        /// <param name="projection">The function used to map the entites from to type TEntity to TProjection.
        /// This function is of type <see cref="Func{TEntity, TProjection}"/>.</param>
        /// <param name="filter">An optional expression used to filter the results.</param>
        /// <param name="orderBy">An optional function used to order the results.</param>
        /// <param name="includeProperties">An optional comma delimited list of related entities to include in the results.</param>
        /// <param name="skip">An optional number of entities to skip from the start of the results.</param>
        /// <param name="take">An optional limit of the number of entities returned in the results.</param>
        /// <returns>A <see cref="Task"/> with an <see cref="IEnumerable{TProjection}"/> result.</returns>
        Task<IEnumerable<TProjection>> GetProjection<TProjection>(
            Func<IQueryable<TEntity>, IQueryable<TProjection>> projection,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            int? skip = null, int? take = null);
    }
}
