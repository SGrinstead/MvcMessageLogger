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

		[Route("/users/{userId:int}/details")]
		public IActionResult Show(int userId)
		{
			var user = _context.Users
				.Where(u => u.Id == userId)
				.Include(u => u.Messages)
				.FirstOrDefault();

			return View(user);
		}

		[Route("/users/edit/{userId:int}")]
		public IActionResult Edit(int userId)
		{
			var user = _context.Users.Find(userId);

			return View(user);
		}

		[HttpPost]
		[Route("/users/update/{userId:int}")]
		public IActionResult Update(int userId, User user)
		{
			user.Id = userId;
			_context.Users.Update(user);
			_context.SaveChanges();

			return Redirect($"/users/{user.Id}/details");
		}

		[Route("/users/delete/{userId:int}")]
		public IActionResult Delete(int userId)
		{
			var user = _context.Users
				.Where(u => u.Id == userId)
				.Include(u => u.Messages)
				.FirstOrDefault(); 

			_context.Remove(user);
			_context.SaveChanges();

			return Redirect("/users");
		}
	}
}
