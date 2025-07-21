namespace Domain.Interfaces;

public interface IQuotationService
{
    public Task InitAsync(CancellationToken token = default);
    public Task RunAsync(CancellationToken token = default);
}