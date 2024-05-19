    using FinancialChat.Hubs;
    using FinancialChat.Services.Interfaces;
    using Microsoft.AspNetCore.SignalR;

    namespace FinancialChat.Services
    {
        public class StockBotService : BackgroundService
        { 
            private readonly IMessageQueueService _messageQueueService;
            private readonly IHubContext<ChatHub> _chatHubContext;
            private readonly HttpClient _httpClient;

            public StockBotService(IMessageQueueService messageQueueService, IHubContext<ChatHub> chatHubContext, HttpClient httpClient)
            {
                _messageQueueService = messageQueueService;
                _chatHubContext = chatHubContext;
                // Configure HttpClient to ignore SSL certificate errors
                var clientHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                _httpClient = new HttpClient(clientHandler)
                {
                    BaseAddress = new Uri("https://stooq.com")
                };
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
                var response = await _httpClient.GetAsync($"https://stooq.com/q/l/?s={stockCode}&f=sd2t2ohlcv&h&e=csv");
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
