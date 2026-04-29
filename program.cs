using System;

//main

class Program
{

    const int    MAX_CART           = 10;
    const int    MAX_ORDERS         = 50;
    const double DISCOUNT_THRESHOLD = 5000;
    const double DISCOUNT_RATE      = 0.10;

    static CartItem[] cart      = new CartItem[MAX_CART];
    static int        cartCount = 0;

    static Order[]    orders     = new Order[MAX_ORDERS];
    static int        orderCount = 0;


    static void Main(string[] args)
    {
        Food[] menu = new Food[]
        {
            new Food(1,  "Pizza",           "Meals",    150.00,  20),
            new Food(2,  "Chicken Burger",  "Meals",    250.00, 20),
            new Food(3,  "Cheese Burger",   "Meals",    200.00, 10),
            new Food(4,  "Fried Chicken",   "Meals",    100.00, 15),
            new Food(5,  "Spaghetti",       "Pasta",    150.00,  17),
            new Food(6,  "Carbonara",       "Pasta",    200.00,  13),
            new Food(7,  "Hawaiian Pizza",  "Meals",    500.00, 12),
            new Food(8,  "Onion Rings",     "Sides",    150.00,  15),
            new Food(9,  "French Fries",    "Sides",     80.00,  21),
            new Food(10, "Iced Tea",        "Drinks",    60.00, 25),
            new Food(11, "Lemonade",        "Drinks",    70.00,  10),
            new Food(12, "Chocolate Cake",  "Desserts", 120.00,  20),
        };

        ConsoleHelper.PrintHeader("Welcome to FoodStore");

        bool running = true;
        while (running)
        {
            Console.WriteLine();
            ConsoleHelper.PrintDivider('=', 60);
            Console.WriteLine("  MAIN MENU");
            ConsoleHelper.PrintDivider('=', 60);
            Console.WriteLine("  [1] Browse & Add Items");
            Console.WriteLine("  [2] Search Product by Name");
            Console.WriteLine("  [3] Browse by Category");
            Console.WriteLine("  [4] Manage/Update quantity & Remove Cart");
            Console.WriteLine("  [5] Checkout");
            Console.WriteLine("  [6] View Order History");
            Console.WriteLine("  [0] Exit");
            ConsoleHelper.PrintDivider('=', 60);
            Console.Write("  Choose an option: ");

            switch ((Console.ReadLine() ?? "").Trim())
            {
                case "1": BrowseAndAdd(menu);     break;
                case "2": SearchProduct(menu);    break;
                case "3": BrowseByCategory(menu); break;
                case "4": ShowCartMenu();         break;
                case "5": Checkout(menu);         break;
                case "6": ViewOrderHistory();     break;
                case "0": running = false;        break;
                default:
                    ConsoleHelper.PrintError("Invalid choice. Please enter 0 to 6.");
                    ConsoleHelper.Pause();
                    break;
            }
        }

        Console.WriteLine("\n  Thank you for shopping at FoodStore! Goodbye!\n");
    }


    // store/browse/search category


    //  1. browse and add items section

