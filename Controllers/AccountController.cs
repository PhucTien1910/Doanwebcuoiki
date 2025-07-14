using Doanwebcuoiki.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Doanwebcuoiki.Controllers
{
    public class AccountController : Controller
    {
        private readonly myContext _context;
        public AccountController(myContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string tab = "login")
        {
            ViewBag.ActiveTab = tab;
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Đăng nhập admin cố định
            if (username == "admin@gmail.com" && password == "admin123")
            {
                HttpContext.Session.SetString("Role", "Admin");
                HttpContext.Session.SetString("AdminEmail", username);
                HttpContext.Session.SetString("admin_session", "1");
                return RedirectToAction("Index", "Admin");
            }

            // Tìm user theo tên đăng nhập
            var user = _context.tbl_customer.FirstOrDefault(c => c.customer_name == username);

            if (user == null)
            {
                ViewBag.Error = "Không tìm thấy tài khoản, xin vui lòng đăng ký!";
                ViewBag.ActiveTab = "login"; // Không chuyển tab, chỉ ở login
                return View("Login");
            }

            if (user.customer_password != password)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng!";
                ViewBag.ActiveTab = "login";
                return View("Login");
            }

            // Đăng nhập thành công
            HttpContext.Session.SetString("Role", "Customer");
            HttpContext.Session.SetString("CustomerId", user.customer_id.ToString());
            HttpContext.Session.SetString("CustomerName", user.customer_name ?? "");
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.ActiveTab = "register";
            return View("Login");
        }

        [HttpPost]
        public IActionResult Register(string username, string email, string password, string confirmPassword)
        {
            // Check email @gmail.com
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@gmail\.com$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(email, emailPattern))
            {
                ViewBag.ErrorSignup = "Vui lòng nhập địa chỉ email hợp lệ (@gmail.com)";
                ViewBag.ActiveTab = "register";
                return View("Login");
            }

            // Password validation
            string passwordPattern = @"^(?=.*[A-Z])(?=.*[@!*\$])[A-Za-z\d@!*\$]{8,}$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(password, passwordPattern))
            {
                ViewBag.ErrorSignup = "Mật khẩu phải tối thiểu 8 ký tự, có ít nhất 1 chữ hoa và 1 ký tự đặc biệt (@!*$)";
                ViewBag.ActiveTab = "register";
                return View("Login");
            }

            if (password != confirmPassword)
            {
                ViewBag.ErrorSignup = "Mật khẩu xác nhận không khớp!";
                ViewBag.ActiveTab = "register";
                return View("Login");
            }
            if (_context.tbl_customer.Any(c => c.customer_email == email))
            {
                ViewBag.ErrorSignup = "Tài khoản đã tồn tại, vui lòng thử lại";
                ViewBag.ActiveTab = "register";
                return View("Login");
            }
            if (_context.tbl_customer.Any(c => c.customer_name == username))
            {
                ViewBag.ErrorSignup = "Tài khoản đã tồn tại, vui lòng thử lại";
                ViewBag.ActiveTab = "register";
                return View("Login");
            }

            try
            {
                var newCustomer = new Customer
                {
                    customer_name = username,
                    customer_email = email,
                    customer_password = password,
                    customer_phone = "",
                    customer_gender = "",
                    customer_country = "",
                    customer_city = "",
                    customer_address = "",
                    customer_image = ""
                };
                _context.tbl_customer.Add(newCustomer);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorSignup = "Có lỗi khi tạo tài khoản: " + ex.ToString();
                ViewBag.ActiveTab = "register";
                return View("Login");
            }

            // Chỉ chuyển qua tab login nếu đăng ký thành công
            ViewBag.ActiveTab = "login";
            ViewBag.RegisterSuccess = "Đăng ký thành công! Vui lòng đăng nhập.";
            return View("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
