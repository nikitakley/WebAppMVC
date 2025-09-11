using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace Kleimenov_AS_22_04.Controllers;

public class FirstController : Controller
{
    // 
    // GET: /First/
    public IActionResult Index()
    {
        return View();
    }

    // 
    // GET: /First/Welcome/ 
    public IActionResult Welcome(string name, int numTimes = 1)
    {
        ViewData["Message"] = "Hello" + name;
        ViewData["NumTimes"] = numTimes;
        return View();
    }
}
