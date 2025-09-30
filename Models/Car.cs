using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CarManagerCLI.Models
{
    /// <summary>
    /// Represents a car in the system. 
    /// Stores vehicle information such as plate, VIN, brand, model, year, color,
    /// and rental state (in use or available).
    /// </summary>
    public class Car
    {
        /// <summary>
        /// Unique identifier of the car (GUID).
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Vehicle plate (e.g., <c>ABC-123</c>).
        /// </summary>
        public string Plate { get; set; }

        /// <summary>
        /// Vehicle brand (e.g., <c>TOYOTA</c>).
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// Vehicle model (e.g., <c>Corolla</c>).
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Manufacturing year of the vehicle (must be 4 digits, e.g., <c>2020</c>).
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// Vehicle color (e.g., <c>Red</c>).
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Vehicle Identification Number (VIN), e.g., <c>1HGBH41JXMN109186</c>.
        /// </summary>
        public string Vin { get; set; }

        /// <summary>
        /// Indicates whether the car is currently rented (<c>true</c>) or available (<c>false</c>).
        /// </summary>
        public bool IsInUse { get; set; }

        /// <summary>
        /// User ID of the current renter. <c>null</c> if the car is not in use.
        /// </summary>
        public string? CurrentUser { get; set; }

        /// <summary>
        /// List of user IDs who previously rented this car (rental history).
        /// </summary>
        public List<string> LastUsers { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Car"/> class.
        /// </summary>
        /// <param name="plate">Vehicle plate in format <c>XXX-000</c>.</param>
        /// <param name="vin">Vehicle VIN (e.g., <c>1HGBH41JXMN109186</c>).</param>
        /// <param name="brand">Vehicle brand (e.g., <c>TOYOTA</c>).</param>
        /// <param name="model">Vehicle model (e.g., <c>Corolla</c>).</param>
        /// <param name="year">Manufacturing year (must be 4 digits, e.g., <c>2020</c>).</param>
        /// <param name="color">Vehicle color (e.g., <c>Red</c>).</param>
        /// <param name="isInUse">Indicates if the car is currently rented (default: <c>false</c>).</param>
        /// <param name="currentUser">User ID of the current renter, if any (default: <c>null</c>).</param>
        /// <param name="lastUsers">List of user IDs who previously rented the car (default: empty list).</param>
        public Car(string plate, string vin, string brand, string model, string year, string color, bool isInUse = false, string? currentUser = null, List<string>? lastUsers = null)
        {
            Id = Guid.NewGuid().ToString();
            Plate = plate;
            Vin = vin;
            Brand = brand;
            Model = model;
            Year = year;
            Color = color;
            IsInUse = isInUse;
            CurrentUser = currentUser;
            LastUsers = lastUsers ?? new List<string>();
        }

        /// <summary>
        /// Updates car information. 
        /// Any parameter left <c>null</c> will keep its current value.
        /// </summary>
        /// <param name="plate">New plate (optional).</param>
        /// <param name="brand">New brand (optional).</param>
        /// <param name="model">New model (optional).</param>
        /// <param name="year">New manufacturing year (optional).</param>
        /// <param name="color">New color (optional).</param>
        public void ChangeInformation(string? plate = null, string? brand = null, string? model = null, string? year = null, string? color = null)
        {
            this.Plate = plate ?? this.Plate;
            this.Brand = brand ?? this.Brand;
            this.Model = model ?? this.Model;
            this.Year = year ?? this.Year;
            this.Color = color ?? this.Color;
        }

        /// <summary>
        /// Validates if a plate matches the format <c>XXX-000</c>.
        /// </summary>
        /// <param name="plate">Plate to validate.</param>
        /// <returns><c>true</c> if the plate is valid; otherwise, <c>false</c>.</returns>
        public static bool CheckRegexPlate(string plate)
        {
            string plateRegex = @"^[A-Z]{3}-\d{3}$";
            return Regex.IsMatch(plate, plateRegex);
        }

        /// <summary>
        /// Validates if a year is valid (must be 4 digits).
        /// </summary>
        /// <param name="year">Year to validate.</param>
        /// <returns><c>true</c> if the year is valid; otherwise, <c>false</c>.</returns>
        public static bool CheckRegexYear(string year)
        {
            string YearRegex = @"^\d{4}$";
            return Regex.IsMatch(year, YearRegex);
        }
    }
}
