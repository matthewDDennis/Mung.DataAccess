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
using System.Text;
using System.Threading.Tasks;

namespace Munq.DataAccess.Shared
{
    /// <summary>
    /// The interface for a basic CRUD repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
    /// <typeparam name="TKey">The type of the key property in TEntity.</typeparam>
    public interface IRepository<TEntity, TKey> 
        where TEntity : class
        where TKey    : IEquatable<TKey>
    {
        /// <summary>
        /// Gets 'All' the entities from the entity store, with optional paging.
        /// </summary>
        /// <param name="skip">The number of entities to skip. 
        /// If null then starts from the first entity in the entity store.</param>
        /// <param name="take">The number of entities to take. 
        /// If null then there no limit on the number of entites retrieved from the entity store.</param>
        /// <returns>A list entities.</returns>
        Task<IEnumerable<TEntity>> GetAll(int? skip = null, int? take = null);

        /// <summary>
        /// Gets an entity from the entity store by it's Id.
        /// </summary>
        /// <param name="id">The Id of the entity to get.</param>
        /// <returns>An entity if found, null otherwise.</returns>
        Task<TEntity> GetByID(TKey id);

        /// <summary>
        /// Adds an entity to the entity store.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The inserted entity.</returns>
        Task<TEntity> Insert(TEntity entity);


        /// <summary>
        /// Updates an entity in the entity store.
        /// </summary>
        /// <param name="entityToUpdate">The entity to update.</param>
        /// <returns>The updated entity.</returns>
        Task<TEntity> Update(TEntity entityToUpdate);

        /// <summary>
        /// Deletes an entity from the entity store.
        /// </summary>
        /// <param name="entityToDelete">The entity to delete.</param>
        /// <returns>The true if successful.</returns>
        Task<bool> Delete(TEntity entityToDelete);

        /// <summary>
        /// Deletes an entity from the entity store by it's Id.
        /// </summary>
        /// <param name="id">The id of the entity to delete.</param>
        /// <returns>The true if successful.</returns>
        Task<bool> Delete(TKey id);
    }

 }
