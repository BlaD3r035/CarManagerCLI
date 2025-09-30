using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using CarManagerCLI.Models;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CarManagerCLI.Config
{
   
    public class Config
    {
        /// <summary>
        /// Configuration type (e.g., Dealers, Session).
        /// </summary>
        public string Type { get; set; } = "Default";

        /// <summary>
        /// Description of the configuration.
        /// </summary>
        public string Description { get; set; } = "Default";
    }

    /// <summary>
    /// Configuration manager for car dealers.
    /// Handles persistence of dealers and their cars in JSON format.
    /// </summary>
    public class DealersConfig : Config
    {
        static string DataPath = Path.Combine(Environment.CurrentDirectory, "Data");
        static string DealersPath = Path.Combine(DataPath, "Dealers.json");

        /// <summary>
        /// List of registered car dealers.
        /// </summary>
        public List<CarDealer> Dealers { get; set; }

        /// <summary>
        /// Default constructor. Initializes type, description, and an empty dealers list.
        /// </summary>
        public DealersConfig()
        {
            Type = "Dealers";
            Description = "Dealers list";
            Dealers = new List<CarDealer>();
        }

        private DealersConfig(string type, string description, List<CarDealer> dealers)
        {
            Type = type;
            Description = description;
            Dealers = dealers;
        }

        /// <summary>
        /// Creates the config file if it does not exist.
        /// </summary>
        static private void CreateConfigFile()
        {
            if (!Directory.Exists(DataPath))
            {
                Directory.CreateDirectory(DataPath);
            }
            if (!File.Exists(DealersPath))
            {
                List<CarDealer> list = new List<CarDealer>();
                DealersConfig data = new DealersConfig("Dealers", "Dealers list", list);
                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(DealersPath, json);
            }
        }

        /// <summary>
        /// Loads dealer configuration from the JSON file.
        /// </summary>
        /// <returns>An instance of <see cref="DealersConfig"/> with the loaded data.</returns>
        static public DealersConfig Load()
        {
            CreateConfigFile();
            string json = File.ReadAllText(DealersPath);
            return JsonSerializer.Deserialize<DealersConfig>(json) ??
                new DealersConfig("Dealers", "Dealers list", new List<CarDealer>());
        }

        /// <summary>
        /// Saves dealer configuration to the JSON file.
        /// </summary>
        /// <param name="config">Dealer configuration to save.</param>
        /// <returns><c>true</c> if the save was successful; otherwise, <c>false</c>.</returns>
        static public bool Save(DealersConfig config)
        {
            try
            {
                string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(DealersPath, json);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Configuration manager for application sessions.
    /// Handles the currently logged-in dealer and presence state.
    /// </summary>
    public class SessionConfig : Config
    {
        static string DataPath = Path.Combine(Environment.CurrentDirectory, "Data");
        static string SessionPath = Path.Combine(DataPath, "Session.json");

        /// <summary>
        /// ID of the dealer who is currently logged in.
        /// </summary>
        public string? DealerId { get; set; }

        /// <summary>
        /// Indicates if session Presence is Enabled or not
        /// </summary>
        public bool Presence { get; set; } = false;

        /// <summary>
        /// Default constructor. Initializes type, description, and session values.
        /// </summary>
        public SessionConfig()
        {
            Type = "Session";
            Description = "Session file";
            DealerId = null;
            Presence = false;
        }

        private SessionConfig(string type, string description, string? dealerId, bool presence = false)
        {
            Type = type;
            Description = description;
            DealerId = dealerId;
            Presence = presence;
        }

        /// <summary>
        /// Creates the session config file if it does not exist.
        /// </summary>
        static private void CreateConfigFile()
        {
            if (!Directory.Exists(DataPath))
            {
                Directory.CreateDirectory(DataPath);
            }
            if (!File.Exists(SessionPath))
            {
                SessionConfig data = new SessionConfig("Session", "Session config", null);
                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SessionPath, json);
            }
        }

        /// <summary>
        /// Logs in with a specific dealer.
        /// </summary>
        /// <param name="dealer">Dealer who is logging in.</param>
        /// <param name="presence">Session Presence.</param>
        /// <returns>The active session configuration.</returns>
        public static SessionConfig LogIn(CarDealer dealer, bool presence = false)
        {
            CreateConfigFile();
            SessionConfig session = Load();

            if (session.Presence)
            {
                return session;
            }

            SessionConfig data = new SessionConfig("Session", "Session config", dealer.Id, presence);
            Save(data);
            return data;
        }

        /// <summary>
        /// Logs out of the current session.
        /// </summary>
        public static void LogOut()
        {
            CreateConfigFile();
            SessionConfig data = new SessionConfig("Session", "Session config", null, false);
            Save(data);
        }

        /// <summary>
        /// Updates the presence status of the current session.
        /// </summary>
        /// <param name="presence">New presence status.</param>
        public static void SetPresence(bool presence)
        {
            CreateConfigFile();
            SessionConfig session = Load();
            session.Presence = presence;
            Save(session);
        }

        /// <summary>
        /// Retrieves the currently active session.
        /// </summary>
        /// <returns>The active session, or <c>null</c> if none exists.</returns>
        public static SessionConfig? GetActiveSession()
        {
            CreateConfigFile();
            SessionConfig session = Load();
            return session.DealerId != null ? session : null;
        }

        /// <summary>
        /// Loads session configuration from the JSON file.
        /// </summary>
        private static SessionConfig Load()
        {
            string json = File.ReadAllText(SessionPath);
            return JsonSerializer.Deserialize<SessionConfig>(json)!;
        }

        /// <summary>
        /// Saves session configuration to the JSON file.
        /// </summary>
        private static void Save(SessionConfig data)
        {
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SessionPath, json);
        }
    }
}
