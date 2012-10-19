using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gobiner.DotNetRepl.Web.Wcf;

namespace Gobiner.DotNetRepl.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var guid = Guid.NewGuid();
            ReplConnection.Connections[guid] = new ReplConnection(guid);
            ViewBag.SessionID = guid;
            return View();
        }

        public ActionResult Execute(Guid session, string inp)
        {
            var repl = ReplConnection.Connections[session];
            var ret = ReplConnection.Connections[session].Execute(inp);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
    }
}
