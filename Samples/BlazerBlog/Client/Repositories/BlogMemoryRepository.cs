using BlazerBlog.Shared.Models;

using Munq.DataAccess.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazerBlog.Client.Repositories
{
    public class BlogMemoryRepository : MemoryRepository<IBlogRepository, Blog, int>, IBlogRepository
    {
        private const int _imageWidth = 320;
        private const int _imageHeight = 240;

        private const string _dummyAbstract =
@"<p>Pork belly schlitz shaman reprehenderit selfies. Pitchfork shabby chic in consequat, tacos beard cold-pressed 
ea raclette taiyaki polaroid est aesthetic slow-carb. Tumblr organic pabst heirloom vexillologist viral cliche, 
non activated charcoal seitan skateboard. Master cleanse letterpress skateboard, wayfarers craft beer 
intelligentsia coloring book. Eiusmod deep v chartreuse vegan iceland.</p>";

        private const string _dummyContent =
@"<p>I'm baby mixtape banjo gastropub literally. Kinfolk roof party intelligentsia skateboard. Twee drinking 
vinegar wolf, schlitz biodiesel semiotics freegan fam lo-fi tote bag flexitarian bushwick mlkshk lumbersexual 
franzen. Ugh enamel pin fixie etsy.</p>

<p>Hammock health goth pug hell of pinterest vinyl retro beard cardigan af hot chicken. Pok pok viral pour-over, 
synth iPhone shoreditch venmo tumeric umami. La croix live-edge intelligentsia, quinoa scenester next level 
pok pok trust fund gochujang williamsburg. Migas ethical organic four dollar toast hashtag disrupt hella. 
Sartorial direct trade shoreditch, aesthetic mumblecore mixtape +1 chillwave PBR&B YOLO kitsch chia master 
cleanse.</p>

<p>Hashtag vinyl meditation blue bottle hexagon, vice everyday carry. Vaporware microdosing live-edge, lumbersexual 
brooklyn activated charcoal lomo gluten-free narwhal. Plaid ethical tacos literally church-key. Shaman everyday 
carry affogato, taiyaki bicycle rights helvetica gochujang cornhole XOXO twee poutine gastropub kogi pitchfork.</p>

<p>Hot chicken jianbing occupy adaptogen. Cornhole vinyl waistcoat, butcher schlitz XOXO cardigan direct trade 
locavore. Artisan vaporware try-hard hammock. Keffiyeh blog meggings, portland deep v you probably haven't heard 
of them disrupt vinyl enamel pin tacos shoreditch dreamcatcher narwhal DIY. Snackwave 3 wolf moon plaid green 
juice glossier VHS. Portland tattooed normcore affogato kale chips chartreuse post-ironic polaroid before they 
sold out twee. Live-edge letterpress semiotics woke.</p>

<p>Bespoke blue bottle waistcoat four loko plaid cardigan. Sartorial tilde mlkshk, skateboard synth organic banh mi 
portland franzen kinfolk shabby chic authentic before they sold out. Raw denim irony hoodie blog intelligentsia 
sriracha succulents hashtag pug gluten-free 90's glossier pork belly. Deep v kinfolk swag bushwick asymmetrical 
vaporware mumblecore selvage narwhal franzen. Hot chicken air plant flexitarian, yr swag semiotics activated 
charcoal edison bulb cliche kickstarter mlkshk austin.</p>";

        private static readonly IKeyAccessor<Post, int> _postKeyAccessor = 
                                                            KeyAccessorFactory.Create<Post, int>();

        public BlogMemoryRepository()
        {
            SeedData().GetAwaiter().GetResult();
        }

        public async Task SeedData()
        {

            var blog = new Blog
            {
                Author = "Matthew",
                Title = "Matthew's Ramblings",
                Slug = "MattsRamblings",
                Abstract = _dummyAbstract,
                ImageUrl = $"https://placeimg.com/1920/200/tech",
            };

            Post post = new Post
            {
                BlogId = 1,
                Title = "Matt's First Blog Post",
                Author = "Matthew Dennis",
                Abstract = "This is Sample Blog Post #1",
                Content = _dummyContent,
                DatePosted = DateTime.Today.AddDays(-4),
                ImageUrl = $"https://loremflickr.com/{_imageWidth}/{_imageHeight}/programming"
            };

            post.Tags.Add("C#");
            post.Tags.Add("Data Access");

            blog.Posts.Add(post);

            await Insert(blog);
        }

        public override Task<Blog> Insert(Blog entity)
        {
            foreach (var post in entity.Posts)
            {
                if (post.Id == default)
                    post.Id = _postKeyAccessor.NextKey();
            }

            return base.Insert(entity);
        }
    }
}
