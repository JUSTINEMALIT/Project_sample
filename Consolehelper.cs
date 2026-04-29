using System;


//consolehelper.cs


//  para sa lahat ng display and printing methods
//  service files stay focused on logic only.

static class ConsoleHelper
{
 
    public static void PrintDivider(char ch, int width)
        => Console.WriteLine(new string(ch, width));

    public static void PrintHeader(string title)
    {
        PrintDivider('*', 60);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"  {title}");
        Console.ResetColor();
        PrintDivider('*', 60);
    }


    public static void PrintError(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  [ERROR] {msg}");
        Console.ResetColor();
    }

    public static void PrintWarning(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  [!] {msg}");
        Console.ResetColor();
    }

    public static void PrintSuccess(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  [✓] {msg}");
        Console.ResetColor();
    }


    public static bool TryReadInt(out int value)
    {
        string input = Console.ReadLine() ?? "";
        if (!int.TryParse(input, out value))
        {
            PrintError("Invalid input. Please enter a numeric value.");
            return false;
        }
        return true;
    }

    public static bool AskYesNo(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt} (Y/N): ");
            string ans = (Console.ReadLine() ?? "").Trim().ToUpper();
            if (ans == "Y") return true;
            if (ans == "N") return false;
            PrintError("Invalid input. Please enter Y or N only.");
        }
    }

    public static void Pause()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("  Press Enter to continue...");
        Console.ResetColor();
        Console.ReadLine();
    }

    //  MENU DISPLAY
    //  — para sa out of stock items 
    //  — Low stock items not greater than 5 ay mag sho-show na  yellow warning

    public static void DisplayMenu(Food[] menu)
    {
        Console.WriteLine();
        PrintDivider('=', 70);
        Console.WriteLine("  STORE MENU");
        PrintDivider('=', 70);
        Console.WriteLine($"  {"#",-4} {"Food",-25} {"Category",-14} {"Price",10}   {"Stock",5}");
        PrintDivider('-', 70);

        foreach (var p in menu)
        {
            if (p.RemainingStock == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                p.DisplayFood();
                Console.WriteLine($"       (OUT OF STOCK)");
                Console.ResetColor();
            }
            else if (p.IsLowStock())
            {
                // Show the item line normally, then yellow warning below it
                p.DisplayFood();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"       ⚠  LOW STOCK — only {p.RemainingStock} left!");
                Console.ResetColor();
            }
            else
            {
                p.DisplayFood();
            }
        }

        PrintDivider('=', 70);
    }

  

    public static void PrintAddLowStockAlert(Food product)
    {
        if (!product.IsLowStock()) return;  

        Console.WriteLine();
        PrintDivider('!', 60);
        if (product.RemainingStock == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ⚠  STOCK ALERT: \"{product.Name}\" is now OUT OF STOCK!");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  ⚠  LOW STOCK ALERT: \"{product.Name}\" now only has");
            Console.WriteLine($"     {product.RemainingStock} stock(s) remaining.");
        }
        Console.ResetColor();
        PrintDivider('!', 60);
    }


    //  RECEIPT PRINTER
    //  Prints full receipt then the low stock will be notice if meron man 
    //  for any purchased item now at ≤ 5 stock.

    public static void PrintReceipt(Order order)
    {
        Console.WriteLine();
        PrintDivider('*', 60);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("              FOODSTORE RECEIPT");
        Console.ResetColor();
        PrintDivider('*', 60);
        Console.WriteLine($"  Receipt No : {order.ReceiptNo}");
        Console.WriteLine($"  Date/Time  : {order.Timestamp:MMMM dd, yyyy  hh:mm tt}");
        PrintDivider('-', 60);
        Console.WriteLine($"  {"Item",-26} {"Qty",4}   {"Unit Price",10}   {"Subtotal",10}");
        PrintDivider('-', 60);

        foreach (var ci in order.Items)
            Console.WriteLine(
                $"  {ci.Product.Name,-26} {ci.Quantity,4}" +
                $"   PHP {ci.Product.Price,7:F2}   PHP {ci.Subtotal,7:F2}"
            );

        PrintDivider('-', 60);
        Console.WriteLine($"  {"Grand Total:",-44} PHP {order.GrandTotal,7:F2}");

        if (order.Discount > 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  {"10% Discount:",-44} PHP {order.Discount,7:F2}");
            Console.ResetColor();
        }

        PrintDivider('-', 60);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"  {"Final Total:",-44} PHP {order.FinalTotal,7:F2}");
        Console.ResetColor();
        Console.WriteLine($"  {"Payment:",-44} PHP {order.Payment,7:F2}");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  {"Change:",-44} PHP {order.Change,7:F2}");
        Console.ResetColor();
        PrintDivider('*', 60);

        if (order.Discount > 0)
            PrintSuccess($"You saved PHP {order.Discount:F2} with the 10% discount!");

        // Low stock notice for items in this order
        PrintReceiptLowStockSection(order.Items);
    }

  
    // low stock section sa bottom ng receipt and Only shows items from the current order
    //  na yung  stock is nag dropped to ≤ 5 after purchase.


    private static void PrintReceiptLowStockSection(CartItem[] items)
    {
        bool hasAlert = false;

        foreach (var ci in items)
        {
            if (!ci.Product.IsLowStock()) continue;

            if (!hasAlert)
            {
                Console.WriteLine();
                PrintDivider('=', 60);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  ⚠  LOW STOCK NOTICE ON YOUR ORDER");
                Console.ResetColor();
                PrintDivider('=', 60);
                hasAlert = true;
            }

            if (ci.Product.RemainingStock == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  [✗] \"{ci.Product.Name}\" is now OUT OF STOCK after your purchase.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"  [!] \"{ci.Product.Name}\" — only {ci.Product.RemainingStock} stock(s) left.");
            }
            Console.ResetColor();
        }

        if (hasAlert) PrintDivider('=', 60);
    }

    // store wide low stock scan 
    //  Scans the whole menu after ng checkout.
    //  Shows ALL items at ≤ 5 stock, not just the
    //  ones that were in the order.
  

    public static void ShowFullLowStockAlert(Food[] menu)
    {
        bool anyAlert = false;

        foreach (var p in menu)
        {
            if (!p.IsLowStock()) continue;

            if (!anyAlert)
            {
                Console.WriteLine();
                PrintDivider('!', 60);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  ⚠  STORE-WIDE LOW STOCK ALERT");
                Console.ResetColor();
                PrintDivider('!', 60);
                anyAlert = true;
            }

            if (p.RemainingStock == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  [✗] \"{p.Name}\" — OUT OF STOCK");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"  [!] \"{p.Name}\" — only {p.RemainingStock} stock(s) left!");
            }
            Console.ResetColor();
        }

        if (anyAlert) PrintDivider('!', 60);
    }
}