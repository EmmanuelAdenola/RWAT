using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using MongoDB.Driver.Linq;
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
          
            //check if user exists
            var isdetailsvalid = MongoHelper.GetCollection<Account>("accounts").AsQueryable().Any(a=>a.UserName == loginModel.UserName);
            
            if(!isdetailsvalid)
            {
                ModelState.AddModelError("","Invalid details");
                return View(loginModel);
            }
            //set cookie
            FormsAuthentication.SetAuthCookie(loginModel.UserName,true);
            
            //send redirect url to client
            return new JsonResult { Data = new { success = "Home/Index" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }


        [HttpPost]
        public ActionResult Register(RegisterModel  registerModel)
        {
            if(!ModelState.IsValid)
            {
                return View("Register", registerModel);
            }
            //check if user name is in use
            var accounts = MongoHelper.GetCollection<Account>("accounts");
            bool userNameInuse = accounts.AsQueryable().Any(a=>a.UserName == registerModel.UserName);
            
            if (!userNameInuse)
            {
                //create new user
                User user = new User { UserName = registerModel.UserName ,HashedEmail =HashHelper.GetHash(registerModel.Email)};
             
                //create new accout 
                Account account = new Account
                {
                    Password = HashHelper.GetHash(registerModel.Password),
                    UserName = registerModel.UserName,
                    Email = registerModel.Email
                };
               
                MongoHelper.GetCollection<User>("users").Save(user);

                var result = accounts.Save(account);

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
}
