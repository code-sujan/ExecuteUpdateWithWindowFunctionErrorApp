namespace ErrorSampleApp.Entity;

public class PurchaseOrder
{
    public long Id { get; set; }
    public virtual Product Product { get; set; }
    public long ProductId { get; set; }
    public virtual Unit Unit { get; set; }
    public long UnitId { get; set; }
    public long Quantity { get; set; }
    public decimal LastCp { get; set; }
    public decimal LastCpWithoutVat { get; set; }
    public decimal NetAmount { get; set; }
}