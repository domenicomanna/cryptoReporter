namespace Api.Controllers.Users.Common;

public class UserDTO
{
    public int Id { get; set; }
    public string Email { get; set; } = String.Empty;
    public string FiatCurrencyTypeName { get; set; } = string.Empty;
}
