using FinancialChat.Hubs;
using FinancialChat.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace FinancialChat.Services
{
    public class StockBotService : BackgroundService
    { 
        private readonly IMessageQueueService _messageQueueService;
        private readonly IHubContext<ChatHub> _chatHubContext;

        public StockBotService(IMessageQueueService messageQueueService, IHubContext<ChatHub> chatHubContext)
        {
            _messageQueueService = messageQueueService;
            _chatHubContext = chatHubContext;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _messageQueueService.Subscribe(async (message) =>
            {
                var stockCode = message.Split('=')[1];
                var stockQuote = await GetStockQuote(stockCode);
                var responseMessage = $"{stockCode.ToUpper()} quote is ${stockQuote} per share";

                await _chatHubContext.Clients.All.SendAsync("ReceiveMessage", "StockBot", responseMessage);
            });

            return Task.CompletedTask;
        }

        private async Task<string> GetStockQuote(string stockCode)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"https://stooq.com/q/l/?s={stockCode}&f=sd2t2ohlcv&h&e=csv");
            response.EnsureSuccessStatusCode();

            var csvContent = await response.Content.ReadAsStringAsync();
            using var reader = new StringReader(csvContent);
            reader.ReadLine(); // Skip header
            var line = reader.ReadLine();
            var values = line.Split(',');

            return values[3]; // Closing price
        }
    }
}
