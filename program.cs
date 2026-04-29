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
//para ma add yung initial quantity plus price
public double getItemTotal(int quantity)
    {
        return Price * quantity;
    }
public bool HasEnoughStock(int quantity)
    {
        return RemainingStock => quantity;
    }
public void DeductStock(int quantity)
    {
        RemainingStock -= quantity;
    }
}
class CartItem
{
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public double Subtotal { get; set; }
    public CartItem(Product product, int quantity)
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
        // ── Cart (fixed-size) ───────────────────────────────────────────
        const int MAX_CART = 10;
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
            Console.WriteLine($"  {"#",-4} {"Product",-25} {"Price",10}   {"Stock",5}");
            PrintDivider('-', 55);
            foreach (var p in menu)
            {
                if (p.RemainingStock == 0)
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                p.DisplayProduct();
                Console.ResetColor();
            }
            PrintDivider('=', 55);
            // Check cart capacity
            if (cartCount >= MAX_CART)
            {
                PrintWarning("Cart is full. Proceeding to checkout...");
                break;
            }
            // ── Product selection ────────────────────────────────────────
            Console.Write("\n  Enter product number (or 0 to checkout): ");
            string prodInput = Console.ReadLine();
            int productId;
            if (!int.TryParse(prodInput, out productId))
            {
                PrintError("Invalid input. Please enter a numeric product number.");
                Pause();
                continue;
            }
            if (productId == 0) break; // user wants to checkout
            if (productId < 1 || productId > menu.Length)
            {
                PrintError($"Invalid product number. Please choose between 1 and {menu.Length}.");
                Pause();
                continue;
            }
            Product selected = menu[productId - 1];
            // Out-of-stock check
            if (selected.RemainingStock == 0)
            {
                PrintError($"Sorry, \"{selected.Name}\" is out of stock.");
                Pause();
                continue;
            }
            // ── Quantity input ────────────────────────────────────────────
            Console.Write($"  Enter quantity for {selected.Name} (available: {selected.RemainingStock}): ");
            string qtyInput = Console.ReadLine();
            int qty;
            if (!int.TryParse(qtyInput, out qty))
            {
                PrintError("Invalid input. Please enter a numeric quantity.");
                Pause();
                continue;
            }
            if (qty <= 0)
            {
                PrintError("Quantity must be greater than zero.");
                Pause();
                continue;
            }
            // Stock availability check
            if (!selected.HasEnoughStock(qty))
            {
                PrintError($"Not enough stock available. Only {selected.RemainingStock} left for \"{selected.Name}\".");
                Pause();
                continue;
            }
            // ── Add to cart / update duplicate ───────────────────────────
            bool found = false;
            for (int i = 0; i < cartCount; i++)
            {
                if (cart[i].Product.Id == selected.Id)
                {
                    // Check combined quantity against remaining stock
                    int newQty = cart[i].Quantity + qty;
                    if (!selected.HasEnoughStock(newQty))
                    {
                        PrintError($"Cannot add {qty} more. Only {selected.RemainingStock - cart[i].Quantity} additional units available.");
                        found = true;
                        Pause();
                        break;
                    }
                    // Update existing cart item
                    cart[i].Quantity  = newQty;
                    cart[i].Subtotal  = selected.GetItemTotal(newQty);
                    selected.DeductStock(qty);
                    PrintSuccess($"Updated cart: {selected.Name} x{newQty}  (Subtotal: PHP {cart[i].Subtotal:F2})");
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                // New cart entry
                selected.DeductStock(qty);
                cart[cartCount] = new CartItem(selected, qty);
                cartCount++;
                PrintSuccess($"Added to cart: {selected.Name} x{qty}  (Subtotal: PHP {cart[cartCount - 1].Subtotal:F2})");
            }
            // ── Continue shopping? ────────────────────────────────────────
            Console.Write("\n  Add more items? (Y/N): ");
            string cont = Console.ReadLine().Trim().ToUpper();
            if (cont != "Y") shopping = false;
        }
        // ── Receipt ───────────────────────────────────────────────────────
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
            // Discount
            double discount = 0;
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
        // ── Updated stock ─────────────────────────────────────────────────
        Console.WriteLine();
        PrintDivider('=', 55);
        Console.WriteLine("  UPDATED REMAINING STOCK");
        PrintDivider('=', 55);
        Console.WriteLine($"  {"#",-4} {"Product",-30} {"Remaining Stock",15}");
        PrintDivider('-', 55);
        foreach (var p in menu)
        {
            string stockLabel = p.RemainingStock == 0 ? "OUT OF STOCK" : p.RemainingStock.ToString();
            if (p.RemainingStock == 0) Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  [{p.Id}] {p.Name,-30} {stockLabel,15}");
            Console.ResetColor();
        }
        PrintDivider('=', 55);
        Console.WriteLine("\n  Thank you for shopping at TechStore PH!\n");
    }
    // ── Helpers ────────────────────────────────────────────────────────────
    static void PrintHeader(string title)
    {
        PrintDivider('*', 55);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"  {title}");
        Console.ResetColor();
        PrintDivider('*', 55);
    }
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