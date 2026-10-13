using Microsoft.AspNetCore.Mvc;
using Restia.Web.Models;
using System.Diagnostics;

namespace Restia.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("Error/{statusCode?}")]
        public IActionResult Error(int? statusCode = null)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            var viewModel = new ErrorViewModel
            {
                RequestId = requestId,
                StatusCode = statusCode,
                ErrorMessage = statusCode == 404
                    ? "The page you are looking for was not found."
                    : "An unexpected error occurred on the server.",
                Timestamp = DateTimeOffset.UtcNow
            };

            return View(viewModel);
        }
    }
}
