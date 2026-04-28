using System;


//Create muna ng Properties
class Food
{
    public int RemainingStock {get; set;}
    public int Id {get; set;}

    public string Name {get; set;}

    public double Price {get; set;}


//Create na ng mga Constructors to access
public Food(int id, string name, double price, int remainingStock)
    {
        Id = id;
        Name = name;
        Price = price;
        RemainingStock = remainingStock;
    }

public void DisplayFood()
    {
        //maglagay ng mga ididsplay na mga content for example name, price, stock at Id 
        // 25 meaning characters wide to the left and yung 8 is kung ilan na product
        Console.WriteLine($"[{Id}] {Name, -25} PHP {Price, 8:F2} Stock {RemainingStock}");
    }


//para ma add yung initial quantity plus price

public double GetItemTotal(int quantity)
    {
        return Price * quantity;
    }


//dito naman ichecheck kung may enough pa ba na stock 
public bool HasEnoughStock(int quantity)
    {
        return RemainingStock >= quantity;
    }


//mag dededuct everytime na nagbabawas tayo ng item sa Food Store
public void DeductStock(int quantity)
    {
        RemainingStock -= quantity;
    }

}

class CartItem
{
    public Food Product { get; set; }
    public int Quantity { get; set; }
    public double Subtotal { get; set; }

    public CartItem(Food product, int quantity)
    {
        Product = product;
        Quantity = quantity;
        Subtotal = product.GetItemTotal(quantity);
    }
}


