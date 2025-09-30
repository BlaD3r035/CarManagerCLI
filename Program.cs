using System;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.IO;
using CarManagerCLI.Models;
using CarManagerCLI.Config;
using CarManagerCLI.Controllers;
using CarManagerCLI.Client;
using System.Security.Cryptography;
using System.Text.Json.Serialization.Metadata;
using System.Runtime.ConstrainedExecution;

namespace CarManagerCLI
{ 
    class Program
    {
        
        static string DealerId = null!; // current session's user Id

        static void Main(string[] args)
        {
            SessionConfig? dealerSession = SessionConfig.GetActiveSession();
            if(dealerSession != null && dealerSession.Presence == true)
            {
                DealerId = dealerSession.DealerId!;
                Menu();
            }
            DealersConfig data = DealersConfig.Load();
            List<CarDealer> Dealers = data.Dealers;

            while (DealerId == null)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Enter car dealer's name:");
                Console.ResetColor();

                string? dealerName = Console.ReadLine()?.Trim(); 

                if (string.IsNullOrEmpty(dealerName))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Dealer name cannot be empty. Try again.");
                    Console.ResetColor();
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                    continue; 
                }

              
                CarDealer? Dealer = Dealers.Find(d => d.Name.Equals(dealerName, StringComparison.OrdinalIgnoreCase));
                

                if (Dealer == null)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Dealer '{dealerName}' does not exist. Do you want to create a new one? (y/n)");
                    Console.ResetColor();

                    string? input = Console.ReadLine()?.Trim().ToLower();

                    if (input == "y")
                    {
                        Dealer = new CarDealer(dealerName);
                        Dealers.Add(Dealer);
                        DealerId = Dealer.Id;
                        DealersConfig.Save(data);
                        SessionConfig.LogIn(Dealer, false);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Dealer '{dealerName}' created successfully!");
                        Console.ResetColor();
                    }
                    else
                    {
                        Dealer = null;
                        
                    }
                }
                else
                {
                    SessionConfig.LogIn(Dealer, false);
                    DealerId = Dealer.Id;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Dealer '{dealerName}' loaded successfully!");
                    Console.ResetColor();
                }

                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
            
