using BlazerBlog.Shared.Models;

using Munq.DataAccess.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazerBlog.Client.Repositories
{
    public class TagMemoryRepository : MemoryRepository<ITagRepository, Tag, string>, ITagRepository
    {
        public TagMemoryRepository()
        {
            SeedData().GetAwaiter().GetResult();
        }

        private async Task SeedData()
        {
            await Insert(new Tag { Name = "Data Access", Description = "All about managing Data" });
            await Insert(new Tag { Name = ".NET Core", Description = "All about .NET Core" });
            await Insert(new Tag { Name = "Blazor", Description = "All about Blazor" });
            await Insert(new Tag { Name = "Tutorial", Description = "Tutorials" });
            await Insert(new Tag { Name = "C#", Description = "C# Language" });
        }
    }
}
