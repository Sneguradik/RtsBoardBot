using Domain.Entities.RTSBoard;

namespace Infrastructure.Dto.RtsBoard;

public class BaseResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public LocalizedText? Message { get; set; }
}
