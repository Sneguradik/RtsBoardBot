namespace Infrastructure.Dto.RtsBoard;

public record MutateQuoteDto(string Id, int Revision);

public record UpdateQuoteDto(string Id, int Revision, RtsBoardQuote Document);