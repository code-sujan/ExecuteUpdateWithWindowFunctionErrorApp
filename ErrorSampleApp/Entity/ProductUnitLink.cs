namespace ErrorSampleApp.Entity;

public class ProductUnitLink
{
    public long Id { get; set; }
    public virtual Product Product { get; set; }
    public long ProductId { get; set; }
    
    public virtual Unit Unit { get; set; }
    public long UnitId { get; set; }
}