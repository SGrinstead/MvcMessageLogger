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

			return Redirect($"/users/{user.Id}/details");
		}

		[Route("/users/{userId:int}/messages/edit/{messageId:int}")]
		public IActionResult Edit(int userId, int messageId)
		{
			var user = _context.Users.Find(userId);
			ViewData["Message"] = _context.Messages.Find(messageId);

			return View(user);
		}

		[HttpPost]
		[Route("/users/{userId:int}/messages/update/{messageId:int}")]
		public IActionResult Update(int userId, int messageId, Message message)
		{
			message.Id = messageId;
			message.CreatedAt = DateTime.Now.ToUniversalTime();
			_context.Messages.Update(message);
			_context.SaveChanges();

			return Redirect($"/users/{userId}/details");
		}

		[Route("/users/{userId:int}/messages/delete/{messageId:int}")]
		public IActionResult Delete(int userId, int messageId)
		{
			var message = _context.Messages.Find(messageId);
			_context.Messages.Remove(message);
			_context.SaveChanges();

			return Redirect($"/users/{userId}/details");
		}
	}
}
