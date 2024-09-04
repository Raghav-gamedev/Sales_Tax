
namespace SalesTaxApp
{
    
    public class Product
    {
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public bool IsImported { get; private set; }
        public bool IsExempt { get; private set; }
        public int Quantity { get; private set; }

        public Product(string name, int quantity, decimal price, bool isImported, bool isExempt)
        {
            Name = name;
            Price = price;
            IsImported = isImported;
            IsExempt = isExempt;
            Quantity = quantity;
        }
    }

    public static class TaxCalculator
    {
        private const decimal BasicTaxRate = 0.10m;
        private const decimal ImportDutyRate = 0.05m;

        public static decimal CalculateTax(Product product)
        {
            decimal tax = 0m;

            if (!product.IsExempt)
            {
                tax += product.Price * BasicTaxRate;
            }

            if (product.IsImported)
            {
                tax += product.Price * ImportDutyRate;
            }

            return RoundUpToNearestFiveCents(tax);
        }

        private static decimal RoundUpToNearestFiveCents(decimal amount)
        {
            return Math.Ceiling(amount * 20) / 20;
        }
    }

    public class ReceiptItem
    {
        public Product Product { get; private set; }
        public decimal Tax { get; private set; }
        public decimal TotalPrice => Product.Quantity * (Product.Price + Tax);

        public ReceiptItem(Product product, decimal tax)
        {
            Product = product;
            Tax = tax;
        }
    }

    public class ShoppingCart
    {
        private List<ReceiptItem> items = new List<ReceiptItem>();

        public void AddProduct(Product product)
        {
            decimal tax = TaxCalculator.CalculateTax(product);
            items.Add(new ReceiptItem(product, tax));
        }

        public Receipt GenerateReceipt()
        {
            return new Receipt(items);
        }
    }

    public class Receipt
    {
        private List<ReceiptItem> items;
        public decimal TotalTaxes { get; private set; }
        public decimal TotalPrice { get; private set; }

        public Receipt(List<ReceiptItem> items)
        {
            this.items = items;
            CalculateTotals();
        }

        private void CalculateTotals()
        {
            foreach (var item in items)
            {
                TotalTaxes += item.Product.Quantity * item.Tax;
                TotalPrice += item.TotalPrice;
            }
        }

        public void PrintReceipt()
        {
            foreach (var item in items)
            {
                Console.WriteLine($"{item.Product.Quantity} {item.Product.Name}: {item.TotalPrice:F2}");
            }
            Console.WriteLine($"Sales Taxes: {TotalTaxes:F2}");
            Console.WriteLine($"Total: {TotalPrice:F2}");
        }
    }

  
    class Program
    {
        static void Main(string[] args)
        {
            //Here are the input examples given in the docs.
            ShoppingCart cart1 = new ShoppingCart();
            cart1.AddProduct(new Product("book", 1, 12.49m, false, true));
            cart1.AddProduct(new Product("music CD", 1, 14.99m, false, false));
            cart1.AddProduct(new Product("chocolate bar", 1, 0.85m, false, true));

            
            ShoppingCart cart2 = new ShoppingCart();
            cart2.AddProduct(new Product("imported box of chocolates", 1, 10.00m, true, true));
            cart2.AddProduct(new Product("imported bottle of perfume", 1, 47.50m, true, false));

        
            ShoppingCart cart3 = new ShoppingCart();
            cart3.AddProduct(new Product("imported bottle of perfume", 1, 27.99m, true, false));
            cart3.AddProduct(new Product("bottle of perfume", 1, 18.99m, false, false));
            cart3.AddProduct(new Product("packet of headache pills", 1, 9.75m, false, true));
            cart3.AddProduct(new Product("box of imported chocolates", 1, 11.25m, true, true));

            Console.WriteLine("Output 1:");
            cart1.GenerateReceipt().PrintReceipt();
            Console.WriteLine();

            Console.WriteLine("Output 2:");
            cart2.GenerateReceipt().PrintReceipt();
            Console.WriteLine();

            Console.WriteLine("Output 3:");
            cart3.GenerateReceipt().PrintReceipt();

            Console.WriteLine("_______________________________________________________________________");



             // Custom Carts
            ShoppingCart cart = new ShoppingCart();
            int productCount = GetValidIntegerInput("Enter the number of products:", minValue: 1);

            for (int i = 0; i < productCount; i++)
            {
                Console.WriteLine($"\nEnter details for product {i + 1}:");

                string name = GetValidStringInput("Name:");
                int quantity = GetValidIntegerInput("Quantity:", minValue: 1);
                decimal price = GetValidDecimalInput("Price:", minValue: 0);
                bool isImported = GetValidBooleanInput("is the product category one of the following: (book, food, medical, other)? (yes/no):");
                bool isExempt = GetValidBooleanInput("Is the product exempt from basic sales tax? (yes/no):");

                cart.AddProduct(new Product(name, quantity, price, isImported, isExempt));
            }

            Console.WriteLine("\nReceipt:");
            cart.GenerateReceipt().PrintReceipt();

            static int GetValidIntegerInput(string prompt, int minValue = int.MinValue, int maxValue = int.MaxValue)
            {
                int value;
                do
                {
                    Console.Write(prompt + " ");
                } while (!int.TryParse(Console.ReadLine(), out value) || value < minValue || value > maxValue);

                return value;
            }

            static decimal GetValidDecimalInput(string prompt, decimal minValue = decimal.MinValue, decimal maxValue = decimal.MaxValue)
            {
                decimal value;
                do
                {
                    Console.Write(prompt + " ");
                } while (!decimal.TryParse(Console.ReadLine(), out value) || value < minValue || value > maxValue);

                return value;
            }

            static string GetValidStringInput(string prompt)
            {
                string input;
                do
                {
                    Console.Write(prompt + " ");
                    input = Console.ReadLine().Trim();
                } while (string.IsNullOrEmpty(input));

                return input;
            }

            static bool GetValidBooleanInput(string prompt)
            {
                string input;
                do
                {
                    Console.Write(prompt + " ");
                    input = Console.ReadLine().Trim().ToLower();
                    if (input != "yes" && input != "no")
                    {
                        Console.WriteLine("Invalid input. Please enter 'yes' or 'no'.");
                    }
                } while (input != "yes" && input != "no");

                return input == "yes";
            }

        }
    }
}
