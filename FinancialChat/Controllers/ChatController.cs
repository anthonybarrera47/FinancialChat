using FinancialChat.Data;
using FinancialChat.Hubs;
using FinancialChat.Models;
using FinancialChat.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FinancialChat.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHubContext<ChatHub> _chatHubContext;
        private readonly IMessageQueueService _messageQueueService;


        public ChatController(ApplicationDbContext context, UserManager<IdentityUser> userManager,IHubContext<ChatHub> hubContext, IMessageQueueService messageQueueService)
        {
            _context = context;
            _userManager = userManager;
            _chatHubContext = hubContext;
            _messageQueueService = messageQueueService;
        }

        public IActionResult Index()
        {
            var messages = _context.ChatMessages
                                   .OrderByDescending(m => m.Timestamp)
                                   .Take(50)
                                   .OrderBy(m => m.Timestamp)
                                   .ToList();
            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            if (!string.IsNullOrEmpty(request.Message))
            {
                var user = await _userManager.GetUserAsync(User);

                if (request.Message.StartsWith("/stock="))
                {
                    _messageQueueService.Publish(request.Message);
                    await _chatHubContext.Clients.All.SendAsync("ReceiveMessage", user.UserName, request.Message);
                }
                else
                {
                    var chatMessage = new ChatMessage
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        Message = request.Message,
                        Timestamp = DateTime.Now,
                        IsStockCommand = false
                    };

                    _context.ChatMessages.Add(chatMessage);
                    await _context.SaveChangesAsync();

                    await _chatHubContext.Clients.All.SendAsync("ReceiveMessage", user.UserName, request.Message);
                }
            }

            return Ok();
        }
        public class SendMessageRequest
        {
            public string Message { get; set; }
        }
    }
}
