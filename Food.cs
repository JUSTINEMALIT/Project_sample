using System;


//food.CS

//create ng food constructors

class Food
{
    public int    Id             { get; set; }
    public string Name           { get; set; }
    public string Category       { get; set; }
    public double Price          { get; set; }
    public int    RemainingStock { get; set; }

    public const int LOW_STOCK_THRESHOLD = 5;

    public Food(int id, string name, string category, double price, int remainingStock)
    {
        Id             = id;
        Name           = name;
        Category       = category;
        Price          = price;
        RemainingStock = remainingStock;
    }

    //  Display 
    public void DisplayFood()
    {
        Console.WriteLine($"  [{Id}] {Name,-25} {Category,-14} PHP {Price,8:F2}   Stock: {RemainingStock}");
    }

    // Stocks categories helpers 
    public double GetItemTotal(int quantity) => Price * quantity;
    public bool   HasEnoughStock(int qty)    => RemainingStock >= qty;
    public void   DeductStock(int qty)       => RemainingStock -= qty;
    public void   RestoreStock(int qty)      => RemainingStock += qty;
    public bool   IsLowStock()               => RemainingStock <= LOW_STOCK_THRESHOLD;
}