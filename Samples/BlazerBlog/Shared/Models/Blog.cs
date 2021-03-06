﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazerBlog.Shared.Models
{
    public class Blog
    {
        /// <summary>
        /// Gets or sets the Id of the Blog.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the Author of the blog.
        /// </summary>
        [Required]
        public string Author { get; set; }

        /// <summary>
        /// Gets or set the title of the blog.
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the slug used to form the url for the blog.
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Gets or sets a brief description of the blog.
        /// </summary>
        [Required]
        public string Abstract { get; set; }

        /// <summary>
        /// Gets or sets the image to display in the blog header.
        /// </summary>
        [Required]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the list of Posts associated with the post.
        /// </summary>
        public List<Post> Posts { get; set; } = new List<Post>();
    }
}
