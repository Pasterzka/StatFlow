namespace Shared.User.Response
{
    // zwracanie informacji o uzytkowniku
    public record class GetUserResponse(
        Guid UserId,
        string firstName,
        string lastName,
        string email
    );
}