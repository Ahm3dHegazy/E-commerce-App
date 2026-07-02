using Microsoft.AspNetCore.Mvc;

namespace CartFlow.Web.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet]
        [Route("Error/{statusCode}")]
        public IActionResult StatusCodeHandler(int statusCode)
        {
            // Return a custom 404 view for Not Found; fall back to a generic status view otherwise
            if (statusCode == 404)
            {
                Response.StatusCode = 404;
                return View("NotFound");
            }

            return View("StatusCode", statusCode);
        }
    }
}
