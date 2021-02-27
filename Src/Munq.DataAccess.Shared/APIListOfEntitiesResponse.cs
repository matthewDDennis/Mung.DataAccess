/*
 * Base on code from Carl Franklin's Blazor Train series Episode 16 
 * Data Access (API/DataManager/EF) https://youtu.be/VaX73p3JfV4
 * Code available at http://blazordeskshow.com/blazortrainfiles/CompleteData.zip
*/

using System;
using System.Collections.Generic;

namespace Munq.DataAccess.Shared
{
    /// <summary>
    /// A class to hold the response from the server to the client for a 
    /// multiple entity request.
    /// </summary>
    /// <typeparam name="TEntity">The type of data returned.</typeparam>
    public class APIListOfEntitiesResponse<TEntity> where TEntity : class
    {
        /// <summary>
        /// Gets or sets whether the request was successful and that there is data.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets a list of error messages.
        /// </summary>
        public List<string> ErrorMessages { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the data for the response.
        /// </summary>
        public IEnumerable<TEntity> Data { get; set; }
    }
}