    static void BrowseAndAdd(Food[] menu)
    {
        bool adding = true;
        while (adding)
        {
            ConsoleHelper.DisplayMenu(menu);

            if (cartCount >= MAX_CART)
            {
                ConsoleHelper.PrintWarning("Cart is full (max 10 items).");
                ConsoleHelper.Pause();
                return;
            }

            Console.Write("\n  Enter food number to add (or 0 to go back): ");
            if (!ConsoleHelper.TryReadInt(out int productId)) { ConsoleHelper.Pause(); continue; }
            if (productId == 0) return;

            if (productId < 1 || productId > menu.Length)
            {
                ConsoleHelper.PrintError($"Invalid number. Choose between 1 and {menu.Length}.");
                ConsoleHelper.Pause(); continue;
            }

            Food selected = menu[productId - 1];

            if (selected.RemainingStock == 0)
            {
                ConsoleHelper.PrintError($"\"{selected.Name}\" is out of stock.");
                ConsoleHelper.Pause(); continue;
            }

            Console.Write($"  Enter quantity for {selected.Name} (available: {selected.RemainingStock}): ");
            if (!ConsoleHelper.TryReadInt(out int qty)) { ConsoleHelper.Pause(); continue; }

            if (qty <= 0)
            {
                ConsoleHelper.PrintError("Quantity must be greater than zero.");
                ConsoleHelper.Pause(); continue;
            }

            if (!selected.HasEnoughStock(qty))
            {
                ConsoleHelper.PrintError($"Not enough stock. Only {selected.RemainingStock} left.");
                ConsoleHelper.Pause(); continue;
            }

            AddToCart(selected, qty);

            adding = ConsoleHelper.AskYesNo("  Add more items?");
        }
    }


    //  2. search by name section


    static void SearchProduct(Food[] menu)
    {
        Console.Write("\n  Enter product name to search: ");
        string keyword = (Console.ReadLine() ?? "").Trim().ToLower();

        if (keyword == "")
        {
            ConsoleHelper.PrintError("Search keyword cannot be empty.");
            ConsoleHelper.Pause(); return;
        }

        Console.WriteLine();
        ConsoleHelper.PrintDivider('=', 70);
        Console.WriteLine("  SEARCH RESULTS");
        ConsoleHelper.PrintDivider('=', 70);
        Console.WriteLine($"  {"#",-4} {"Food",-25} {"Category",-14} {"Price",10}   {"Stock",5}");
        ConsoleHelper.PrintDivider('-', 70);

        bool found = false;
        foreach (var p in menu)
        {
            if (!p.Name.ToLower().Contains(keyword)) continue;

            if (p.RemainingStock == 0)
                Console.ForegroundColor = ConsoleColor.DarkGray;
            p.DisplayFood();
            Console.ResetColor();

            if (p.IsLowStock() && p.RemainingStock > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"       ⚠  LOW STOCK — only {p.RemainingStock} left!");
                Console.ResetColor();
            }

            found = true;
        }

        if (!found)
            Console.WriteLine($"  No products found matching \"{keyword}\".");

        ConsoleHelper.PrintDivider('=', 70);
        ConsoleHelper.Pause();
    }


    // 3. browse by category section
  

