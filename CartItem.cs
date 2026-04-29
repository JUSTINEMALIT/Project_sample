using System;



//cartitem.CS

class CartItem
{
    // Properties
    public Food   Product  { get; set; }
    public int    Quantity { get; set; }

    // Subtotal ay naka auto-computed from Product price x Quantity
    public double Subtotal => Product.GetItemTotal(Quantity);

    public CartItem(Food product, int quantity)
    {
        Product  = product;
        Quantity = quantity;
    }

    //Display
    public void Display(int index)
    {
        Console.WriteLine(
            $"  [{index}] {Product.Name,-26} x{Quantity,3}" +
            $"   PHP {Product.Price,8:F2}   Subtotal: PHP {Subtotal,8:F2}"
        );
    }
}