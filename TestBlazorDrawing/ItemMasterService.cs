namespace TestBlazorDrawing;

public class ItemMasterService
{
    public Task<ItemMaster[]> GetItemMasters()
    {
        var rng = new Random();
        int ivale = 0;
        return Task.FromResult(Enumerable.Range(1, 5).Select(index => new ItemMaster
        {
            ItemName = "itm" + rng.Next(1, 100),
            SaleCount = rng.Next(20, 100),
        }).ToArray()); ;
    }
}

public class ItemMaster
{
    public string ItemName { get; set; }
    public int SaleCount { get; set; }
}

