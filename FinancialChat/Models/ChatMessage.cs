using System.ComponentModel.DataAnnotations;

namespace FinancialChat.Models
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }
        public bool IsStockCommand { get; set; } 
    }
}
