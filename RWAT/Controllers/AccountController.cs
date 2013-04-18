using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using RWAT.Models;
using RWAT.Utility;

namespace RWAT.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpGet]
        [ChildActionOnly]
        public ActionResult Login()
        {
            return View();
        }
        [HttpGet]
        [ChildActionOnly]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel  loginModel)
        {
            if(!ModelState.IsValid)
            {
                return View(loginModel);
            }
            var conn =
               new MongoConnectionStringBuilder(
                   ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString);

            MongoClient mongoClient = new MongoClient(conn.ConnectionString);
            MongoServer mongoServer = mongoClient.GetServer();
            MongoDatabase mongoDatabase = mongoServer.GetDatabase(conn.DatabaseName);

            var isdetailsvalid = mongoDatabase.GetCollection<Account>("accounts").AsQueryable().Any(a=>a.UserName == loginModel.UserName);
            
            if(!isdetailsvalid)
            {
                ModelState.AddModelError("","Invalid details");
                return View(loginModel);
            }
            FormsAuthentication.SetAuthCookie(loginModel.UserName,true);
            return new JsonResult { Data = new { success = "Home/Index" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        [HttpPost]
        public ActionResult Register(RegisterModel  registerModel)
        {
            if(!ModelState.IsValid)
            {
                return View("Register", registerModel);
            }

            var conn =
                 new MongoConnectionStringBuilder(
                     ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString);

            MongoClient mongoClient = new MongoClient(conn.ConnectionString);
            MongoServer mongoServer = mongoClient.GetServer();
            MongoDatabase mongoDatabase = mongoServer.GetDatabase(conn.DatabaseName);
            bool userNameInuse = mongoDatabase.GetCollection("accounts").AsQueryable<Account>().Any(a=>a.UserName == registerModel.UserName);
            
            if (!userNameInuse)
            {
                User user = new User { UserName = registerModel.UserName ,HashedEmail =HashHelper.GetHash(registerModel.Email)};
                Account account = new Account { Password = HashHelper.GetHash(registerModel.Password), 
                    UserName = registerModel.UserName,Email = registerModel.Email};
                mongoDatabase.GetCollection<User>("users").Save(user);

                var result = mongoDatabase.GetCollection<Account>("accounts").Save(account);

                if (result.Ok)
                {
                    FormsAuthentication.SetAuthCookie(registerModel.UserName, true);
                }
                else
                {
                    ModelState.AddModelError("",result.ErrorMessage);
                    return View(registerModel);
                }
            }
            else
            {
                ModelState.AddModelError("","Username is already taken.");
                return View(registerModel);
            }

            return new JsonResult{Data = new {success="Home/Index"},JsonRequestBehavior=JsonRequestBehavior.AllowGet};
        }


        [Authorize]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }

    public class RegisterModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }
    }

    public class LoginModel  
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
