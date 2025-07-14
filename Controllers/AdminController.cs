using Doanwebcuoiki.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Doanwebcuoiki.Controllers
{
    public class AdminController : Controller
    {
        private myContext _context;
        private IWebHostEnvironment _env;
        public AdminController(myContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            string admin_session = HttpContext.Session.GetString("admin_session");
            if(admin_session!= null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("login");
            }
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string adminEmail, string adminPassword)
        {
            var row = _context.tbl_admin.FirstOrDefault(a => a.admin_email == adminEmail);
            if(row != null && row.admin_password == adminPassword)
            {
                HttpContext.Session.SetString("admin_session",row.admin_id.ToString());
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.message = "Incorrect Username or Password";
                return View();
            }
       
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("admin_session");
            return RedirectToAction("login");
        }
        public IActionResult Profile()
        {
            var adminId = HttpContext.Session.GetString("admin_session");
            if (string.IsNullOrEmpty(adminId))
            {
                return RedirectToAction("Profile");
            }

            var row = _context.tbl_admin
                .Where(a => a.admin_id == int.Parse(adminId))
                .ToList();

            return View(row);
        }
        [HttpPost]
        public IActionResult Profile(Admin admin)
        {
            _context.tbl_admin.Update(admin);
            _context.SaveChanges();
            return RedirectToAction("Profile");
        }
        [HttpPost]
        public IActionResult ChangeProfileImage(IFormFile admin_image, Admin admin)
        {
            var adminId = HttpContext.Session.GetString("admin_session");
            if (string.IsNullOrEmpty(adminId) || !int.TryParse(adminId, out int id))
            {
                return RedirectToAction("Profile");
            }

            var existingAdmin = _context.tbl_admin.FirstOrDefault(a => a.admin_id == id);
            if (existingAdmin == null)
            {
                return RedirectToAction("Profile");
            }

            if (admin_image != null && admin_image.Length > 0)
            {
                string ImagePath = Path.Combine(_env.WebRootPath, "admin_image", admin_image.FileName);
                using (var fs = new FileStream(ImagePath, FileMode.Create))
                {
                    admin_image.CopyTo(fs);
                }
                existingAdmin.admin_image = admin_image.FileName;
            }

            _context.tbl_admin.Update(existingAdmin);
            _context.SaveChanges();
            return RedirectToAction("Profile");
        }
        public IActionResult fetchCustomer()
        {
            return View(_context.tbl_customer.ToList());
        }
        public IActionResult customerDetails(int id)
        {            
            return View(_context.tbl_customer.FirstOrDefault(c => c.customer_id == id));
        }
        public IActionResult updateCustomer(int id)
        {
            return View(_context.tbl_customer.Find(id));
        }
        [HttpPost]
        public IActionResult updateCustomer(Customer customer, IFormFile customer_image)
        {
            // Lấy id từ model truyền lên (chính là customer.customer_id)
            int id = customer.customer_id;

            var existingCustomer = _context.tbl_customer.FirstOrDefault(c => c.customer_id == id);
            if (existingCustomer == null)
            {
                return RedirectToAction("fetchCustomer");
            }

            // Gán lại các trường từ customer lên existingCustomer
            existingCustomer.customer_name = customer.customer_name;
            existingCustomer.customer_phone = customer.customer_phone;
            existingCustomer.customer_email = customer.customer_email;
            existingCustomer.customer_password = customer.customer_password;
            existingCustomer.customer_gender = customer.customer_gender;
            existingCustomer.customer_country = customer.customer_country;
            existingCustomer.customer_city = customer.customer_city;
            existingCustomer.customer_address = customer.customer_address;

            if (customer_image != null && customer_image.Length > 0)
            {
                string ImagePath = Path.Combine(_env.WebRootPath, "customer_images", customer_image.FileName);
                using (var fs = new FileStream(ImagePath, FileMode.Create))
                {
                    customer_image.CopyTo(fs);
                }
                existingCustomer.customer_image = customer_image.FileName;
            }

            _context.tbl_customer.Update(existingCustomer);
            _context.SaveChanges();
            return RedirectToAction("fetchCustomer");
        }
        public IActionResult deletePermission(int id)
        {
            return View(_context.tbl_customer.FirstOrDefault(c => c.customer_id == id));
        }

        public IActionResult deleteCustomer(int id)
        {
            var customer = _context.tbl_customer.Find(id);
            _context.tbl_customer.Remove(customer);
            _context.SaveChanges();
            return RedirectToAction("fetchCustomer");
        }
        public IActionResult fetchCategory()
        {
            return View(_context.tbl_category.ToList());
        }
        public IActionResult addCategory()
        {
            return View();
        }
        [HttpPost]
        public IActionResult addCategory(Category cat)
        {
            _context.tbl_category.Add(cat);
            _context.SaveChanges();
            return RedirectToAction("fetchCategory");
        }
        public IActionResult updateCategory(int id)
        {
            var category = _context.tbl_category.Find(id);
            return View(category);
        }
        [HttpPost]
        public IActionResult updateCategory(Category cat)
        {
            _context.tbl_category.Update(cat);
            _context.SaveChanges();
            return RedirectToAction("fetchCategory");
        }
        public IActionResult deletePermissionCategory(int id)
        {
            return View(_context.tbl_category.FirstOrDefault(c => c.category_id == id));
        }
        public IActionResult deleteCategory(int id)
        {
            var category = _context.tbl_category.Find(id);
            _context.tbl_category.Remove(category);
            _context.SaveChanges();
            return RedirectToAction("fetchCategory");
        }
        public IActionResult fetchProduct()
        {
            return View(_context.tbl_product.ToList());
        }
        public IActionResult addProduct()
        {
            List<Category> categories = _context.tbl_category.ToList();
            ViewData["category"] = categories;
            return View();
        }
        // THÊM SẢN PHẨM - CÓ NHIỀU ẢNH PHỤ
        [HttpPost]
        public IActionResult addProduct(IFormFile product_image, List<IFormFile> product_images)
        {
            var form = Request.Form;

            Product prod = new Product
            {
                product_name = form["product_name"],
                product_price = int.Parse(form["product_price"]),
                product_description = form["product_description"],
                cat_id = int.Parse(form["cat_id"]),
                product_discount_price = string.IsNullOrEmpty(form["product_discount_price"]) ? null : int.Parse(form["product_discount_price"]),
                product_rating = double.TryParse(form["product_rating"], out double rating) ? rating : 0,
                product_review_count = int.TryParse(form["product_review_count"], out int reviews) ? reviews : 0,
                product_sold = int.TryParse(form["product_sold"], out int sold) ? sold : 0,
                CreatedAt = DateTime.Now
            };

            // Ảnh chính
            if (product_image != null && product_image.Length > 0)
            {
                string fileName = Path.GetFileName(product_image.FileName);
                string path = Path.Combine(_env.WebRootPath, "product_images", fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    product_image.CopyTo(stream);
                }
                prod.product_image = fileName;
            }

            _context.tbl_product.Add(prod);
            _context.SaveChanges();

            // Ảnh phụ
            if (product_images != null && product_images.Any())
            {
                foreach (var img in product_images)
                {
                    if (img != null && img.Length > 0)
                    {
                        string fileName = Path.GetFileName(img.FileName);
                        string path = Path.Combine(_env.WebRootPath, "product_images", fileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            img.CopyTo(stream);
                        }
                        var productImage = new ProductImage
                        {
                            ProductId = prod.product_id,
                            ImagePath = fileName
                        };
                        _context.ProductImage.Add(productImage);
                    }
                }
                _context.SaveChanges();
            }

            return RedirectToAction("fetchProduct");
        }

        public IActionResult productDetails(int id)
        {
            return View(_context.tbl_product.Include(p => p.Category)
                                           .Include(p => p.ProductImages)
                                           .FirstOrDefault(p => p.product_id == id));
        }

        public IActionResult updateProduct(int id)
        {
            List<Category> categories = _context.tbl_category.ToList();
            ViewData["category"] = categories;

            var product = _context.tbl_product
                .Include(p => p.ProductImages)
                .FirstOrDefault(p => p.product_id == id);

            ViewBag.selectedCategoryId = product.cat_id;
            return View(product);
        }

        // UPDATE SẢN PHẨM + THÊM ẢNH PHỤ
        [HttpPost]
        public IActionResult updateProduct(Product product, List<IFormFile> product_images)
        {
            var existingProduct = _context.tbl_product
                .Include(p => p.ProductImages)
                .FirstOrDefault(p => p.product_id == product.product_id);

            if (existingProduct == null)
            {
                return RedirectToAction("fetchProduct");
            }

            existingProduct.product_name = product.product_name;
            existingProduct.product_price = product.product_price;
            existingProduct.product_description = product.product_description;
            existingProduct.cat_id = product.cat_id;
            existingProduct.product_discount_price = product.product_discount_price;
            existingProduct.product_rating = product.product_rating;
            existingProduct.product_review_count = product.product_review_count;
            existingProduct.product_sold = product.product_sold;

            // Ảnh phụ mới (nếu có)
            if (product_images != null && product_images.Any())
            {
                foreach (var img in product_images)
                {
                    if (img != null && img.Length > 0)
                    {
                        string fileName = Path.GetFileName(img.FileName);
                        string path = Path.Combine(_env.WebRootPath, "product_images", fileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            img.CopyTo(stream);
                        }
                        var productImage = new ProductImage
                        {
                            ProductId = existingProduct.product_id,
                            ImagePath = fileName
                        };
                        _context.ProductImage.Add(productImage);
                    }
                }
            }

            _context.tbl_product.Update(existingProduct);
            _context.SaveChanges();

            return RedirectToAction("fetchProduct");
        }
        [HttpPost]
        public IActionResult DeleteProductImage(int imageId, int productId)
        {
            var image = _context.ProductImage.FirstOrDefault(pi => pi.Id == imageId);
            if (image != null)
            {
                // Xóa file vật lý nếu muốn (option)
                var imgPath = Path.Combine(_env.WebRootPath, "product_images", image.ImagePath);
                if (System.IO.File.Exists(imgPath))
                {
                    System.IO.File.Delete(imgPath);
                }
                _context.ProductImage.Remove(image);
                _context.SaveChanges();
            }
            // Trở lại trang sửa sản phẩm
            return RedirectToAction("updateProduct", new { id = productId });
        }
        [HttpPost]
        public IActionResult ChangeProductImage(IFormFile product_image, Product product)
        {
            int id = product.product_id;
            var existingProduct = _context.tbl_product.FirstOrDefault(p => p.product_id == id);
            if (existingProduct == null)
            {
                return RedirectToAction("fetchProduct");
            }
            if (product_image != null && product_image.Length > 0)
            {
                string ImagePath = Path.Combine(_env.WebRootPath, "product_images", product_image.FileName);
                using (var fs = new FileStream(ImagePath, FileMode.Create))
                {
                    product_image.CopyTo(fs);
                }
                existingProduct.product_image = product_image.FileName;
            }
            _context.tbl_product.Update(existingProduct);
            _context.SaveChanges();
            return RedirectToAction("fetchProduct");
        }

    }
}
