using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMessageLogger.DataAccess;

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
	}
}
