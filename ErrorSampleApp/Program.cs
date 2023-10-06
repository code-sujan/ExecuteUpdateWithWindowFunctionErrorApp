using ErrorSampleApp;
using ErrorSampleApp.Entity;
using Microsoft.EntityFrameworkCore;

class Program
{
    static void Main()
    {
        using var context = new AppDbContext();
        context.SaveChanges();
        if (!context.Products.Any()) SeedInitialData(context);
        
        var orderQueryWithoutJoin = from item in context.PurchaseOrders
            join cp in GetLastCpWithoutJoin(context) on new { item.ProductId, item.UnitId } equals new { cp.ProductId, cp.UnitId }
            select new
            {
                item,
                Rate = cp.Rate
            };
        orderQueryWithoutJoin.ExecuteUpdate(setters =>
            setters.SetProperty(x => x.item.LastCp, x => x.Rate)
                .SetProperty(x => x.item.NetAmount, x => x.item.Quantity * x.Rate)
        ); // This works too
        
        var orderQueryWithJoin = from item in context.PurchaseOrders
            join cp in GetLastCp(context) on new { item.ProductId, item.UnitId } equals new { cp.ProductId, cp.UnitId }
            select new
            {
                item,
                Rate = cp.Rate,
                RateWithoutVat = cp.RateWithoutVat
            };
        var list = orderQueryWithJoin.ToList(); // This works fetching rate info
        
        orderQueryWithJoin.ExecuteUpdate(setters =>
            setters.SetProperty(x => x.item.LastCp, x => x.Rate)
                .SetProperty(x => x.item.LastCpWithoutVat, x => x.RateWithoutVat)
                .SetProperty(x => x.item.NetAmount, x => x.item.Quantity * x.Rate)
            ); // This Fails
        Console.WriteLine("Completed with success");
    }

    static IQueryable<LastCpDto> GetLastCp(AppDbContext context)
    {
        var res = from item in context.ProductUnitLinks
            from cp in context.ProductCps.Include(x => x.Product).Where(x => x.ProductId == item.ProductId && x.UnitId == item.UnitId)
                .OrderByDescending(x => x.Date)
                .Take(1)
            select new LastCpDto
            {
                ProductId = item.ProductId,
                UnitId = item.UnitId,
                Rate = cp.Rate,
                RateWithoutVat = cp.Rate > 0 ? Math.Round((cp.Product.VatPercent > 0) ? (cp.Rate - (cp.Product.VatPercent/100 * cp.Rate)) : cp.Rate, 3) : 0
            };
        return res;
    }
    
    static IQueryable<LastCpDto> GetLastCpWithoutJoin(AppDbContext context)
        {
            var res = from item in context.ProductUnitLinks
                from cp in context.ProductCps.Where(x => x.ProductId == item.ProductId && x.UnitId == item.UnitId)
                    .OrderByDescending(x => x.Date)
                    .Take(1)
                select new LastCpDto
                {
                    ProductId = item.ProductId,
                    UnitId = item.UnitId,
                    Rate = cp.Rate,
                };
            return res;
        }

    static void SeedInitialData(AppDbContext context)
    {
        var unit = new Unit() { Name = "Pcs" };
        var products = new List<Product>();
        var links = new List<ProductUnitLink>();
        var rates = new List<ProductCp>();
        var orders = new List<PurchaseOrder>();
        for (var i = 1; i < 1000; i++)
        {
            var product = new Product() { Name = $"Product {i}", VatPercent = i % 2 == 0 ? 0 : 13 };
            products.Add(product);
            links.Add(new ProductUnitLink() { Product = product, Unit = unit });
            rates.Add(new ProductCp { Date = DateTime.UtcNow, Product = product, Unit = unit, Rate = 100 });
            orders.Add(new PurchaseOrder { Product = product, Unit = unit, Quantity = 10, LastCp = 0 });
        }

        context.Add(unit);
        context.AddRange(products);
        context.AddRange(links);
        context.AddRange(rates);
        context.AddRange(orders);
        context.SaveChanges();
    }
}

internal class LastCpDto
{
    public long ProductId { get; set; }
    public long UnitId { get; set; }
    public decimal Rate { get; set; }
    public decimal RateWithoutVat { get; set; }
}