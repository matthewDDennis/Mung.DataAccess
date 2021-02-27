using BlazerBlog.Shared.Models;

using Munq.DataAccess.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazerBlog.Client.Repositories
{
    public class TagMemoryRepository : MemoryRepository<ITagRepository, Tag, int>, ITagRepository
    {
        public TagMemoryRepository()
        {
        }
    }
}
