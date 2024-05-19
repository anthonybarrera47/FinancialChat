using FinancialChat.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FinancialChat.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Home/Error")]
        public IActionResult Error()
        {
            var statusCode = HttpContext.Response.StatusCode;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, StatusCode = statusCode });
        }
    }
}
