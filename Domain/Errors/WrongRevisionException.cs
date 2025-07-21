namespace Domain.Errors;

public class WrongRevisionException : Exception
{
    public int Revision { get; set; }
    public string DealId { get; set; } = string.Empty;
}