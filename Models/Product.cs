using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doanwebcuoiki.Models
{
    public class Product
    {
        [Key]
        public int product_id { get; set; }

        public string product_name { get; set; }

        public int product_price { get; set; }

        public string product_description { get; set; }

        public string product_image { get; set; }

        public int? cat_id { get; set; }

        public Category Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // 👉 THÊM MỚI
        public int? product_discount_price { get; set; }     // Giá sau khi giảm (nullable)
        public int product_sold { get; set; } = 0;           // Số lượng đã bán
        public double product_rating { get; set; } = 0.0;    // Điểm đánh giá trung bình
        public int product_review_count { get; set; } = 0;   // Số lượng đánh giá
        [NotMapped] // thuộc tính không lưu trong database
        public bool IsNew { get; set; } = false;
        public ICollection<ProductImage> ProductImages { get; set; }

    }
}
