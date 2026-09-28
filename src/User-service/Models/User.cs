namespace User_service.Models
{
    public class User
    {   
        // ID
        public Guid UserId { get; set; }

        // Dane użytkownika
        public string email { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty; // hasło bez zabezpieczeń narazie
        public string firstName { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;
        public string phoneNumber { get; set; } = string.Empty;
        public string country { get; set; } = string.Empty;

        // Data utworzenia
        public DateTime createdAt { get; set; } = DateTime.UtcNow;


    }
}
