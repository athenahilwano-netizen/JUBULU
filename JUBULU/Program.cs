using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
namespace JubuluFoodKiosk
{
    internal class Program
    {
        private const string RestaurantName = "JUBULU FOOD KIOSK";
        private const string PaidStatus = "PAID";
        private static readonly string[] MenuNames =
        {
 "Chicken Meal",
 "Burger",
 "Fries",
 "Soft Drink",
 "Sundae"
 };
        private static readonly decimal[] MenuPrices =
        {
 120.00m,
 60.00m,
 45.00m,
 30.00m,
 35.00m
 };
        private static readonly int[] CartQuantities =
        new int[MenuNames.Length];
        private static readonly string RecordsFolder =
        Path.Combine(AppContext.BaseDirectory, "records");
        private static readonly string OrdersFile =
        Path.Combine(RecordsFolder, "orders.txt");
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = RestaurantName;
            while (true)
            {
                Console.Clear();
                DisplayHeader();
                Console.WriteLine("1. Customer View");
                Console.WriteLine("2. Cashier View");
                Console.WriteLine("3. Exit");
                Console.WriteLine();
                int choice = ReadInteger(
                "Select an option: ",
                1,
                3);
                switch (choice)
                {
                    case 1:
                        CustomerView();
                        break;
                    case 2:
                        CashierView();
                        break;
                    case 3:
                        Console.WriteLine();
                        Console.WriteLine("Thank you for using JUBULU!");
                        return;
                }
            }
        }
        private static void DisplayHeader()
        {
            Console.WriteLine("==============================================");
            Console.WriteLine($" {RestaurantName}");
            Console.WriteLine("==============================================");
        }
        private static void CustomerView()
        {
            ClearCart();
            while (true)
            {
                Console.Clear();
                DisplayHeader();
                DisplayFoodMenu();
                Console.WriteLine();
                Console.WriteLine("A. Add an item");
                Console.WriteLine("R. Review order");
                Console.WriteLine("C. Cancel order");
                Console.WriteLine("B. Back to main menu");
                Console.WriteLine();
                Console.Write("Select an option: ");
                string choice = (Console.ReadLine() ?? "")
                .Trim()
                .ToUpperInvariant();
                switch (choice)
                {
                    case "A":
                        AddItemToCart();
                        break;
                    case "R":
                        if (IsCartEmpty())
                        {
                            ShowMessage(
                            "The cart is empty. Add an item before checkout.");
                        }
                        else if (ReviewOrder())
                        {
                            return;
                        }
                        break;
                    case "C":
                    case "B":
                        if (!IsCartEmpty())
                        {
                            ClearCart();
                            ShowMessage(
                            "Order canceled. No sale was saved.");
                        }
                        return;
                    default:
                        ShowMessage("Invalid option. Please try again.");
                        break;
                }
            }
        }
        private static void DisplayFoodMenu()
        {
            Console.WriteLine("FOOD MENU");
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("{0,-5} {1,-24} {2,12}",
            "No.", "Item", "Price");
            Console.WriteLine("----------------------------------------------");
            for (int index = 0; index < MenuNames.Length; index++)
            {
                Console.WriteLine(
                "{0,-5} {1,-24} PHP {2,7:N2}",
                index + 1,
                MenuNames[index],
                MenuPrices[index]);
            }
            Console.WriteLine("----------------------------------------------");
        }
        private static void AddItemToCart()
        {
            Console.WriteLine();
            int itemNumber = ReadInteger(
            $"Enter item number (1-{MenuNames.Length}): ",
            1,
            MenuNames.Length);
            int itemIndex = itemNumber - 1;
            int remainingQuantity = 99 - CartQuantities[itemIndex];
            if (remainingQuantity == 0)
            {
                ShowMessage(
                "This item already has the maximum quantity of 99.");
                return;
            }
            int quantity = ReadInteger(
            $"Enter quantity (1-{remainingQuantity}): ",
            1,
            remainingQuantity);
            CartQuantities[itemIndex] += quantity;
            ShowMessage(
            $"{quantity} x {MenuNames[itemIndex]} added to the cart.");
        }
        private static int ReadInteger(
        string prompt,
        int minimum,
        int maximum)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = (Console.ReadLine() ?? "").Trim();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Input cannot be blank.");
                    continue;
                }
                if (!int.TryParse(
                input,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int value))
                {
                    Console.WriteLine(
                    "Enter a whole number without letters or decimals.");
                    continue;
                }
                if (value < minimum || value > maximum)
                {
                    Console.WriteLine(
                    $"Enter a value from {minimum} to {maximum}.");
                    continue;
                }
                return value;
            }
        }
        private static bool ReviewOrder()
        {
            while (true)
            {
                Console.Clear();
                DisplayHeader();
                DisplayCart();
                Console.WriteLine();
                Console.WriteLine("1. Add more items");
                Console.WriteLine("2. Confirm and pay");
                Console.WriteLine("3. Cancel order");
                Console.WriteLine();
                int choice = ReadInteger(
                "Select an option: ",
                1,
                3);
                switch (choice)
                {
                    case 1:
                        return false;
                    case 2:
                        return ProcessPayment();
                    case 3:
                        ClearCart();
                        ShowMessage(
                        "Order canceled. No sale was saved.");
                        return true;
                }
            }
        }
        private static void DisplayCart()
        {
            Console.WriteLine("ORDER REVIEW");
            Console.WriteLine(
            "----------------------------------------------------------------");
            Console.WriteLine(
            "{0,-20} {1,12} {2,8} {3,16}",
            "Item",
            "Unit Price",
            "Qty",
            "Subtotal");
            Console.WriteLine(
            "----------------------------------------------------------------");
            for (int index = 0;
            index < CartQuantities.Length;
            index++)
            {
                if (CartQuantities[index] == 0)
                {
                    continue;
                }
                decimal subtotal =
                MenuPrices[index] * CartQuantities[index];
                Console.WriteLine(
                "{0,-20} PHP {1,7:N2} {2,8} PHP {3,10:N2}",
                MenuNames[index],
                MenuPrices[index],
                CartQuantities[index],
                subtotal);
            }
            Console.WriteLine(
            "----------------------------------------------------------------");
            Console.WriteLine(
            "{0,47} PHP {1,10:N2}",
            "TOTAL:",
            CalculateTotal());
            Console.WriteLine(
            "----------------------------------------------------------------");
        }
        private static decimal CalculateTotal()
        {
            decimal total = 0.00m;
            for (int index = 0;
            index < CartQuantities.Length;
            index++)
            {
                total += MenuPrices[index] *
                CartQuantities[index];
            }
            return total;
        }
        private static bool ProcessPayment()
        {
            decimal total = CalculateTotal();
            while (true)
            {
                Console.Clear();
                DisplayHeader();
                DisplayCart();
                Console.WriteLine();
                Console.WriteLine(
                "Enter X to cancel payment and the order.");
                Console.Write("Cash amount: PHP ");
                string input = (Console.ReadLine() ?? "").Trim();
                if (input.Equals(
                "X",
                StringComparison.OrdinalIgnoreCase))
                {
                    ClearCart();
                    ShowMessage(
                    "Payment canceled. No sale was saved.");
                    return true;
                }
                if (!TryParseMoney(input, out decimal cash))
                {
                    Console.WriteLine();
                    Console.WriteLine(
                    "Invalid cash amount. Enter a nonnegative " +
                    "number with no more than two decimal places.");
                    Pause();
                    continue;
                }
                if (cash < total)
                {
                    decimal amountNeeded = total - cash;
                    Console.WriteLine();
                    Console.WriteLine(
                    $"Insufficient cash. Amount still needed: " +
                    $"PHP {amountNeeded:N2}");
                    Console.WriteLine(
                    "The next entry must be the full cash amount.");
                    Pause();
                    continue;
                }
                string orderId = GenerateNextOrderId();
                decimal change = cash - total;
                bool saved = SaveOrder(
                orderId,
                PaidStatus,
                total,
                cash,
                change);
                if (!saved)
                {
                    Console.WriteLine();
                    Console.WriteLine(
                    "The order could not be saved.");
                    Console.WriteLine(
                    "No successful sale was recorded.");
                    Console.WriteLine();
                    Console.WriteLine("1. Retry saving");
                    Console.WriteLine("2. Cancel and return");
                    int saveChoice = ReadInteger(
                    "Select an option: ",
                    1,
                    2);
                    if (saveChoice == 1)
                    {
                        continue;
                    }
                    ClearCart();
                    return true;
                }
                Console.Clear();
                DisplayReceipt(
                orderId,
                PaidStatus,
                GetCurrentCartItems(),
                total,
                cash,
                change);
                ClearCart();
                Pause();
                return true;
            }
        }
        private static bool TryParseMoney(
        string input,
        out decimal value)
        {
            value = 0.00m;
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }
            if (!decimal.TryParse(
            input,
            NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture,
            out value))
            {
                return false;
            }
            if (value < 0)
            {
                return false;
            }
            int decimalPlaces = GetDecimalPlaces(input);
            return decimalPlaces <= 2;
        }
        private static int GetDecimalPlaces(string input)
        {
            int decimalPointIndex = input.IndexOf('.');
            if (decimalPointIndex < 0)
            {
                return 0;
            }
            return input.Length - decimalPointIndex - 1;
        }
        private static List<OrderItem> GetCurrentCartItems()
        {
            List<OrderItem> items = new List<OrderItem>();
            for (int index = 0;
            index < CartQuantities.Length;
            index++)
            {
                int quantity = CartQuantities[index];
                if (quantity == 0)
                {
                    continue;
                }
                items.Add(new OrderItem
                {
                    Name = MenuNames[index],
                    UnitPrice = MenuPrices[index],
                    Quantity = quantity,
                    Subtotal = MenuPrices[index] * quantity
                });
            }
            return items;
        }
        private static string GenerateNextOrderId()
        {
            int highestNumber = 0;
            if (File.Exists(OrdersFile))
            {
                try
                {
                    foreach (string line in File.ReadLines(OrdersFile))
                    {
                        if (!line.StartsWith("ORDER|",
                        StringComparison.Ordinal))
                        {
                            continue;
                        }
                        string[] fields = line.Split('|');
                        if (fields.Length < 2)
                        {
                            continue;
                        }
                        string id = fields[1].Trim();
                        if (id.StartsWith(
                        "ORD",
                        StringComparison.OrdinalIgnoreCase) &&
                        int.TryParse(
                        id.Substring(3),
                        out int number))
                        {
                            highestNumber = Math.Max(
                            highestNumber,
                            number);
                        }
                    }
                }
                catch (IOException)
                {
                    // SaveOrder will display a useful error if
                    // the records location cannot be accessed.
                }
                catch (UnauthorizedAccessException)
                {
                    // SaveOrder handles the actual save failure.
                }
            }
            return $"ORD{highestNumber + 1:D4}";
        }
        private static bool SaveOrder(
        string orderId,
        string status,
        decimal total,
        decimal cash,
        decimal change)
        {
            try
            {
                Directory.CreateDirectory(RecordsFolder);
                using StreamWriter writer =
                new StreamWriter(OrdersFile, append: true);
                writer.WriteLine(
                $"ORDER|{orderId}|{status}");
                for (int index = 0;
                index < CartQuantities.Length;
                index++)
                {
                    if (CartQuantities[index] == 0)
                    {
                        continue;
                    }
                    decimal subtotal =
                    MenuPrices[index] *
                    CartQuantities[index];
                    writer.WriteLine(
                    $"ITEM|{EscapeField(MenuNames[index])}|" +
                    $"{MenuPrices[index].ToString("0.00", CultureInfo.InvariantCulture)}|" +
                    $"{CartQuantities[index]}|" +
                    $"{subtotal.ToString("0.00", CultureInfo.InvariantCulture)}");
                }
                writer.WriteLine(
                $"PAYMENT|" +
                $"{total.ToString("0.00", CultureInfo.InvariantCulture)}|" +
                $"{cash.ToString("0.00", CultureInfo.InvariantCulture)}|" +
                $"{change.ToString("0.00", CultureInfo.InvariantCulture)}");
                writer.WriteLine("END");
                return true;
            }
            catch (UnauthorizedAccessException exception)
            {
                Console.WriteLine(
                $"Save failed: {exception.Message}");
                return false;
            }
            catch (IOException exception)
            {
                Console.WriteLine(
                $"Save failed: {exception.Message}");
                return false;
            }
            catch (Exception exception)
            {
                Console.WriteLine(
                $"Unexpected save failure: {exception.Message}");
                return false;
            }
        }
        private static string EscapeField(string value)
        {
            return value
            .Replace("|", "/")
            .Replace("\r", " ")
            .Replace("\n", " ");
        }
        private static void CashierView()
        {
            while (true)
            {
                Console.Clear();
                DisplayHeader();
                Console.WriteLine("CASHIER VIEW");
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine("1. Find paid order");
                Console.WriteLine("2. View sales report");
                Console.WriteLine("3. Back to main menu");
                Console.WriteLine();
                int choice = ReadInteger(
                "Select an option: ",
                1,
                3);
                switch (choice)
                {
                    case 1:
                        FindOrder();
                        break;
                    case 2:
                        ShowSalesReport();
                        break;
                    case 3:
                        return;
                }
            }
        }
        private static void FindOrder()
        {
            Console.Clear();
            DisplayHeader();
            Console.Write("Enter Order ID: ");
            string orderId = (Console.ReadLine() ?? "").Trim();
            if (string.IsNullOrWhiteSpace(orderId))
            {
                ShowMessage("Order ID cannot be blank.");
                return;
            }
            List<SavedOrder> orders = ReadSavedOrders();
            SavedOrder? order = orders.FirstOrDefault(
            savedOrder => savedOrder.Id.Equals(
            orderId,
            StringComparison.OrdinalIgnoreCase));
            Console.Clear();
            DisplayHeader();
            if (order == null)
            {
                Console.WriteLine("Order not found.");
                Pause();
                return;
            }
            DisplayReceipt(
            order.Id,
            order.Status,
            order.Items,
            order.Total,
            order.Cash,
            order.Change);
            Pause();
        }
        private static void ShowSalesReport()
        {
            Console.Clear();
            DisplayHeader();
            Console.WriteLine("SALES REPORT");
            Console.WriteLine("----------------------------------------------");
            List<SavedOrder> paidOrders = ReadSavedOrders()
            .Where(order => order.Status.Equals(
            PaidStatus,
            StringComparison.OrdinalIgnoreCase))
            .ToList();
            decimal totalSales = paidOrders.Sum(
            order => order.Total);
            Console.WriteLine(
            $"Completed orders : {paidOrders.Count}");
            Console.WriteLine(
            $"Total sales : PHP {totalSales:N2}");
            if (paidOrders.Count == 0)
            {
                Console.WriteLine();
                Console.WriteLine("No sales yet.");
            }
            Console.WriteLine("----------------------------------------------");
            Pause();
        }
        private static List<SavedOrder> ReadSavedOrders()
        {
            List<SavedOrder> orders = new List<SavedOrder>();
            if (!File.Exists(OrdersFile))
            {
                return orders;
            }
            try
            {
                SavedOrder? currentOrder = null;
                foreach (string rawLine in File.ReadLines(OrdersFile))
                {
                    if (string.IsNullOrWhiteSpace(rawLine))
                    {
                        continue;
                    }
                    string[] fields = rawLine.Split('|');
                    string recordType = fields[0];
                    if (recordType == "ORDER" && fields.Length >= 3)
                    {
                        currentOrder = new SavedOrder
                        {
                            Id = fields[1],
                            Status = fields[2]
                        };
                    }
                    else if (recordType == "ITEM" &&
                    fields.Length >= 5 &&
                    currentOrder != null)
                    {
                        if (TryReadOrderItem(
                        fields,
                        out OrderItem? item))
                        {
                            currentOrder.Items.Add(item);
                        }
                    }
                    else if (recordType == "PAYMENT" &&
                    fields.Length >= 4 &&
                    currentOrder != null)
                    {
                        if (TryReadDecimal(
                        fields[1],
                        out decimal total) &&
                        TryReadDecimal(
                        fields[2],
                        out decimal cash) &&
                        TryReadDecimal(
                        fields[3],
                        out decimal change))
                        {
                            currentOrder.Total = total;
                            currentOrder.Cash = cash;
                            currentOrder.Change = change;
                        }
                    }
                    else if (recordType == "END" &&
                    currentOrder != null)
                    {
                        orders.Add(currentOrder);
                        currentOrder = null;
                    }
                }
            }
            catch (UnauthorizedAccessException exception)
            {
                Console.WriteLine(
                $"Unable to read orders: {exception.Message}");
                Pause();
            }
            catch (IOException exception)
            {
                Console.WriteLine(
                $"Unable to read orders: {exception.Message}");
                Pause();
            }
            return orders;
        }
        private static bool TryReadOrderItem(
        string[] fields,
        out OrderItem? item)
        {
            item = null;
            if (!TryReadDecimal(
            fields[2],
            out decimal unitPrice) ||
            !int.TryParse(
            fields[3],
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out int quantity) ||
            !TryReadDecimal(
            fields[4],
            out decimal subtotal))
            {
                return false;
            }
            item = new OrderItem
            {
                Name = fields[1],
                UnitPrice = unitPrice,
                Quantity = quantity,
                Subtotal = subtotal
            };
            return true;
        }
        private static bool TryReadDecimal(
        string input,
        out decimal value)
        {
            return decimal.TryParse(
            input,
            NumberStyles.AllowDecimalPoint |
            NumberStyles.AllowLeadingSign,
            CultureInfo.InvariantCulture,
            out value);
        }
        private static void DisplayReceipt(
        string orderId,
        string status,
        List<OrderItem> items,
        decimal total,
        decimal cash,
        decimal change)
        {
            Console.WriteLine("==============================================");
            Console.WriteLine($" {RestaurantName}");
            Console.WriteLine(" RECEIPT");
            Console.WriteLine("==============================================");
            Console.WriteLine($"Order ID : {orderId}");
            Console.WriteLine($"Status : {status}");
            Console.WriteLine("----------------------------------------------");
            foreach (OrderItem item in items)
            {
                Console.WriteLine(item.Name);
                Console.WriteLine(
                $" PHP {item.UnitPrice:N2} x {item.Quantity}" +
                $" = PHP {item.Subtotal:N2}");
            }
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine($"Total : PHP {total:N2}");
            Console.WriteLine($"Cash : PHP {cash:N2}");
            Console.WriteLine($"Change : PHP {change:N2}");
            Console.WriteLine("==============================================");
        }
        private static bool IsCartEmpty()
        {
            return CartQuantities.All(quantity => quantity == 0);
        }
        private static void ClearCart()
        {
            Array.Clear(
            CartQuantities,
            0,
            CartQuantities.Length);
        }
        private static void ShowMessage(string message)
        {
            Console.WriteLine();
            Console.WriteLine(message);
            Pause();
        }
        private static void Pause()
        {
            Console.WriteLine();
            Console.Write("Press Enter to continue...");
            Console.ReadLine();
        }
    }
    internal class OrderItem
    {
        public string Name { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }
    internal class SavedOrder
    {
        public string Id { get; set; } = "";
        public string Status { get; set; } = "";
        public List<OrderItem> Items { get; set; } =
        new List<OrderItem>();
        public decimal Total { get; set; }
        public decimal Cash { get; set; }
        public decimal Change { get; set; }
    }
}