using Doanwebcuoiki.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Doanwebcuoiki.Controllers
{
    public class HomeController : Controller
    {
        private readonly myContext _context;

        public HomeController(myContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var newProducts = _context.tbl_product
                .OrderByDescending(p => p.CreatedAt)
                .Take(12)
                .ToList();

            var newProductIds = newProducts.Select(p => p.product_id).ToHashSet();

            var allProducts = _context.tbl_product.Include(p => p.Category).ToList();

            ViewBag.OnePiece = allProducts
                .Where(p => p.Category != null && NormalizeCategory(p.Category.category_name) == "onepiece")
                .ToList();

            ViewBag.Naruto = allProducts
                .Where(p => p.Category != null && NormalizeCategory(p.Category.category_name) == "naruto")
                .ToList();

            ViewBag.Kimetsu = allProducts
                .Where(p => p.Category != null && NormalizeCategory(p.Category.category_name) == "kimetsunoyaiba")
                .ToList();

            ViewBag.DragonBall = allProducts
                .Where(p => p.Category != null && NormalizeCategory(p.Category.category_name) == "dragonball")
                .ToList();

            ViewBag.Gundam = allProducts
                .Where(p => p.Category != null && NormalizeCategory(p.Category.category_name) == "gundam")
                .ToList();

            // Gán flag IsNew để AllProducts view xử lý
            foreach (var product in allProducts)
            {
                product.IsNew = newProductIds.Contains(product.product_id);
            }

            return View(newProducts);
        }

        public IActionResult AllProducts(
            List<string> categories,
            int? minPrice,
            int? maxPrice,
            int? ratings,
            string sort
)
        {
            var productsQuery = _context.tbl_product
                .Include(p => p.Category)
                .AsQueryable();

            // Sản phẩm mới (dựa vào ngày tạo mới nhất)
            var newProductIds = _context.tbl_product
                .OrderByDescending(p => p.CreatedAt)
                .Take(12)
                .Select(p => p.product_id)
                .ToHashSet();

            // Filter by category
            if (categories != null && categories.Any())
            {
                var normalized = categories
                    .Select(c => c.Trim().ToLower().Replace(" ", ""))
                    .ToList();

                bool hasSanPhamMoi = normalized.Contains("sanphammoi");

                productsQuery = productsQuery.Where(p =>
                    (
                        p.Category != null &&
                        normalized.Contains(p.Category.category_name.Trim().ToLower().Replace(" ", ""))
                    )
                    || (
                        hasSanPhamMoi && newProductIds.Contains(p.product_id)
                    )
                );
            }

            // Filter by price (giá sau giảm nếu có)
            if (minPrice.HasValue)
                productsQuery = productsQuery.Where(p => (p.product_discount_price ?? p.product_price) >= minPrice.Value);

            if (maxPrice.HasValue)
                productsQuery = productsQuery.Where(p => (p.product_discount_price ?? p.product_price) <= maxPrice.Value);

            // === SỬA Ở ĐÂY ===
            // Filter by rating (chỉ đúng mức sao được chọn)
            if (ratings.HasValue && ratings.Value > 0)
            {
                int rate = ratings.Value;
                productsQuery = productsQuery.Where(p => Math.Floor(p.product_rating) == rate);
            }

            // Sắp xếp (sort)
            switch (sort)
            {
                case "sold":
                    productsQuery = productsQuery.OrderByDescending(p => p.product_sold);
                    break;
                case "priceAsc":
                    productsQuery = productsQuery.OrderBy(p => (p.product_discount_price ?? p.product_price));
                    break;
                case "priceDesc":
                    productsQuery = productsQuery.OrderByDescending(p => (p.product_discount_price ?? p.product_price));
                    break;
                default:
                    productsQuery = productsQuery.OrderByDescending(p => p.CreatedAt);
                    break;
            }

            var productList = productsQuery.ToList();

            // Đánh dấu sản phẩm mới
            foreach (var product in productList)
            {
                product.IsNew = newProductIds.Contains(product.product_id);
            }

            // Để giữ lại trạng thái lọc
            ViewBag.SelectedCategories = categories ?? new List<string>();
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.SelectedRatings = ratings;
            ViewBag.SelectedSort = sort;

            return View("AllProducts", productList);
        }





        public IActionResult CategoryProducts(string category)
        {
            if (string.IsNullOrEmpty(category)) return RedirectToAction("Index");

            var products = _context.tbl_product
                .Include(p => p.Category)
                .ToList()
                .Where(p => p.Category != null && NormalizeCategory(p.Category.category_name) == NormalizeCategory(category))
                .ToList();

            return View("CategoryProducts", Tuple.Create(category, products));
        }

        private string NormalizeCategory(string input)
        {
            return input?.Replace(" ", "").Trim().ToLower();
        }

        public IActionResult ProductDetail(int id)
        {
            var product = _context.tbl_product
                .Include(p => p.Category)
                .FirstOrDefault(p => p.product_id == id);

            if (product == null)
                return NotFound();

            // Set cứng 4 sản phẩm gợi ý (ví dụ id 1, 2, 3, 4 - nhớ thay id này bằng id thực có trong DB bạn)
            var suggestedIds = new List<int> { 1, 2, 3, 4 };
            // Nếu id hiện tại trùng thì bỏ ra
            suggestedIds.Remove(id);

            var recentlyViewedProducts = _context.tbl_product
                .Where(p => suggestedIds.Contains(p.product_id))
                .Take(4)
                .ToList();

            ViewBag.RecentlyViewedProducts = recentlyViewedProducts;

            return View(product);
        }


        [HttpGet]
        public JsonResult AllProductsJson(int? minPrice, int? maxPrice, int[]? ratings)
        {
            var products = _context.tbl_product.AsQueryable();

            if (minPrice.HasValue)
                products = products.Where(p => p.product_price >= minPrice.Value);

            if (maxPrice.HasValue)
                products = products.Where(p => p.product_price <= maxPrice.Value);

            if (ratings != null && ratings.Any())
                products = products.Where(p => ratings.Contains((int)Math.Round(p.product_rating)));

            var result = products.Select(p => new
            {
                p.product_id,
                p.product_name,
                p.product_price,
                p.product_discount_price,
                p.product_image,
                p.product_rating
            }).ToList();

            return Json(result);
        }
        [HttpGet]
        public JsonResult SearchProducts(string q)
        {
            var keyword = (q ?? "").Trim().ToLower();

            // Nếu rỗng thì trả về tất cả hoặc rỗng, tuỳ bạn
            var query = _context.tbl_product
                .Where(p => p.product_name.ToLower().Contains(keyword))
                .Select(p => new {
                    p.product_id,
                    p.product_name,
                    p.product_image,
                    p.product_price,
                    p.product_discount_price,
                    p.product_rating
                })
                .Take(20) // Giới hạn số lượng trả về
                .ToList();

            return Json(query);
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
    }
}
