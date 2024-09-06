

using System.Text.RegularExpressions;

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
        public void  AddProducts(List<Product> products)
        {   
            foreach (var product in products)
            {
                decimal tax = TaxCalculator.CalculateTax(product);
                items.Add(new ReceiptItem(product, tax));
            }
            
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


    class SalesTaxProcessor
    {
        static void Main(string[] args)
        {

            while (true)
            {
                Console.WriteLine("\nEnter Your Inputs:");
                string userInput = Console.ReadLine();

                if (string.IsNullOrEmpty(userInput))
                {
                    Console.WriteLine("Input cannot be null or empty. Please try again.");
                    continue;
                }

                try
                {
                    ShoppingCart cart =  DataProcessor.CreateShoppingCartFromInput(userInput);
                    var receipt = cart.GenerateReceipt();
                    receipt.PrintReceipt();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }

        }
    }


    public class DataProcessor
    {
        private const string InputPattern = @"(\d+) ([\w\s]+) at (\d+\.\d+)";
        public static ShoppingCart CreateShoppingCartFromInput(string input)
        {   
            try
            {

                Regex regex = new Regex(InputPattern, RegexOptions.IgnoreCase);
                MatchCollection matches = regex.Matches(input);
                List<Product> products = new List<Product>();


                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        int quantity = ParseQuantity(match);
                        string name = ExtractProductName(match);
                        decimal price = ParsePrice(match);
                        bool isImported = IsImported(name);
                        bool isTaxable = IsSalesTaxExemptable(name);

                        Product product = new Product(name, quantity, price, isImported, isTaxable);
                        products.Add(product);
                    }
                }

                ShoppingCart shoppingCart = new ShoppingCart() ;
                shoppingCart.AddProducts(products);
                return shoppingCart;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }


        private static bool IsSalesTaxExemptable(string productName)
        {
            string[] taxableKeywords = { "chocolate", "book", "pills" };
            foreach (var keyword in taxableKeywords)
            {
                if (productName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }
            return false;
        }
        private static bool IsImported(string name) => name.Contains("imported", StringComparison.OrdinalIgnoreCase);
        
        private static decimal ParsePrice(Match match) => decimal.Parse(match.Groups[3].Value);

        private static string ExtractProductName(Match match) => match.Groups[2].Value.Trim();

        private static int ParseQuantity(Match match) => int.Parse(match.Groups[1].Value);

    }
}