class Program
{
    static void Main(string[] args)
    {
        //  Food Menu 
        Food[] menu = new Food[]
        {
            new Food(1, "Pizza",          150.00, 5),
            new Food(2, "Chicken Burger", 250.00, 20),
            new Food(3, "Cheese Burger",  200.00, 10),
            new Food(4, "Fried Chicken",  100.00, 15),
            new Food(5, "Spaghetti",      150.00, 3),
            new Food(6, "Carbonara",      200.00, 8),
            new Food(7, "Hawaiian Pizza", 500.00, 12),
            new Food(8, "Onion Rings",    150.00, 7),
        };

        //cart fixed size na pwedeng mabili on the store 3 lang yung nilagay ko para makita agad yung breaking 
        //kapag sobra na sa pag cacart
        const int MAX_CART = 3;
        CartItem[] cart = new CartItem[MAX_CART];
        int cartCount = 0;

        PrintHeader("Welcome to FoodStore");

        bool shopping = true;
        while (shopping)
        {
            // Display menu
            Console.WriteLine();
            PrintDivider('=', 55);
            Console.WriteLine("  STORE MENU");
            PrintDivider('=', 55);
            Console.WriteLine($"  {"#",-4} {"Food",-25} {"Price",10}   {"Stock",5}");
            PrintDivider('-', 55);

            foreach (var p in menu)
            {
                if (p.RemainingStock == 0)
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                p.DisplayFood();
                Console.ResetColor();
            }
            PrintDivider('=', 55);

            // Check yung cart capacity na kung ilan yung sinet natin and na meet nya na then  iistop na yung loop
            if (cartCount >= MAX_CART)
            {
                PrintWarning("Cart is full. Proceeding to checkout...");
                break;
            }

            //Food selection
            Console.Write("\n  Enter food product number (or 0 to checkout): ");
            string prodInput = Console.ReadLine() ?? "";
            int productId;

            //here dapat numeric value yung ilalagay ni user

            if (!int.TryParse(prodInput, out productId))
            {
                PrintError("Invalid input. Please enter a numeric food product number.");
                Pause();
                continue;
            }

            if (productId == 0) break;

            //validation here na kapag wala sa choices may lalabas na "invalid food product number"

            if (productId < 1 || productId > menu.Length)
            {
                PrintError($"Invalid food product number. Please choose between 1 and {menu.Length}.");
                Pause();
                continue;
            }

            Food selected = menu[productId - 1];

            // dito ichecheck yung out of stock tapos kapag nakita na bumili sila na out of stock na may validation
            if (selected.RemainingStock == 0)
            {
                PrintError($"Sorry, \"{selected.Name}\" is out of stock.");
                Pause();
                continue;
            }

            // Quantity input
            Console.Write($"  Enter quantity for {selected.Name} (available: {selected.RemainingStock}): ");
            string qtyInput = Console.ReadLine() ?? "";
            int qty;

            //dito naman kapag hindi naglagay si user ng numeric quantity for example ininput nya is Characters

            if (!int.TryParse(qtyInput, out qty))
            {
                PrintError("Invalid input. Please enter a numeric quantity.");
                Pause();
                continue;
            }

            //here kailangan mas greater than zero yung ilalagay

            if (qty <= 0)
            {
                PrintError("Quantity must be greater than zero.");
                Pause();
                continue;
            }

            //if ever na ubos na yung stock

            if (!selected.HasEnoughStock(qty))
            {
                PrintError($"Not enough stock. Only {selected.RemainingStock} left for \"{selected.Name}\".");
                Pause();
                continue;
            }

            //  Add to cart / update duplicate 
            bool found = false;
            for (int i = 0; i < cartCount; i++)
            {
                if (cart[i].Product.Id == selected.Id)
                {


                    if (!selected.HasEnoughStock(qty))
                    {
                        PrintError($"Cannot add {qty} more. Only {selected.RemainingStock - cart[i].Quantity} additional units available.");
                        found = true;
                        Pause();
                        break;
                    }

                    int newQty = cart[i].Quantity + qty;
                    cart[i].Quantity = newQty;
                    cart[i].Subtotal = selected.GetItemTotal(newQty);
                    selected.DeductStock(qty);
                    PrintSuccess($"Updated cart: {selected.Name} x{newQty}  (Subtotal: PHP {cart[i].Subtotal:F2})");
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                selected.DeductStock(qty);
                cart[cartCount] = new CartItem(selected, qty);
                cartCount++;
                PrintSuccess($"Added to cart: {selected.Name} x{qty}  (Subtotal: PHP {cart[cartCount - 1].Subtotal:F2})");
            }

            string cont;
            while (true)
            {

                Console.Write("\n  Add more items? (Y/N)");
                cont = (Console.ReadLine() ?? "").Trim().ToUpper();
                if (cont == "Y" || cont == "N") break;
                PrintError("Invalid input. Please enter Y or N.");

            }
            if (cont != "Y") shopping = false;

        // Receipt 
        Console.WriteLine();
        PrintDivider('=', 55);
        Console.WriteLine("  RECEIPT");
        PrintDivider('=', 55);

        if (cartCount == 0)
        {
            Console.WriteLine("  Your cart is empty. No purchase made.");
        }
        else
        {
            Console.WriteLine($"  {"Item",-26} {"Qty",4}  {"Unit Price",10}  {"Subtotal",10}");
            PrintDivider('-', 55);

            double grandTotal = 0;
            for (int i = 0; i < cartCount; i++)
            {
                CartItem ci = cart[i];
                Console.WriteLine($"  {ci.Product.Name,-26} {ci.Quantity,4}  PHP {ci.Product.Price,7:F2}  PHP {ci.Subtotal,7:F2}");
                grandTotal += ci.Subtotal;
            }

            PrintDivider('-', 55);
            Console.WriteLine($"  {"GRAND TOTAL:",-40} PHP {grandTotal,7:F2}");

            double discount  = 0;
            double finalTotal = grandTotal;

            if (grandTotal >= 5000)
            {
                discount   = grandTotal * 0.10;
                finalTotal = grandTotal - discount;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  {"10% Discount:",-40} PHP {discount,7:F2}");
                Console.ResetColor();
            }

            PrintDivider('=', 55);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"  {"FINAL TOTAL:",-40} PHP {finalTotal,7:F2}");
            Console.ResetColor();

            if (grandTotal >= 5000)
                PrintSuccess("Congratulations! You saved PHP " + discount.ToString("F2") + " with the 10% discount.");
        }

        // ── Update ng stock
        Console.WriteLine();
        PrintDivider('=', 55);
        Console.WriteLine("  UPDATED REMAINING STOCK");
        PrintDivider('=', 55);
        Console.WriteLine($"  {"#",-4} {"Food",-30} {"Remaining Stock",15}");
        PrintDivider('-', 55);

        foreach (var p in menu)
        {
            string stockLabel = p.RemainingStock == 0 ? "OUT OF STOCK" : p.RemainingStock.ToString();
            if (p.RemainingStock == 0) Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  [{p.Id}] {p.Name,-30} {stockLabel,15}");
            Console.ResetColor();
        }

        PrintDivider('=', 55);
        Console.WriteLine("\n  Thank you for shopping at FoodStore!\n");
    }

    //nagpa help ako kay ai na kung pwede lagyan nya ng color para makita agad ng user yung naging changes
    static void PrintHeader(string title)
    {
        PrintDivider('*', 55);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"  {title}");
        Console.ResetColor();
        PrintDivider('*', 55);
    }



     // Ginagamit ko to para gumawa ng visual separator sa console output
     // Example: pang design ng menu, headings, or sections
    static void PrintDivider(char ch, int width)
    {
        Console.WriteLine(new string(ch, width));
    }


    static void PrintError(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  [ERROR] {msg}");
        Console.ResetColor();
    }

    static void PrintWarning(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  [!] {msg}");
        Console.ResetColor();
    }

    static void PrintSuccess(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  [✓] {msg}");
        Console.ResetColor();
    }

    static void Pause()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("  Press Enter to continue...");
        Console.ResetColor();
        Console.ReadLine();
    }
 }

}