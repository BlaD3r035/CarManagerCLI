using System;
using System.Collections.Generic;

namespace CarManagerCLI.Models
{
    /// <summary>
    /// Represents a car dealer in the system.
    /// A dealer has an identifier, a name, and a list of cars under its management.
    /// </summary>
    public class CarDealer
    {
        /// <summary>
        /// Unique identifier of the dealer (GUID).
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Dealer's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of cars that belong to the dealer.
        /// </summary>
        public List<Car> Cars { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CarDealer"/> class.
        /// </summary>
        /// <param name="name">Dealer's name.</param>
        public CarDealer(string name)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Cars = new List<Car>();
        }
    }
}
