using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMessageLogger.DataAccess;
using MvcMessageLogger.Models;

namespace MvcMessageLogger.Controllers
{
	public class MessagesController : Controller
	{
		private readonly MvcMessageLoggerContext _context;

		public MessagesController(MvcMessageLoggerContext context)
		{
			_context = context;
		}

		[Route("/users/{userId:int}/messages")]
		public IActionResult Index(int userId)
		{
			var user = _context.Users
				.Where(u => u.Id == userId)
				.Include(u => u.Messages)
				.FirstOrDefault();

			return View(user);
		}

		[Route("/users/{userId:int}/messages/new")]
		public IActionResult New(int userId)
		{
			var user = _context.Users
				.Where(u => u.Id == userId)
				.Include(u => u.Messages)
				.FirstOrDefault();

			return View(user);
		}

		[HttpPost]
		[Route("/users/{userId:int}/messages")]
		public IActionResult Create(Message message, int userId)
		{
			message.CreatedAt = DateTime.Now.ToUniversalTime();
			var user = _context.Users
				.Where(u => u.Id == userId)
				.Include(u => u.Messages)
				.FirstOrDefault();
			user.Messages.Add(message);
			_context.SaveChanges();

			return Redirect($"/users/{user.Id}/messages");
		}
	}
}
