using BlazerBlog.Client.Repositories;
using BlazerBlog.Shared.Models;

using Munq.DataAccess.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazerBlog.Client.Services
{
    public class TagService : Manager<ITagRepository, Tag, string>
    {
        public TagService(ITagRepository repository)
        : base(repository)
        {
        }

        public override Task<IEnumerable<Tag>> GetAll(int? skip = null, int? take = null)
        {
            return base.Get(orderBy: (query) => query.OrderBy(tag => tag.Name));
        }
    }
}
