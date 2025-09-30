using CarManagerCLI.Controllers;
using CarManagerCLI.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace CarManagerCLI.Client
{
    public class Manager
    {
        /// <summary>
        /// Vehicle management page
        /// </summary>
        /// <param name="dealerId">Dealer's Id</param>
        public static void ManageVehicle(string dealerId)
        {
            Console.Write("Enter vehicle plate (format XXX-000): ");
            string plate = Console.ReadLine()!.Trim().ToUpperInvariant();

            if (!Car.CheckRegexPlate(plate))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid plate format. Use XXX-000.");
                Console.ResetColor();
                return;
            }

            CarDealer? dealer = DealerController.GetDealerById(dealerId);
            if (dealer == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Dealer not found.");
                Console.ResetColor();
                return;
            }

            Car? car = dealer.Cars.FirstOrDefault(c => c.Plate == plate);
            if (car == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Car not found.");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n================= VEHICLE INFO =================");
            Console.ResetColor();

            Console.WriteLine($"{"Plate:",-15} {car.Plate}");
            Console.WriteLine($"{"Brand:",-15} {car.Brand}");
            Console.WriteLine($"{"Model:",-15} {car.Model}");
            Console.WriteLine($"{"Year:",-15} {car.Year}");
            Console.WriteLine($"{"Color:",-15} {car.Color}");
            Console.WriteLine($"{"VIN:",-15} {car.Vin}");
            Console.WriteLine($"{"In use:",-15} {(car.IsInUse ? "Yes" : "No")}");

            if (car.IsInUse && !string.IsNullOrEmpty(car.CurrentUser))
            {
                Console.WriteLine($"{"Current user:",-15} {car.CurrentUser}");
            }

            if (car.LastUsers != null && car.LastUsers.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\nLast 5 users who rented this car:");
                Console.ResetColor();

                var lastFive = car.LastUsers
                                  .TakeLast(5)
                                  .Reverse()
                                  .ToList();

                foreach (var user in lastFive)
                {
                    Console.WriteLine($"   - {user}");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("\nNo rental history available.");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=================================================");
            Console.ResetColor();

            if (!car.IsInUse)
            {
                Console.Write("Do you want to rent this car? (y/n): ");
                string input = Console.ReadLine()!.Trim().ToLower();

                if (input == "y")
                {
                    Console.Write("Enter numeric User ID: ");
                    string userId = Console.ReadLine()!.Trim();

                    if (!Regex.IsMatch(userId, @"^\d+$"))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("User ID must be numeric.");
                        Console.ResetColor();
                        return;
                    }

                    try
                    {
                        DealerController.RentCar(dealerId, plate, userId);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Car successfully rented!");
                        Console.ResetColor();
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Error: {ex.Message}");
                        Console.ResetColor();
                    }
                }
            }
            else
            {
                Console.Write("Do you want to return this car? (y/n): ");
                string input = Console.ReadLine()!.Trim().ToLower();

                if (input == "y")
                {
                    try
                    {
                        DealerController.ReturnCar(dealerId, plate);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Car successfully returned!");
                        Console.ResetColor();
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Error: {ex.Message}");
                        Console.ResetColor();
                    }
                }
            }
        }
    }
}
