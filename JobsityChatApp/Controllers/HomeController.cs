using JobsityChatApp.Data;
using JobsityChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace JobsityChatApp.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly UserManager<ChatUser> userManager;

    public HomeController(UserManager<ChatUser> userManager)
    {
        this.userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var currentUser = await this.userManager.GetUserAsync(User);
        ViewBag.CurrentUserName = currentUser.UserName;
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
