using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMessageLogger.DataAccess;
using MvcMessageLogger.Models;

namespace MvcMessageLogger.Controllers
{
	public class UsersController : Controller
	{

		private readonly MvcMessageLoggerContext _context;

		public UsersController(MvcMessageLoggerContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			var users = _context.Users;
			return View(users);
		}

		public IActionResult New()
		{
			return View();
		}

		[HttpPost]
		[Route("/users")]
		public IActionResult Create(User user)
		{
			_context.Users.Add(user);
			_context.SaveChanges();

			return Redirect("/users");
		}

		public IActionResult Statistics()
		{
			var users = _context.Users.Include(u => u.Messages);
			ViewData["HourWithMostMessages"] = StatsHelper.HourWithMostMessages(_context);
			ViewData["MostCommonWord"] = StatsHelper.MostCommonWord(_context);
			return View(users);
		}
	}
}
