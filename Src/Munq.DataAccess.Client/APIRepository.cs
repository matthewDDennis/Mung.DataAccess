/*
 * Base on code from Carl Franklin's Blazor Train series Episode 16 
 * Data Access (API/DataManager/EF) https://youtu.be/VaX73p3JfV4
 * Code available at http://blazordeskshow.com/blazortrainfiles/CompleteData.zip
 * 
 * I have modified to use IRepository<TEntity, TKey> instead of IRepository<TEntity> 
 * which just uses an object.
 * Also modified to use the System.Net.Http.Json extensions instead of NewtonSoft.Json.Net
*/

using Munq.DataAccess.Shared;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Munq.DataAccess.Client
{
    /// <summary>
    /// Reusable API Repository base class that provides access to CRUD APIs
    /// </summary>
	/// <typeparam name="TRepository">The type of the Repository to implement.</typeparam>
    /// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
    /// <typeparam name="TKey">The type of the key property in TEntity.</typeparam>
    public abstract class APIRepository<TRepository, TEntity, TKey> : IRepository<TEntity, TKey>
        where TRepository : IRepository<TEntity, TKey>
        where TEntity     : class
        where TKey        : struct, IEquatable<TKey>
    {
        private readonly string     _controllerName;
        private readonly HttpClient _http;

        private readonly IKeyAccessor<TEntity, TKey> _keyAccessor;

        /// <summary>
        /// Gets the <see cref="Client"/> used by the repository.
        /// </summary>
        protected HttpClient Client => _http;

        /// <summary>
        /// Initializes a new instance of the <see cref="APIRepository{TRepository, TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="_http">A HttpClient to use to call the WebApi.</param>
        /// <param name="_controllerName">The name of the controller such as "api/Product"</param>
        public APIRepository(HttpClient _http, string _controllerName)
        {
            this._http           = _http;
            this._controllerName = _controllerName;
            _keyAccessor         = KeyAccessorFactory.Create<TEntity, TKey>();
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TEntity>> GetAll(int? skip = null, int? take = null)
        {
            try
            {
                var urlString = _controllerName;
                if (skip != null)
                    urlString += $"?skip={skip}";

                if (take != null)
                    urlString += $"{ ((skip != null) ? "&" : "?")}take={take}";

                var result = await Client.GetAsync(urlString);
                return await ReadListAsync<TEntity>(result);
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public virtual async Task<TEntity> GetByID(TKey id)
        {
            try
            {
                var arg    = WebUtility.HtmlEncode(id.ToString());
                var url    = _controllerName + "/" + arg;
                var result = await Client.GetAsync(url);
                return await ReadSingleAsync<TEntity>(result);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return null;
            }
        }

        /// <inheritdoc/>
        public virtual async Task<TEntity> Insert(TEntity entity)
        {
            try
            {
                var result = await Client.PostAsJsonAsync(_controllerName, entity);
                result.EnsureSuccessStatusCode();

                var response = await result.Content.ReadFromJsonAsync<APIEntityResponse<TEntity>>();
                return await ReadSingleAsync<TEntity>(result);
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public virtual async Task<TEntity> Update(TEntity entityToUpdate)
        {
            try
            {
                var result = await Client.PutAsJsonAsync(_controllerName, entityToUpdate);
                result.EnsureSuccessStatusCode();

                var response = await result.Content.ReadFromJsonAsync<APIEntityResponse<TEntity>>();
                return await ReadSingleAsync<TEntity>(result);
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public virtual async Task<bool> Delete(TEntity entityToDelete)
        {
            try
            {
                var value  = _keyAccessor.GetKey(entityToDelete)
                             .ToString();

                var url    = _controllerName + "/" + WebUtility.HtmlEncode(value);
                var result = await Client.DeleteAsync(url);
                result.EnsureSuccessStatusCode();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public virtual async Task<bool> Delete(TKey id)
        {
            try
            {
                var url    = _controllerName + "/" + WebUtility.HtmlEncode(id.ToString());
                var result = await Client.DeleteAsync(url);
                result.EnsureSuccessStatusCode();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Reads a list of TEntity from the Request results content.
        /// </summary>
        /// <typeparam name="TData">The type of data in the list.</typeparam>
        /// <param name="result">The result of the request.</param>
        /// <returns>A <see cref="Task"/> with a <see cref="IEnumerable{TData}"/> result.</returns>
        protected static async Task<IEnumerable<TData>> ReadListAsync<TData>(HttpResponseMessage result)
            where TData : class
        {
            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadFromJsonAsync<APIListOfEntitiesResponse<TData>>();

            if (response?.Success ?? false)
                return response.Data;
            else
                return new List<TData>();
        }

        /// <summary>
        /// Reads a single TEntity from the Request results content.
        /// </summary>
        /// <typeparam name="TData">The type of data in the list.</typeparam>
        /// <param name="result">The result of the request.</param>
        /// <returns>A <see cref="Task"/> with a TEntity result.</returns>
        protected static async Task<TData> ReadSingleAsync<TData>(HttpResponseMessage result)
            where TData : class
        {
            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadFromJsonAsync<APIEntityResponse<TData>>();

            if (response?.Success ?? false)
                return response.Data;
            else
                return default;
        }
    }
}
