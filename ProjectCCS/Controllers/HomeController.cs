using ProjectCCS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectCCS.ViewsModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Globalization;
using System.Text;
using System.Security.Cryptography;

namespace ProjectCCS.Controllers
{
    public class HomeController : Controller
    {
        ContextDB context = new ContextDB();

        public bool CheckLogin()
        {
            HttpCookie cookie = Request.Cookies["user"];
            if (cookie == null)
                return false;
            else
                return true;
        }
        public string getUser()
        {
            return Request.Cookies["user"].Value;
        }
        public void SavefileToServer(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var filename = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/app/img/Giay"), filename);
                file.SaveAs(path);
            }
        }
        public void SendMail(IEnumerable<ShoppingCart> list, int IdBill)
        {
            var user = getUser();
            MailMessage mail = new MailMessage();
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("nguyenvanchien247247@gmail.com");
            mail.To.Add(getUser());
            mail.Subject = "Confirm you order";
            mail.Body = "\nThis is automatic email, please don't reply it !";
            mail.Body += "\nYour order ID: " + IdBill.ToString();
            foreach (var item in list)
            {
                mail.Body += "\n\nProduct: " + context.Products.Where(p => p.id.Equals(item.id)).Select(p => p.name).FirstOrDefault();
                mail.Body += "\nAmount: " + item.Amount.ToString();
                mail.Body += "\nPrice: " + String.Format(CultureInfo.CurrentCulture, "{0:C0}", ((long)item.Amount * (long)context.Products.Where(p => p.id.Equals(item.id)).Select(p => p.price).FirstOrDefault())).ToString();
            }
            mail.Body += "\n\nTotal: " + String.Format(CultureInfo.CurrentCulture, "{0:C0}", context.Bills.Where(p => p.idBill.Equals(IdBill)).Select(p => p.Total).FirstOrDefault()).ToString();

            smtpClient.Port = 587;
            smtpClient.Credentials = new System.Net.NetworkCredential("nguyenvanchien247247@gmail.com", "nvc123456");
            smtpClient.EnableSsl = true;
            smtpClient.Send(mail);
        }
        private String GetMD5(string txt)
        {
            String str = "";
            Byte[] buffer = Encoding.UTF8.GetBytes(txt);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            buffer = md5.ComputeHash(buffer);
            foreach (Byte b in buffer)
            {
                str += b.ToString("X2");
            }
            return str;
        }



        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [ActionName("ChangePass")]
        public ActionResult ChangePass_Get()
        {
            return View();
        }
        [HttpPost]
        [ActionName("ChangePass")]
        public ActionResult ChangePass_Post(FormCollection frm)
        {
            var user = getUser();
            User usr = context.Users.Where(p => p.Email.Equals(user)).FirstOrDefault();
            var md5Oldpass = GetMD5(frm.Get("oldpass"));
            if (!usr.Password.Equals(md5Oldpass))
            {
                ModelState.AddModelError("", "Password does not match");
                return View();
            }
            if(!frm.Get("newpass").Equals(frm.Get("renewpass")))
            {
                ModelState.AddModelError("", "New password and Re-new does not match");
                return View();
            }    
            usr.Password = GetMD5(frm.Get("newpass"));
            context.Entry(usr).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
            return RedirectToAction("Logout");
        }
        public ActionResult ShoppingCart()
        {
            if (!CheckLogin())
                return RedirectToAction("Login");

            var usr = getUser();
            ViewBag.ShoppingCart = context.ShoppingCarts.Join(
                context.Products,
                p=>p.id,
                q=>q.id,
                (p,q)=>new {p,q}
                )
                .Where(p => p.p.Email.Equals(usr))
                .Select(p => new ShoppingCartVM
                {
                    id = p.q.id,
                    name = p.q.name,
                    image = p.q.image,
                    price = p.q.price,
                    Amount = p.p.Amount,
                })
                .ToList();
            ViewBag.User = context.Users.Where(p => p.Email.Equals(usr)).FirstOrDefault();
            return View();
        }
        public ActionResult AddToCart(int id)
        {
            var email = getUser();
            ShoppingCart sc = context.ShoppingCarts.Where(p => p.Email.Equals(email) && p.id.Equals(id)).FirstOrDefault();
            if(sc==null)
            {
                sc = new ShoppingCart();
                sc.Email = email;
                sc.id = id;
                sc.Amount = 1;
                context.ShoppingCarts.Add(sc);
                context.SaveChanges();
            }
            else
            {
                sc.Amount++;
                context.Entry(sc).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            return RedirectToAction("ListProduct");
        }
        public ActionResult RemoveCart(int id)
        {
            var user = getUser();
            ShoppingCart spc = context.ShoppingCarts.Where(p => p.Email.Equals(user) && p.id.Equals(id)).FirstOrDefault();
            if(spc!=null)
            {
                context.ShoppingCarts.Remove(spc);
                context.SaveChanges();
            }
            return RedirectToAction("ShoppingCart");
        }
        [HttpPost]
        public ActionResult UpdateCart(FormCollection frm)
        {
            var user = getUser();
            var id = int.Parse(frm.Get("id"));
            ShoppingCart spc = context.ShoppingCarts.Where(p => p.Email.Equals(user) && p.id.Equals(id)).FirstOrDefault();
            if(spc!=null)
            {
                spc.Amount = int.Parse(frm.Get("amount"));
                context.Entry(spc).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            return RedirectToAction("ShoppingCart");
        }
        public ActionResult ListProduct()
        {
            List<Product> list = context.Products.ToList();
            ViewBag.categories = context.Categories.ToList();
            return View(list);
        }
        public ActionResult HistoryCart()
        {
            var user = getUser();
            ViewBag.HistoryCart = context.Bills.Where(p => p.Email.Equals(user)).ToList();
            return View();
        }
        public ActionResult DetailProduct(int id)
        {
            Product pd = context.Products.FirstOrDefault(p => p.id == id);
            return View(pd);
        }
        public ActionResult UserDashBoard()
        {
            if(!CheckLogin())
            {
                return RedirectToAction("Login");
            }
            else
            {
                var usremail = getUser();
                User usr = context.Users.Where(p => p.Email.Equals(usremail)).FirstOrDefault();
                return View(usr);
            }
        }
        [HttpPost]
        public ActionResult Update(User user)
        {
            if(!ModelState.IsValidField("Phone"))
            {
                return View();
            }    
            var usermail = getUser();
            User usr = context.Users.Where(p => p.Email.Equals(usermail)).FirstOrDefault();
            usr.Name = user.Name.Trim();
            usr.Phone = user.Phone.Trim();
            usr.Address = user.Address.Trim();
            context.Entry(usr).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
            return RedirectToAction("UserDashBoard");
        }

        public ActionResult productManager()
        {
            List<Product> list = context.Products.ToList();
            ViewBag.lst = list;
            ViewBag.Categories = context.Categories.ToList();
            return View();
        }
        
        [HttpPost]
        public ActionResult Add(Product product, HttpPostedFileBase file)
        {

            if (file != null && file.ContentLength > 0)
            {
                SavefileToServer(file);
                product.image = String.Concat("/app/img/Giay/", Path.GetFileName(file.FileName));
                context.Products.Add(product);
                context.SaveChanges();
            }
            return RedirectToAction("productManager");
        }
        public ActionResult deleteProduct(int id)
        {
            Product pd = context.Products.FirstOrDefault(p => p.id == id);

            context.Products.Remove(pd);
            context.SaveChanges();

            return RedirectToAction("productManager");
        }
        public ActionResult editProduct(int id)
        {
            Product pd = context.Products.FirstOrDefault(p => p.id == id);
            return View(pd);
        }

        [HttpPost]
        public ActionResult Edit(Product product, HttpPostedFileBase file)
        {

            Product pd = context.Products.FirstOrDefault(p => p.id == product.id);

            pd.name = product.name.Trim();
            pd.price = product.price;
            pd.descriptions = product.descriptions.Trim();
            pd.image = product.image;

            context.SaveChanges();
            return RedirectToAction("productManager");
        }
        public ActionResult Filter(int id)
        {
            ViewBag.categories = context.Categories.ToList();
            return View("ListProduct", context.Products.Where(p => p.categoryId.Equals(id)).ToList());
        }
        [HttpPost]
        public ActionResult Find(string txtKeyWord)
        {
            ViewBag.categories = context.Categories.ToList();
            var list = context.Products.Where(p => p.descriptions.Contains(txtKeyWord)).ToList();
            return View("ListProduct", list);
        }

        [HttpGet]
        [ActionName("Login")]
        public ActionResult Login_get()
        {
            return View();
        }
        [HttpPost]
        [ActionName("Login")]
        public ActionResult Login_Post(User user)
        {
            if (!ModelState.IsValidField("Email"))
            {
                return View();
            }
            var passMD5 = GetMD5(user.Password);
            var login = context.Users.Where(p => p.Email.Equals(user.Email) && p.Password.Equals(passMD5)).FirstOrDefault();
            if(login!=null)
            {
                HttpCookie cookie = new HttpCookie("user", user.Email.ToString());
                cookie.Expires.AddHours(8);
                HttpContext.Response.SetCookie(cookie);
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = "Email hoac mat khau khong dung";
                return View();
            }
        }
        [HttpGet]
        [ActionName("Register")]
        public ActionResult Register_get()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Register")]
        public ActionResult Register_post(User user)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            if (context.Users.Where(p => p.Email.Equals(user.Email)).FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Email have been used!");
                return View();
            }
            user.Password = GetMD5(user.Password);
            context.Users.Add(user);
            context.SaveChanges();
            return RedirectToAction("Login");
        }
        public ActionResult Logout()
        {
            var c = new HttpCookie("user");
            c.Expires = DateTime.Now;
            Response.Cookies.Add(c);
            return RedirectToAction("Index");
        }
        public ActionResult Checkout()
        {
            var user = getUser();
            var list = context.ShoppingCarts.Where(p => p.Email.Equals(user)).ToList();
            if(list.Count==0)
            {
                return RedirectToAction("ListProduct");
            }
            var total = 0L;
            Bill b = new Bill();
            b.Email = user;
            foreach(var item in list)
            {
                total += (long)item.Amount * (long)context.Products.Where(p => p.id.Equals(item.id)).Select(p => p.price).FirstOrDefault();
            }
            b.Total = total;
            b.Status = false;
            b.Date = System.DateTime.Now;
            context.Bills.Add(b);
            context.SaveChanges();

            foreach(var item in list)
            {
                BillDetail bd = new BillDetail();
                bd.idBill = b.idBill;
                bd.id = item.id;
                bd.amount = item.Amount;
                context.BillDetails.Add(bd);
                context.SaveChanges();
            }

            var sc = context.ShoppingCarts.Where(p => p.Email.Equals(user)).ToList();
            foreach(var item in sc)
            {
                context.ShoppingCarts.Remove(item);
                context.SaveChanges();
            }
            SendMail(list, b.idBill);


            return RedirectToAction("HistoryCart");
        }
        public ActionResult BillManager()
        {
            var list = context.Bills.Join(
                        context.Users,
                        b => b.Email,
                        u => u.Email,
                        (b, u) => new { b, u }
                    ).Select(
                        p => new BillManagerVM
                        {
                            Address = p.u.Address,
                            Name = p.u.Name,
                            idBill = p.b.idBill,
                            Date = p.b.Date,
                            Total = p.b.Total,
                            Status = p.b.Status
                        }
                    ).ToList();
            return View(list);
        }
        public ActionResult Complete(int idBill)
        {
            var bill = context.Bills.Where(p => p.idBill.Equals(idBill)).FirstOrDefault();
            if(bill!=null)
            {
                bill.Status = true;
                context.Entry(bill).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            return RedirectToAction("BillManager");
        }
        public ActionResult UserManager()
        {
            var list = context.Users.ToList();
            return View(list);
        }
        public ActionResult DeleteUser(string uEmail)
        {
            User u = context.Users.Where(p => p.Email.Equals(uEmail)).FirstOrDefault();
            if(u!=null)
            {
                context.Users.Remove(u);
                context.SaveChanges();
            }
            return RedirectToAction("UserManager");
        }
        public ActionResult UserFilter(string txtSearch)
        {
            var list = context.Users.Where(p => p.Address.Contains(txtSearch) || p.Email.Contains(txtSearch) || p.Name.Contains(txtSearch) || p.Phone.Contains(txtSearch)).ToList();
            return View("UserManager", list);
        }
    }
}