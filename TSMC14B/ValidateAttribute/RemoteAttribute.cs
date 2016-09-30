using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TSMC14B.ValidateAttribute
{
    public class RemoteAttribute : System.Web.Mvc.RemoteAttribute
    {
        public RemoteAttribute(string action, string controller) : base(action, controller) { }

        public RemoteAttribute(string action, string controller, string area)
            : base(action, controller, area)
        {
            this.RouteData["area"] = area;
        }
    }
}