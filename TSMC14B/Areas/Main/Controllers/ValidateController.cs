using System.Linq;
using System.Web.Mvc;
using WebCMS.Areas.Main.Models;

namespace WebCMS.Areas.Main.Controllers
{
    public class ValidateController : Controller
    {
        private tsmc14BDataContext db = new tsmc14BDataContext();

        // 驗證代碼是否存在

        public JsonResult CheckUserName(string UserName)
        {
            //return Json(!(LoginModel.Get(UserName )!= null), JsonRequestBehavior.AllowGet);
            return Json(!db.login_info.Any(user => user.login_name == UserName), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckUserPwd(string UserName, string Password2)
        {
            bool isValidate = false;
            //利用 IsLocalUrl檢查是否為網站呼叫的
            //借此忽略一些不必要的流量
            if (Url.IsLocalUrl(Request.Url.AbsoluteUri))
                if (string.IsNullOrEmpty(UserName) || LoginModel.Get(UserName, Password2) == null)
                    isValidate = true;
            
            // Remote 驗證是使用 Get 因此要開放
            return Json(isValidate, JsonRequestBehavior.AllowGet);
        }
    }
}
