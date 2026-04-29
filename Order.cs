using System;

//order cart.CS

class Order
{

    public string    ReceiptNo  { get; }
    public DateTime  Timestamp  { get; }
    public CartItem[] Items     { get; }
    public double    GrandTotal { get; }
    public double    Discount   { get; }
    public double    FinalTotal { get; }
    public double    Payment    { get; }

    // Change is always derived no need to store separately
    public double Change => Payment - FinalTotal;

    //constructor
    public Order(string receiptNo, CartItem[] items,
                 double grandTotal, double discount,
                 double finalTotal, double payment)
    {
        ReceiptNo  = receiptNo;
        Timestamp  = DateTime.Now;
        Items      = items;
        GrandTotal = grandTotal;
        Discount   = discount;
        FinalTotal = finalTotal;
        Payment    = payment;
    }
}