namespace ErrorSampleApp.Entity;

public class ProductCp
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public virtual Product Product { get; set; }
    public long ProductId { get; set; }
    public virtual Unit Unit { get; set; }
    public long UnitId { get; set; }
    
    public decimal Rate { get; set; }
}