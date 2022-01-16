using JobsityChatApp.Data;
using JobsityChatApp.Models;
using JobsityChatApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace JobsityChatApp.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IMessageService messageService;
    private readonly UserManager<ChatUser> userManager;

    public HomeController(IMessageService messageService, UserManager<ChatUser> userManager)
    {
        this.messageService = messageService;
        this.userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var currentUser = await this.userManager.GetUserAsync(User);
        ViewBag.CurrentUserName = currentUser.UserName;
        var messages = await this.messageService.GetMessages()
                                .OrderByDescending(x => x.Created)
                                .Take(50)
                                .ToListAsync();

        return View(messages);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
