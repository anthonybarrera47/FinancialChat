using FinancialChat;
using FinancialChat.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;

namespace FinancialChatIntegrationTest
{
    public class ChatControllerIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public ChatControllerIntegrationTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task SendMessage_Should_Return_OkResult()
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                _ = builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                    });

                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                    var userManager = scopedServices.GetRequiredService<UserManager<IdentityUser>>();
                    db.Database.EnsureCreated();
                    SeedDatabase(db, userManager).Wait();
                });
            }).CreateClient();

            var messageContent = new StringContent("{\"Message\":\"Hello\"}", Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/Chat/SendMessage", messageContent);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        private static async Task SeedDatabase(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            var user = new IdentityUser { UserName = "testuser", Email = "test@example.com" };
            await userManager.CreateAsync(user, "Test@123");
            await dbContext.SaveChangesAsync();
        }
    }
}
