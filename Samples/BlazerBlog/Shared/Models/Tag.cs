using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazerBlog.Shared.Models
{
    /// <summary>
    /// This class represents tags that can be applied to blog post.
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// Gets or sets the Id of the Tag.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the Name of the Tag.
        /// </summary>
        [Required]
        [StringLength(maximumLength: 16, ErrorMessage = "Tag Names must be 16 characters or less")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Description of the Tag.
        /// </summary>
        [StringLength(64, ErrorMessage = "Tag Descriptions must be 64 characters or less.")]
        public string Description { get; set; }
    }
}
