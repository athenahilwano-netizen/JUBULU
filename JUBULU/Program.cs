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
            Console.WriteLine($"             {RestaurantName}");
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

        private static void CustomerView()
        {
            ClearCart();

            while (true)
            {
                Console.Clear();
                DisplayHeader();
                DisplayFoodMenu();

                Console.WriteLine("A. Add an item");
                Console.WriteLine("R. Review order");
                Console.WriteLine("C. Cancel order");
                Console.WriteLine("B. Back to main menu");

                string choice = (Console.ReadLine() ?? "")
                    .Trim().ToUpperInvariant();

                switch (choice)
                {
                    case "A":
                        AddItemToCart();
                        break;

                    case "R":
                        if (IsCartEmpty())
                        {
                            ShowMessage("The cart is empty.");
                        }
                        else if (ReviewOrder())
                        {
                            return;
                        }
                        break;

                    case "C":
                    case "B":
                        ClearCart();
                        ShowMessage("Order canceled. No sale was saved.");
                        return;

                    default:
                        ShowMessage("Invalid option. Please try again.");
                        break;
                }
            }
        }

        private static void AddItemToCart()
        {
            int itemNumber = ReadInteger(
                $"Enter item number (1-{MenuNames.Length}): ",
                1, MenuNames.Length);

            int itemIndex = itemNumber - 1;
            int remainingQuantity = 99 - CartQuantities[itemIndex];

            int quantity = ReadInteger(
                $"Enter quantity (1-{remainingQuantity}): ",
                1, remainingQuantity);

            CartQuantities[itemIndex] += quantity;

            ShowMessage(
                $"{quantity} x {MenuNames[itemIndex]} added to the cart.");
        }

        private static int ReadInteger(
            string prompt, int minimum, int maximum)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = (Console.ReadLine() ?? "").Trim();

                if (int.TryParse(input, out int value) &&
                    value >= minimum && value <= maximum)
                {
                    return value;
                }

                Console.WriteLine(
                    $"Enter a valid whole number from {minimum} to {maximum}.");
            }
        }

        private static bool ReviewOrder()
        {
            while (true)
            {
                Console.Clear();
                DisplayHeader();
                DisplayCart();

                Console.WriteLine("1. Add more items");
                Console.WriteLine("2. Confirm and pay");
                Console.WriteLine("3. Cancel order");

                int choice = ReadInteger("Select an option: ", 1, 3);

                switch (choice)
                {
                    case 1:
                        return false;
                    case 2:
                        return ProcessPayment();
                    case 3:
                        ClearCart();
                        ShowMessage("Order canceled.");
                        return true;
                }
            }
        }

        private static void DisplayCart()
        {
            Console.WriteLine("ORDER REVIEW");

            for (int index = 0; index < CartQuantities.Length; index++)
            {
                if (CartQuantities[index] == 0)
                    continue;

                decimal subtotal =
                    MenuPrices[index] * CartQuantities[index];

                Console.WriteLine(
                    $"{MenuNames[index]} - PHP {MenuPrices[index]:N2} " +
                    $"x {CartQuantities[index]} = PHP {subtotal:N2}");
            }

            Console.WriteLine($"TOTAL: PHP {CalculateTotal():N2}");
        }

        private static decimal CalculateTotal()
        {
            decimal total = 0;

            for (int index = 0; index < CartQuantities.Length; index++)
            {
                total += MenuPrices[index] * CartQuantities[index];
            }

            return total;
        }

        private static bool ProcessPayment()
        {
            decimal total = CalculateTotal();

            while (true)
            {
                Console.Clear();
                DisplayCart();

                Console.Write("Cash amount: PHP ");
                string input = (Console.ReadLine() ?? "").Trim();

                if (input.Equals("X", StringComparison.OrdinalIgnoreCase))
                {
                    ClearCart();
                    return true;
                }

                if (!TryParseMoney(input, out decimal cash) || cash < total)
                {
                    ShowMessage("Invalid or insufficient cash.");
                    continue;
                }

                string orderId = GenerateNextOrderId();
                decimal change = cash - total;

                bool saved = SaveOrder(
                    orderId, PaidStatus, total, cash, change);

                if (!saved)
                {
                    ShowMessage("The order could not be saved.");
                    return true;
                }

                DisplayReceipt(
                    orderId, PaidStatus, GetCurrentCartItems(),
                    total, cash, change);

                ClearCart();
                Pause();
                return true;
            }
        }
    }
}
