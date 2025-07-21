namespace Domain.Entities.Core;

public class MarketLevel
{
    public Instrument Instrument { get; set; } = null!;
    public double Price { get; set; }
    public double Volume { get; set; }
    public LevelDirection Direction { get; set; }
}

public enum LevelDirection
{
    Buy,
    Sell
}