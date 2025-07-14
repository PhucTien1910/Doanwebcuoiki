using System.ComponentModel.DataAnnotations;
namespace Doanwebcuoiki.Models
{
    public class Blog
    {
        [Key]
        public int blog_id { get; set; }
        public string? blog_title { get; set; }
        public string? blog_image { get; set; }
        public string? blog_description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
