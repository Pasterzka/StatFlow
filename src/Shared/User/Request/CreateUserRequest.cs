namespace Shared.User
{
    // Tworzenie uzytkownika w bazie danych
    public record CreateUserRequest(
        string firstName,
        string lastName,
        string email,
        string password,
        string addres,
        string phoneNumber,
        string country
        );
}
