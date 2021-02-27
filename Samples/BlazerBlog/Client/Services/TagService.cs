using BlazerBlog.Client.Repositories;
using BlazerBlog.Shared.Models;

using Munq.DataAccess.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazerBlog.Client.Services
{
    public class TagService : Manager<ITagRepository, Tag, int>
    {
        public TagService(ITagRepository repository)
        : base(repository)
        {
        }

        public async Task SeedData()
        {
            await Insert(new Tag { Id = 1, Name = "Data Access", Description = "All about managing Data" });
            await Insert(new Tag { Id = 2, Name = ".NET Core",   Description = "All about .NET Core" });
            await Insert(new Tag { Id = 3, Name = "Blazor",      Description = "All about Blazor" });
            await Insert(new Tag { Id = 4, Name = "Tutorial",    Description = "Tutorials" });

        }

        public override Task<IEnumerable<Tag>> GetAll(int? skip = null, int? take = null)
        {
            return base.Get(orderBy: (query) => query.OrderBy(tag => tag.Name));
        }
    }
}
