
//Create muna ng Properties
class Food
{
    public int RemainingStock {get; set;}
    public int Id {get; set;}

    public string Name {get; set;}

    public double Price {get; set;}

//Create na ng mga Constructors to access
public Food(int remainingStock, int id, string name, double price)
{
    RemainingStock = remainingStock;
    Id = id;
    Name = name;
    Price = price; 
    
}

public void DisplayFood()
    {
        //maglagay ng mga ididsplay na mga content for example name, price, stock at Id 
        // 25 meaning characters wide to the left and yung 8 is kung ilan na product
        Console.WriteLine($"[{Id}] {Name, -25} PHP {Price, 8:F2} Stock {RemainingStock}");
    }







}





class Program
{
    static void Main(string[] args)
    {
        // ── Food Menu ──────────────────────────────────────────────────
        Food[] menu = new Food[]
        {
            new Food(1, "Pizza",           150.00, 5),
            new Food(2, "Chicken_Burger",     250.00, 20),
            new Food(3, "Cheese_Burger",200.00, 10),
            new Food(4, "Fried_Chicken",         100.00, 15),
            new Food(5, "Sphagetti",   150.00, 3),
            new Food(6, "Carbonara",         200.00, 8),
            new Food(7, "Hawaiyan_Pizza",           500.00, 12),
            new Food(8, "Onion_Rings",  150.00, 7),
        };
    }
}
