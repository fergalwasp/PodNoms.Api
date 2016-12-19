using Microsoft.AspNetCore.Mvc;

namespace PodNoms.Api.Controllers
{
    public class TemplateController : Controller
    {
        [Route("__t/{name}")]
        public ActionResult P(string name)
        {
            return PartialView(name);
        }
    }
}