            Menu();
        }
        /// <summary>
        /// Main Menu
        /// </summary>
        static void Menu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("""

                                                                                                                                 
                ▄▄▄▄                 ▀▀█                                                                         
                █   ▀▄  ▄▄▄    ▄▄▄     █     ▄▄▄    ▄ ▄▄         ▄▄▄▄▄   ▄▄▄   ▄ ▄▄    ▄▄▄    ▄▄▄▄   ▄▄▄    ▄ ▄▄ 
                █    █ █▀  █  ▀   █    █    █▀  █   █▀  ▀        █ █ █  ▀   █  █▀  █  ▀   █  █▀ ▀█  █▀  █   █▀  ▀
                █    █ █▀▀▀▀  ▄▀▀▀█    █    █▀▀▀▀   █            █ █ █  ▄▀▀▀█  █   █  ▄▀▀▀█  █   █  █▀▀▀▀   █    
                █▄▄▄▀  ▀█▄▄▀  ▀▄▄▀█    ▀▄▄  ▀█▄▄▀   █            █ █ █  ▀▄▄▀█  █   █  ▀▄▄▀█  ▀█▄▀█  ▀█▄▄▀   █    
                                                                                              ▄  █               
                                                                                               ▀▀                
                """);
            CarDealer? carDealer = DealerController.GetDealerById(DealerId);
            SessionConfig? session = SessionConfig.GetActiveSession();
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Logged in as " + (carDealer == null? "Not Founded" : carDealer.Name));
            Console.ResetColor();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Select an option");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("1. Show all vehicles");
            Console.WriteLine("2. Manage vehicle");
            Console.WriteLine("3. Add a vehicle");
            Console.WriteLine("4. Remove a vehicle");
            Console.WriteLine("5. Session Presence" + $" ({(session == null || !session.Presence ? "Disabled":"Enabled") })" );
            Console.WriteLine("6. Exit");
            Console.ResetColor();

            int option = 0;
            while (option <= 0 || option >= 7) 
            {   
                Console.WriteLine("Chose an option");
                string input  = Console.ReadLine() ?? "";
                if(int.TryParse(input, out option)){
                    if(option > 0 && option < 6)
                    {
                        break;
                    }
                }
                
            }
            Option(option);

        }
        /// <summary>
        /// Main Menu option's handler
        /// </summary>
        /// <param name="option">Int option num range (1-6)</param>
        static void Option(int option) 
        {
            SessionConfig session = SessionConfig.GetActiveSession()!;
            
            Console.Clear();
           switch (option)
            {  
                // Show all vehicles
                case 1:
                    DealerController.ListVehicles(DealerId);
                    break;
                case 2:
                    Manager.ManageVehicle(DealerId);
                    break;
                // Add a vehicle
                case 3:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Enter vehicle plate (XXX-000 format)");
                    Console.ResetColor();
                    string plate = Console.ReadLine() ?? "";
                    plate = plate.ToUpper();
                    if (!Car.CheckRegexPlate(plate))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Plate format must be (XXX-000)");
                        break;
                    }
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Enter vehicle VIN");
                    Console.ResetColor();
                    string vin = Console.ReadLine() ?? "";
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Enter vehicle brand");
                    Console.ResetColor();
                    string brand = Console.ReadLine() ?? "";
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Enter vehicle model");
                    Console.ResetColor();
                    string model = Console.ReadLine() ?? "";
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Enter vehicle year");
                    Console.ResetColor();
                    string year = Console.ReadLine() ?? "";
                    if (!Car.CheckRegexYear(year))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Year must be in range (1000 - 9999)");
                        break;
                    }
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Enter vehicle color");
                    Console.ResetColor();
                    string color = Console.ReadLine() ?? "";
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\nReview the vehicle details:");
                    Console.WriteLine($"Plate: {plate}");
                    Console.WriteLine($"VIN: {vin}");
                    Console.WriteLine($"Brand: {brand}");
                    Console.WriteLine($"Model: {model}");
                    Console.WriteLine($"Year: {year}");
                    Console.WriteLine($"Color: {color}");
                    Console.ResetColor();

                    string? opt = null;
                    while (opt != "y" && opt != "n")
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Add this vehicle ? (y/n)");
                        Console.ResetColor();
                        opt = Console.ReadLine()!.ToLower() ;
                    }
                    if(opt == "y")
                    {
                        try
                        {
                          DealerController.AddCar(plate,vin,brand,model,year,color, DealerId);
                        }
                        catch (FormatException ex)
                        {
                            Console.ForegroundColor= ConsoleColor.Red;
                            Console.WriteLine($"Error: {ex.Message}");
                            Console.ResetColor();
                            break;
                        }
                        catch (InvalidOperationException ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Error: {ex.Message}");
                            Console.ResetColor();
                            break;
                        }
                        catch( Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Error: {ex.Message}");
                            Console.ResetColor();
                            break;
                        }
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Vehicle successfully Registered");
                        Console.ResetColor();
                        Thread.Sleep(1500);

                    }
                   

                        break;

                case 4:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Enter vehicle plate to remove (XXX-000 format)");
                    Console.ResetColor();
                    string plateToRemove = Console.ReadLine() ?? "";
                    plateToRemove =  plateToRemove.ToUpper();

                    if (!Car.CheckRegexPlate(plateToRemove))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Plate format must be (XXX-000)");
                        Console.ResetColor();
                        break;
                    }

                    string? confirm = null;
                    while (confirm != "y" && confirm != "n")
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Are you sure you want to remove vehicle with plate {plateToRemove.ToUpperInvariant()}? (y/n)");
                        Console.ResetColor();
                        confirm = Console.ReadLine()!.ToLower();
                    }

                    if (confirm == "y")
                    {
                        try
                        {
                            DealerController.RemoveCar(plateToRemove, DealerId);

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Vehicle successfully removed");
                            Console.ResetColor();
                            Thread.Sleep(1500);
                        }
                        catch (InvalidOperationException ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Error: {ex.Message}");
                            Console.ResetColor();
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Unexpected Error: {ex.Message}");
                            Console.ResetColor();
                            break;
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("Operation canceled.");
                        Console.ResetColor();
                    }

                    break;
                case 5:
                    Console.WriteLine("When Session presence is Enabled, system load the dealer's config automatically");
                    Console.WriteLine();
                    string? r = null;
                    while (r != "e" && r != "d")
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Enter (e) to Enable this option or (d) to Disable it");
                        Console.ResetColor();
                        r = Console.ReadLine();

                    }
                    
                    if(r == "e")
                    {
                        SessionConfig.SetPresence(true);
                    }
                    else
                    {
                        SessionConfig.SetPresence(false);
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Presence successfully changed");
                    Console.ResetColor();
                    Thread.Sleep(1500);
                    break;

                case 6:
                    if (!session.Presence)
                    {
                      SessionConfig.LogOut();
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                    Environment.Exit(0);
                    break;



                        }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Press Enter to return to main menu");
            Console.ReadLine();
            Menu();
        }
        
        
        

    }
}   