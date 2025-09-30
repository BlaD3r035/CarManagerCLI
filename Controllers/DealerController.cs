using CarManagerCLI.Config;
using CarManagerCLI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CarManagerCLI.Controllers
{
    /// <summary>
    /// Provides operations for managing car dealers and their vehicles,
    /// including adding, removing, listing, renting, and returning cars.
    /// </summary>
    public class DealerController
    {
        /// <summary>
        /// Retrieves a dealer by its unique identifier.
        /// </summary>
        /// <param name="id">Dealer's unique identifier.</param>
        /// <returns>The matching <see cref="CarDealer"/> if found, otherwise <see langword="null"/>.</returns>
        public static CarDealer? GetDealerById(string id)
        {
            DealersConfig config = DealersConfig.Load();
            return config.Dealers.Find(d => d.Id == id);
        }

        /// <summary>
        /// Adds a new car to the specified dealer.
        /// </summary>
        /// <param name="plate">Car plate in format (XXX-000).</param>
        /// <param name="vin">Vehicle Identification Number (VIN).</param>
        /// <param name="brand">Car brand.</param>
        /// <param name="model">Car model.</param>
        /// <param name="year">Car manufacturing year (1000-9999).</param>
        /// <param name="color">Car color.</param>
        /// <param name="dealerId">Dealer's unique identifier.</param>
        /// <returns>The newly created <see cref="Car"/>.</returns>
        /// <exception cref="FormatException">Thrown when plate or year are invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the car already exists in the dealer.</exception>
        public static Car AddCar(string plate, string vin, string brand, string model, string year, string color, string dealerId)
        {
            // Normalize values
            plate = plate.ToUpperInvariant();
            vin = vin.ToUpperInvariant();
            brand = brand.ToUpperInvariant();
            model = model.ToUpperInvariant();
            year = year.ToUpperInvariant();
            color = color.ToUpperInvariant();

            // Validations
            if (!Car.CheckRegexPlate(plate))
            {
                throw new FormatException("Plate is not valid. (Use format XXX-000)");
            }
            if (!Car.CheckRegexYear(year))
            {
                throw new FormatException("Year must contain 4 digits.");
            }

            CarDealer dealer = GetDealerById(dealerId)!;
            if (dealer!.Cars.Any(c => c.Plate == plate))
            {
                throw new InvalidOperationException("Car already exists.");
            }

            // Add car
            Car car = new Car(plate, vin, brand, model, year, color);
            dealer.Cars.Add(car);

            DealersConfig config = DealersConfig.Load();
            var dealerInConfig = config.Dealers.FirstOrDefault(d => d.Id == dealerId);
            if (dealerInConfig != null)
            {
                dealerInConfig.Cars.Add(car);
            }
            DealersConfig.Save(config);

            return car;
        }

        /// <summary>
        /// Removes a car from a dealer by its plate.
        /// </summary>
        /// <param name="plate">Car plate (XXX-000).</param>
        /// <param name="dealerId">Dealer's unique identifier.</param>
        /// <exception cref="InvalidOperationException">Thrown when the car is not found.</exception>
        public static void RemoveCar(string plate, string dealerId)
        {
            plate = plate.ToUpperInvariant();

            CarDealer dealer = GetDealerById(dealerId)!;
            Car? car = dealer.Cars.FirstOrDefault(c => c.Plate == plate);

            if (car == null)
            {
                throw new InvalidOperationException("Car not found.");
            }

            dealer.Cars.Remove(car);

            DealersConfig config = DealersConfig.Load();
            var dealerInConfig = config.Dealers.FirstOrDefault(d => d.Id == dealerId);
            if (dealerInConfig != null)
            {
                var carInConfig = dealerInConfig.Cars.FirstOrDefault(c => c.Plate == plate);
                if (carInConfig != null)
                {
                    dealerInConfig.Cars.Remove(carInConfig);
                }
            }
            DealersConfig.Save(config);
        }

        /// <summary>
        /// Lists all cars of a dealer, grouped into available and rented vehicles.
        /// </summary>
        /// <param name="dealerId">Dealer's unique identifier.</param>
        public static void ListVehicles(string dealerId)
        {
            CarDealer? dealer = GetDealerById(dealerId);

            if (dealer?.Cars == null || dealer.Cars.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No cars found.");
                Console.ResetColor();
                return;
            }

            var availableCars = dealer.Cars.Where(c => !c.IsInUse).ToList();
            var rentedCars = dealer.Cars.Where(c => c.IsInUse).ToList();

            // Available cars
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n================= AVAILABLE VEHICLES =================");
            Console.WriteLine($"{"ID",-5} {"Plate",-8} {"BRAND",-15} {"MODEL",-15} {"YEAR",-6} {"IN USE",-8}");
            Console.WriteLine("-----------------------------------------------------------");
            Console.ResetColor();

            if (availableCars.Count > 0)
            {
                for (int i = 0; i < availableCars.Count; i++)
                {
                    Car car = availableCars[i];
                    Console.WriteLine($"{i + 1,-5} {car.Plate,-8} {car.Brand,-15} {car.Model,-15} {car.Year,-6} {(car.IsInUse ? "Yes" : "No"),-8}");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("No available cars.");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=====================================================\n");
            Console.ResetColor();

            // Rented cars
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("================== RENTED VEHICLES ==================");
            Console.WriteLine($"{"ID",-5} {"Plate",-8} {"BRAND",-15} {"MODEL",-15} {"YEAR",-6} {"IN USE",-8}");
            Console.WriteLine("-----------------------------------------------------------");
            Console.ResetColor();

            if (rentedCars.Count > 0)
            {
                for (int i = 0; i < rentedCars.Count; i++)
                {
                    Car car = rentedCars[i];
                    Console.WriteLine($"{i + 1,-5} {car.Plate,-8} {car.Brand,-15} {car.Model,-15} {car.Year,-6} {(car.IsInUse ? "Yes" : "No"),-8}");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("No rented cars.");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=====================================================\n");
            Console.ResetColor();
        }

        /// <summary>
        /// Rents a car to a user.
        /// </summary>
        /// <param name="dealerId">Dealer's unique identifier.</param>
        /// <param name="plate">Car plate (XXX-000).</param>
        /// <param name="userId">User ID (must be numeric).</param>
        /// <exception cref="FormatException">Thrown when the user ID is not numeric.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the car is not found or already rented.</exception>
        public static void RentCar(string dealerId, string plate, string userId)
        {
            plate = plate.ToUpperInvariant();
            userId = userId.Trim();

            if (!Regex.IsMatch(userId, @"^\d+$"))
            {
                throw new FormatException("User ID must contain only numbers.");
            }

            CarDealer dealer = GetDealerById(dealerId)!;
            Car? car = dealer.Cars.FirstOrDefault(c => c.Plate == plate);

            if (car == null)
            {
                throw new InvalidOperationException("Car not found.");
            }

            if (car.IsInUse)
            {
                throw new InvalidOperationException("Car is already rented.");
            }

            car.IsInUse = true;
            car.CurrentUser = userId;
            car.LastUsers.Add(userId);

            DealersConfig config = DealersConfig.Load();
            var dealerInConfig = config.Dealers.FirstOrDefault(d => d.Id == dealerId);
            if (dealerInConfig != null)
            {
                var carInConfig = dealerInConfig.Cars.FirstOrDefault(c => c.Plate == plate);
                if (carInConfig != null)
                {
                    carInConfig.IsInUse = true;
                    carInConfig.CurrentUser = userId;
                    carInConfig.LastUsers.Add(userId);
                }
            }
            DealersConfig.Save(config);
        }

        /// <summary>
        /// Returns a rented car, making it available again.
        /// </summary>
        /// <param name="dealerId">Dealer's unique identifier.</param>
        /// <param name="plate">Car plate (XXX-000).</param>
        /// <exception cref="InvalidOperationException">Thrown when the car is not found or is not currently rented.</exception>
        public static void ReturnCar(string dealerId, string plate)
        {
            plate = plate.ToUpperInvariant();

            CarDealer dealer = GetDealerById(dealerId)!;
            Car? car = dealer.Cars.FirstOrDefault(c => c.Plate == plate);

            if (car == null)
            {
                throw new InvalidOperationException("Car not found.");
            }

            if (!car.IsInUse)
            {
                throw new InvalidOperationException("Car is not currently rented.");
            }

            car.IsInUse = false;
            car.CurrentUser = null;

            DealersConfig config = DealersConfig.Load();
            var dealerInConfig = config.Dealers.FirstOrDefault(d => d.Id == dealerId);
            if (dealerInConfig != null)
            {
                var carInConfig = dealerInConfig.Cars.FirstOrDefault(c => c.Plate == plate);
                if (carInConfig != null)
                {
                    carInConfig.IsInUse = false;
                    carInConfig.CurrentUser = null;
                }
            }
            DealersConfig.Save(config);
        }
    }
}
