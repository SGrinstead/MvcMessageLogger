using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using MvcMessageLogger.DataAccess;
using MvcMessageLogger.Models;

namespace MvcMessageLoggerTests
{
	[Collection("Controller Tests")]
	public class UsersControllerTests : IClassFixture<WebApplicationFactory<Program>>
	{
		private readonly WebApplicationFactory<Program> _factory;

		public UsersControllerTests(WebApplicationFactory<Program> factory)
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
		public async void Index_ReturnsViewOfAllUsers()
		{
			var context = GetDbContext();
			var client = _factory.CreateClient();

			User user1 = new User("Seth", "SGrin");
			User user2 = new User("John", "JMoney");

			context.Users.Add(user1);
			context.Users.Add(user2);
			context.SaveChanges();

			var response = await client.GetAsync("/users");
			var html = await response.Content.ReadAsStringAsync();

			response.EnsureSuccessStatusCode();
			Assert.Contains("Seth", html);
			Assert.Contains("SGrin", html);
			Assert.Contains("John", html);
			Assert.Contains("JMoney", html);
		}

		[Fact]
		public async Task New_ReturnsFormForNewUser()
		{
			var client = _factory.CreateClient();

			var response = await client.GetAsync("/users/new");
			var html = await response.Content.ReadAsStringAsync();

			response.EnsureSuccessStatusCode();
			Assert.Contains("New User", html);
			Assert.Contains("Create User", html);
			Assert.Contains("<form method=\"post\" action=\"/users\">", html);
			Assert.Contains("Name:", html);
			Assert.Contains("Username:", html);
		}

		[Fact]
		public async Task Create_AddsUserToDatabaseAndRedirectsToIndex()
		{
			var client = _factory.CreateClient();

			var formData = new Dictionary<string, string>
			{
				{ "Name", "Seth" },
				{ "Username", "SGrin" }
			};

			var response = await client.PostAsync("/users", new FormUrlEncodedContent(formData));
			var html = await response.Content.ReadAsStringAsync();

			response.EnsureSuccessStatusCode();
			Assert.Contains("<h1>Users</h1>", html);
			Assert.Contains("Seth", html);
			Assert.Contains("SGrin", html);
		}
	}
}