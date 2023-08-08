using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using MvcMessageLogger.DataAccess;
using MvcMessageLogger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcMessageLoggerTests
{
	[Collection("Controller Tests")]
	public class MessagesControllerTests : IClassFixture<WebApplicationFactory<Program>>
	{
		private readonly WebApplicationFactory<Program> _factory;

		public MessagesControllerTests(WebApplicationFactory<Program> factory)
		{
			_factory = factory;
		}

		private MvcMessageLoggerContext GetDbContext()
		{
			var optionsBuilder = new DbContextOptionsBuilder<MvcMessageLoggerContext>();
			optionsBuilder.UseInMemoryDatabase("TestDatabase");

			var context = new MvcMessageLoggerContext(optionsBuilder.Options);
			context.Database.EnsureDeleted();
			context.Database.EnsureCreated();

			return context;
		}

		[Fact]
		public async Task Index_ReturnsViewWithAllMessagesForASpecificUser()
		{
			var context = GetDbContext();
			var client = _factory.CreateClient();

			User user1 = new User("Seth", "SGrin");
			User user2 = new User("John", "JMoney");

			Message message1 = new Message("Message 1");
			Message message2 = new Message("Message 2");
			Message message3 = new Message("Message 3");

			user1.Messages.Add(message1);
			user2.Messages.Add(message2);
			user2.Messages.Add(message3);

			context.Users.Add(user1);
			context.Users.Add(user2);
			context.SaveChanges();

			var response = await client.GetAsync($"/users/{user2.Id}/messages");
			var html = await response.Content.ReadAsStringAsync();

			response.EnsureSuccessStatusCode();
			Assert.Contains("JMoney's Messages", html);
			Assert.Contains("Message 2", html);
			Assert.Contains("Message 3", html);
			Assert.DoesNotContain("Message 1", html);
		}

		[Fact]
		public async Task New_ReturnsFormToCreateMessage()
		{
			var context = GetDbContext();
			var client = _factory.CreateClient();

			User user2 = new User("John", "JMoney");
			context.Users.Add(user2);
			context.SaveChanges();

			var response = await client.GetAsync($"/users/{user2.Id}/messages/new");
			var html = await response.Content.ReadAsStringAsync();

			response.EnsureSuccessStatusCode();
			Assert.Contains($"<form method=\"post\" action=\"/users/{user2.Id}/messages\">", html);
			Assert.Contains("Add a Message", html);
			Assert.Contains("Create Message", html);
		}
	}
}
