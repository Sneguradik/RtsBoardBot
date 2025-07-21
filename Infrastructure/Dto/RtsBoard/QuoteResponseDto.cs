namespace Infrastructure.Dto.RtsBoard;

public class QuoteResponseDto : BaseResponse
{
    public RtsBoardQuote Document { get; set; } = null!;
}