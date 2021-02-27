
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Munq.DataAccess.Shared;

namespace Munq.DataAccess.Server
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class ApiControllerBase<TEntity, TKey> : ControllerBase
        where TEntity : class
        where TKey : struct, IEquatable<TKey>
    {
        private readonly IRepositoryExtended<TEntity, TKey>                    _repository;
        private readonly Expression<Func<TEntity, bool>>                       _filter;
        private readonly Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> _orderBy;
        private readonly string                                                _includeProperties;

        /// <param name="filter">An optional expression used to filter the results,</param>
        /// <param name="orderBy">An optional function used to order the results.</param>
        /// <param name="includeProperties">An optional comma delimited list of related entities to include in the results.</param>
        public ApiControllerBase(IRepositoryExtended<TEntity, TKey> repository,
                Expression<Func<TEntity, bool>> filter = null,
                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                string includeProperties = "")
        {
            _repository        = repository;
            _filter            = filter;
            _orderBy           = orderBy;
            _includeProperties = includeProperties;

        }

        /// <summary>
        /// Gets the Repository used by the controller.
        /// </summary>
        public IRepositoryExtended<TEntity, TKey> Repository => _repository;

        // GET: api/<Controller>
        [HttpGet]
        public virtual async Task<ActionResult<APIListOfEntitiesResponse<TEntity>>>
            GetAllCompanies([FromQuery] int? skip, [FromQuery] int? take)
        {
            try
            {
                var result = await _repository.Get(
                    filter: _filter,
                    orderBy: _orderBy,
                    includeProperties: _includeProperties,
                    skip: skip, take: take);

                return Ok(new APIListOfEntitiesResponse<TEntity>()
                {
                    Success = true,
                    Data    = result
                });
            }
            catch
            {
                return StatusCode(500);
            }
        }

        // GET api/<TEntityController>/5
        [HttpGet("{id}")]
        public virtual async Task<ActionResult<APIEntityResponse<TEntity>>> GetByTEntityId(TKey id)
        {
            try
            {
                var result = await _repository.GetByID(id);
                return Ok(new APIEntityResponse<TEntity>()
                {
                    Success = true,
                    Data    = result
                });
            }
            catch
            {
                // log exception here
                return StatusCode(500);
            }
        }

        // POST api/<TEntityController>
        [HttpPost]
        public virtual async Task<ActionResult<APIEntityResponse<TEntity>>> Post([FromBody] TEntity entityToCreate)
        {
            try
            {
                var savedEntity = await _repository.Insert(entityToCreate);
                return Ok(new APIEntityResponse<TEntity>()
                {
                    Success = true,
                    Data    = savedEntity
                });
            }
            catch
            {
                // log exception here
                return StatusCode(500);
            }


        }

        // PUT api/<TEntityController>/5
        [HttpPut()]
        public virtual async Task<ActionResult<APIEntityResponse<TEntity>>> Put([FromBody] TEntity entityToUpdate)
        {
            try
            {
                var result = await _repository.Update(entityToUpdate);
                return Ok(new APIEntityResponse<TEntity>()
                {
                    Success = true,
                    Data    = result
                });
            }
            catch
            {
                // log exception here
                return StatusCode(500);
            }
        }

        // DELETE api/<TEntityController>/5
        [HttpDelete("{id}")]
        public virtual async Task<ActionResult<APIEntityResponse<TEntity>>> Delete(TKey id)
        {
            try
            {
                var result = await _repository.Delete(id);
                return Ok(new APIEntityResponse<TEntity>()
                {
                    Success = true,
                    Data    = null
                });
            }
            catch
            {
                // log exception here
                return StatusCode(500);
            }
        }
    }
}
