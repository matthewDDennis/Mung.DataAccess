using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazerBlog.Shared.Models
{
    public class Post
    {
        /// <summary>
        /// Gets or sets the Id of the Blog Post.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The Id of the Blog
        /// </summary>
        public int BlogId { get; set; }

        /// <summary>
        /// Gets or sets the Date the item was posted.
        /// </summary>
        [Required]
        public DateTime DatePosted { get; set; }

        /// <summary>
        /// Gets or sets the name of the Author of the blog post.
        /// </summary>
        [Required]
        public string Author { get; set; }

        /// <summary>
        /// Gets or set the title of the blog post.
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a brief description of the blog post.
        /// </summary>
        [Required]
        public string Abstract { get; set; }

        /// <summary>
        /// Gets or sets the body of the blog post.
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the image to display in the above the blog post.
        /// </summary>
        [Required]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the list of tags associated with the blog post.
        /// </summary>
        public IEnumerable<Tag> Tags { get; set; } = new List<Tag>();

    }
}