    static void BrowseByCategory(Food[] menu)
    {
        string[] cats     = new string[menu.Length];
        int      catCount = 0;
        foreach (var p in menu)
        {
            bool exists = false;
            for (int i = 0; i < catCount; i++)
                if (cats[i] == p.Category) { exists = true; break; }
            if (!exists) cats[catCount++] = p.Category;
        }

        Console.WriteLine();
        ConsoleHelper.PrintDivider('=', 60);
        Console.WriteLine("  CATEGORIES");
        ConsoleHelper.PrintDivider('=', 60);
        for (int i = 0; i < catCount; i++)
            Console.WriteLine($"  [{i + 1}] {cats[i]}");
        Console.WriteLine("  [0] Back");
        ConsoleHelper.PrintDivider('=', 60);
        Console.Write("  Choose a category: ");

        if (!ConsoleHelper.TryReadInt(out int choice) || choice == 0) return;
        if (choice < 1 || choice > catCount)
        {
            ConsoleHelper.PrintError("Invalid choice.");
            ConsoleHelper.Pause(); return;
        }

        string chosen = cats[choice - 1];

        Console.WriteLine();
        ConsoleHelper.PrintDivider('=', 70);
        Console.WriteLine($"  CATEGORY: {chosen.ToUpper()}");
        ConsoleHelper.PrintDivider('=', 70);
        Console.WriteLine($"  {"#",-4} {"Food",-25} {"Category",-14} {"Price",10}   {"Stock",5}");
        ConsoleHelper.PrintDivider('-', 70);

        foreach (var p in menu)
        {
            if (p.Category != chosen) continue;

            if (p.RemainingStock == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                p.DisplayFood();
                Console.WriteLine($"       (OUT OF STOCK)");
                Console.ResetColor();
            }
            else if (p.IsLowStock())
            {
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

        ConsoleHelper.PrintDivider('=', 70);
        ConsoleHelper.Pause();
    }


    //  cart -  1. add 2. view, 3. update, 4. remove, 5. clear  SECTIONS
   

    static void ShowCartMenu()
    {
        bool open = true;
        while (open)
        {
            Console.WriteLine();
            ConsoleHelper.PrintDivider('=', 60);
            Console.WriteLine("  CART MANAGEMENT");
            ConsoleHelper.PrintDivider('=', 60);
            Console.WriteLine("  [1] View Cart");
            Console.WriteLine("  [2] Update Item Quantity");
            Console.WriteLine("  [3] Remove an Item");
            Console.WriteLine("  [4] Clear Cart");
            Console.WriteLine("  [0] Back");
            ConsoleHelper.PrintDivider('=', 60);
            Console.Write("  Choose an option: ");

            switch ((Console.ReadLine() ?? "").Trim())
            {
                case "1": ViewCartInLine();       break;
                case "2": UpdateCartItem(); break;
                case "3": RemoveCartItem(); break;
                case "4": ClearCart();      break;
                case "0": open = false;     break;
                default:
                    ConsoleHelper.PrintError("Invalid choice. Enter 0 to 4.");
                    ConsoleHelper.Pause();
                    break;
            }
        }
    }

    static void AddToCart(Food selected, int qty)
    {
        // If item already in cart, update quantity
        for (int i = 0; i < cartCount; i++)
        {
            if (cart[i].Product.Id == selected.Id)
            {
                if (!selected.HasEnoughStock(qty))
                {
                    ConsoleHelper.PrintError(
                        $"Cannot add {qty} more. Only {selected.RemainingStock} additional units available."
                    );
                    return;
                }
                cart[i].Quantity += qty;
                selected.DeductStock(qty);
                ConsoleHelper.PrintSuccess(
                    $"Updated cart: {selected.Name} x{cart[i].Quantity}" +
                    $"  (Subtotal: PHP {cart[i].Subtotal:F2})"
                );
                return;
            }
        }

        // New item
        if (cartCount >= MAX_CART)
        {
            ConsoleHelper.PrintWarning("Cart is full.");
            return;
        }

        selected.DeductStock(qty);
        cart[cartCount] = new CartItem(selected, qty);
        cartCount++;
        ConsoleHelper.PrintSuccess(
            $"Added: {selected.Name} x{qty}" +
            $"  (Subtotal: PHP {cart[cartCount - 1].Subtotal:F2})"
        );
    }


    

    static void ViewCartInLine()
    {
        Console.WriteLine();
        ConsoleHelper.PrintDivider('=', 70);
        Console.WriteLine("  YOUR CART");
        ConsoleHelper.PrintDivider('=', 70);

        if (cartCount == 0)
        {
            Console.WriteLine("  Cart is empty.");
            ConsoleHelper.PrintDivider('=', 70);
            ConsoleHelper.Pause();
            return;
        }

        Console.WriteLine($"  {"#",-7} {"Item",-30} {"Qty",4}   {"Unit Price",10}   {"Subtotal",10}");
        ConsoleHelper.PrintDivider('-', 70);

        double total = 0;
        for (int i = 0; i < cartCount; i++)
        {
            Console.Write($"  {$"{cart[i].Product.Id, -4}"} ");
            cart[i].Display(i + 1);
            total += cart[i].Subtotal;
        }

        ConsoleHelper.PrintDivider('-', 70);
        Console.WriteLine($"  {"TOTAL:",-48} PHP {total,8:F2}");
        ConsoleHelper.PrintDivider('=', 70);
        ConsoleHelper.Pause();
    }

    static void CartLine()
    {
        ViewCartInLine();
        ConsoleHelper.Pause();
        
    }

    static void UpdateCartItem()
    {
        if (cartCount == 0)
        {
            ConsoleHelper.PrintWarning("Cart is empty.");
            ConsoleHelper.Pause(); return;
        }

        ViewCartInLine();

        Console.Write("  Enter cart item number to update (or 0 to cancel): ");
        if (!ConsoleHelper.TryReadInt(out int id) || id == 0) return;

        int idx = -1;

        for (int i = 0; i < cartCount; i++)
    {
        if (cart[i].Product.Id == id) { idx = i; break; }
    }

    if (idx == -1)
    {
        ConsoleHelper.PrintError($"Invalid Cart Number");
        ConsoleHelper.Pause(); return;
    }

        CartItem ci         = cart[idx];
        int      currentQty = ci.Quantity;
        int      maxAllowed = ci.Product.RemainingStock + currentQty;

        Console.Write(
            $"  Enter new quantity for {ci.Product.Name}" +
            $" (current: {currentQty}, max available: {maxAllowed}): "
        );
        if (!ConsoleHelper.TryReadInt(out int newQty)) { ConsoleHelper.Pause(); return; }

        if (newQty <= 0)
        {
            ConsoleHelper.PrintError("Quantity must be greater than zero. Use 'Remove' to delete.");
            ConsoleHelper.Pause(); return;
        }

        if (newQty > maxAllowed)
        {
            ConsoleHelper.PrintError($"Not enough stock. Max available: {maxAllowed}.");
            ConsoleHelper.Pause(); return;
        }

        ci.Product.RestoreStock(currentQty);
        ci.Product.DeductStock(newQty);
        ci.Quantity = newQty;

        ConsoleHelper.PrintSuccess(
            $"Updated: {ci.Product.Name} → x{newQty}  (Subtotal: PHP {ci.Subtotal:F2})"
        );
        ConsoleHelper.PrintAddLowStockAlert(ci.Product);
        ConsoleHelper.Pause();
    }

    static void RemoveCartItem()
    {
        if (cartCount == 0)
        {
            ConsoleHelper.PrintWarning("Cart is empty.");
            ConsoleHelper.Pause(); return;
        }

        ViewCartInLine();

        Console.Write("  Enter cart item number to remove (or 0 to cancel): ");
        if (!ConsoleHelper.TryReadInt(out int id) || id == 0) return;


        int idx = -1;
        for (int i = 0; i < cartCount; i++)
    {
        if (cart[i].Product.Id == id) { idx = i; break; }
    }

        if (idx == -1)
    {
        ConsoleHelper.PrintError($"Invalid Cart Number");
        ConsoleHelper.Pause(); return;
    }


        CartItem ci = cart[idx];
        ci.Product.RestoreStock(ci.Quantity);
        string removedName = ci.Product.Name;

        for (int i = idx; i < cartCount - 1; i++)
        cart[i] = cart[i + 1];
        cart[cartCount - 1] = null;
        cartCount--;

        ConsoleHelper.PrintSuccess($"Removed \"{removedName}\" from cart.");
        ConsoleHelper.Pause();
    }

    static void ClearCart()
    {
        if (cartCount == 0)
        {
            ConsoleHelper.PrintWarning("Cart is already empty.");
            ConsoleHelper.Pause(); return;
        }

        if (!ConsoleHelper.AskYesNo("  Are you sure you want to clear the entire cart?")) return;

        for (int i = 0; i < cartCount; i++)
        {
            cart[i].Product.RestoreStock(cart[i].Quantity);
            cart[i] = null;
        }
        cartCount = 0;

        ConsoleHelper.PrintSuccess("Cart cleared.");
        ConsoleHelper.Pause();
    }

  
    // checkout section
    

    static void Checkout(Food[] menu)
    {
        if (cartCount == 0)
        {
            ConsoleHelper.PrintWarning("Your cart is empty. Add items first.");
            ConsoleHelper.Pause(); return;
        }

        // Compute totals
        double grandTotal = 0;
        for (int i = 0; i < cartCount; i++)
            grandTotal += cart[i].Subtotal;

        double discount   = grandTotal >= DISCOUNT_THRESHOLD ? grandTotal * DISCOUNT_RATE : 0;
        double finalTotal = grandTotal - discount;

        // Payment validation — loop until sufficient
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"  Final Total: PHP {finalTotal:F2}");
        Console.ResetColor();

        double payment = 0;
        while (true)
        {
            Console.Write("  Enter payment amount: PHP ");
            string raw = Console.ReadLine() ?? "";
            if (!double.TryParse(raw, out payment) || payment <= 0)
            {
                ConsoleHelper.PrintError("Invalid amount. Please enter a positive numeric value.");
                continue;
            }
            if (payment < finalTotal)
            {
                ConsoleHelper.PrintError($"Insufficient payment. You need at least PHP {finalTotal:F2}.");
                continue;
            }
            break;
        }

        // Build receipt number and snapshot the cart
        string receiptNo = (orderCount + 1).ToString("D4");

        CartItem[] snapshot = new CartItem[cartCount];
        for (int i = 0; i < cartCount; i++)
            snapshot[i] = new CartItem(cart[i].Product, cart[i].Quantity);

        Order order = new Order(receiptNo, snapshot, grandTotal, discount, finalTotal, payment);

        if (orderCount < MAX_ORDERS)
            orders[orderCount++] = order;

        // Print receipt (low stock notice for ordered items is inside)
        ConsoleHelper.PrintReceipt(order);

        // Store-wide low stock scan after receipt
        ConsoleHelper.ShowFullLowStockAlert(menu);

        // Clear cart
        for (int i = 0; i < cartCount; i++) cart[i] = null;
        cartCount = 0;

        ConsoleHelper.Pause();
    }


    //order history section


    static void ViewOrderHistory()
    {
        Console.WriteLine();
        ConsoleHelper.PrintDivider('=', 60);
        Console.WriteLine("  ORDER HISTORY");
        ConsoleHelper.PrintDivider('=', 60);

        if (orderCount == 0)
        {
            Console.WriteLine("  No orders yet.");
            ConsoleHelper.PrintDivider('=', 60);
            ConsoleHelper.Pause(); return;
        }

        for (int i = 0; i < orderCount; i++)
        {
            Order o = orders[i];
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"  Receipt #{o.ReceiptNo}  |  {o.Timestamp:MMM dd, yyyy  hh:mm tt}");
            Console.ResetColor();
            Console.WriteLine($"    Items       : {o.Items.Length}");
            Console.WriteLine($"    Grand Total : PHP {o.GrandTotal:F2}");
            if (o.Discount > 0)
                Console.WriteLine($"    Discount    : PHP {o.Discount:F2}");
            Console.WriteLine($"    Final Total : PHP {o.FinalTotal:F2}");
            Console.WriteLine($"    Payment     : PHP {o.Payment:F2}");
            Console.WriteLine($"    Change      : PHP {o.Change:F2}");
            ConsoleHelper.PrintDivider('-', 60);
        }

        Console.Write("  Reprint a receipt? Enter receipt number (or 0 to skip): ");
        if (!ConsoleHelper.TryReadInt(out int pick) || pick == 0) { ConsoleHelper.Pause(); return; }

        string pickStr = pick.ToString("D4");
        bool   found   = false;
        for (int i = 0; i < orderCount; i++)
        {
            if (orders[i].ReceiptNo == pickStr)
            {
                ConsoleHelper.PrintReceipt(orders[i]);
                found = true;
                break;
            }
        }

        if (!found) ConsoleHelper.PrintError($"Receipt #{pickStr} not found.");
        ConsoleHelper.Pause();
    }
}