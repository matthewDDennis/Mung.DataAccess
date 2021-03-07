using BlazerBlog.Client.Repositories;
using BlazerBlog.Shared.Models;

using Munq.DataAccess.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazerBlog.Client.Services
{
    public class BlogService : Manager<IBlogRepository, Blog, int>
    {
        public BlogService(IBlogRepository repository)
        : base(repository)
        {
        }

        public override Task<IEnumerable<Blog>> GetAll(int? skip = null, int? take = null)
        {
            return base.Get(orderBy: (query) => query.OrderBy(blog => blog.Title),
                            skip: skip, take: take);
        }

        public async Task<Blog> GetBlogBySlug(string slug)
        {
            slug = slug.ToLower();
            var blogs = await base.Get(filter: (b) => b.Slug.ToLower() == slug);
            return blogs.FirstOrDefault();
        }
    }
}
