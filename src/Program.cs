using System;
using System.Collections.Generic;
using System.Linq;

public abstract class Asset // Abstract class to create a shared base for all assets (Laptop and Mobile)
{
    public string Brand { get; set; } // Brand
    public string Model { get; set; } // Model
    public DateTime PurchaseDate { get; set; } // Purchase date
    public double PriceInUSD { get; set; } // Price in USD, later used to convert to local currency
    public string AssetType { get; set; } // Type of asset (Laptop or Mobile)
    public Office OfficeLocation { get; set; } // Include OfficeLocation directly here

    public Asset(string brand, string model, DateTime purchaseDate, double priceInUSD, Office office) // Constructor to create a new asset
    {
        Brand = brand;
        Model = model;
        PurchaseDate = purchaseDate;
        PriceInUSD = priceInUSD;
        OfficeLocation = office;
        AssetType = this.GetType().Name; // Dynamic type based on subclass
    }

    public abstract void PrintDetails(); // Abstract method to print asset details

    protected void SetColorBasedOnAge(DateTime purchaseDate) // Centralized method to set color based on asset age
    {
        int yearsSincePurchase = DateTime.Now.Year - purchaseDate.Year;
        int monthsSincePurchase = DateTime.Now.Month - purchaseDate.Month;

        if (monthsSincePurchase < 0)
        {
            yearsSincePurchase--;
            monthsSincePurchase += 12;
        }

        // Set color based on years and months
        if (yearsSincePurchase > 3)
        {
            Console.ForegroundColor = ConsoleColor.Black; // Black for more than 3 years
        }
        else if (yearsSincePurchase == 3 && monthsSincePurchase > 0)
        {
            Console.ForegroundColor = ConsoleColor.Red; // Red for more than 2 years and 9 months but less than 3 years
        }
        else if (yearsSincePurchase == 2 && monthsSincePurchase >= 6)
        {
            Console.ForegroundColor = ConsoleColor.Yellow; // Yellow for more than 2 years and 6 months but less than 2 years and 9 months
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.White; // White for less than 2 years and 6 months
        }
    }
}

public class Laptop : Asset // Subclass to create a new asset of type Laptop
{
    public Laptop(string brand, string model, DateTime purchaseDate, double priceInUSD, Office office)
        : base(brand, model, purchaseDate, priceInUSD, office) {}

    public override void PrintDetails() // Override PrintDetails to print laptop details
    {
        double localPrice = CurrencyConverter.ConvertToCurrency(PriceInUSD, OfficeLocation.Location); // Convert price to local currency
        string currencySymbol = CurrencyConverter.GetCurrencySymbol(OfficeLocation.Location); // Get currency symbol

        // Call the centralized method to set the color
        SetColorBasedOnAge(PurchaseDate);

        // Use fixed width for all columns
        Console.WriteLine($"{OfficeLocation.Location,-12} | {this.GetType().Name,-15} | {Brand,-15} | {Model,-15} | {PriceInUSD,-15:F2} | {localPrice,-15:F2} | {currencySymbol,-3} | {PurchaseDate.ToShortDateString(),-12}");

        Console.ResetColor();
    }
}

public class Mobil : Asset // Subclass for Mobile
{
    public Mobil(string brand, string model, DateTime purchaseDate, double priceInUSD, Office office)
        : base(brand, model, purchaseDate, priceInUSD, office) {}

    public override void PrintDetails()  // Override PrintDetails to print mobile details
    {
        double localPrice = CurrencyConverter.ConvertToCurrency(PriceInUSD, OfficeLocation.Location);
        string currencySymbol = CurrencyConverter.GetCurrencySymbol(OfficeLocation.Location);

        // Call the centralized method to set the color
        SetColorBasedOnAge(PurchaseDate);

        // Use fixed width for all columns
        Console.WriteLine($"{OfficeLocation.Location,-12} | {this.GetType().Name,-15} | {Brand,-15} | {Model,-15} | {PriceInUSD,-15:F2} | {localPrice,-15:F2} | {currencySymbol,-3} | {PurchaseDate.ToShortDateString(),-12}");

        Console.ResetColor();
    }
}

public static class CurrencyConverter // Class for converting prices to local currency
{
    public static double ConvertToCurrency(double priceInUSD, string country) // Method to convert price to local currency
    {
        double conversionRate = 1;

        switch (country.ToLower()) // Conversion rates for different countries
        {
            case "sverige":
                conversionRate = 9.5;  // SEK
                break;
            case "tyskland":
                conversionRate = 0.85; // EUR
                break;
            case "usa":
                conversionRate = 1.0;  // USD
                break;
            default:
                throw new Exception("Unsupported country");
        }

        return priceInUSD * conversionRate; // Convert price to local currency
    }

    public static string GetCurrencySymbol(string country) // Method to return currency symbol instead of country name
    {
        switch (country.ToLower())
        {
            case "sverige":
                return "SEK";
            case "tyskland":
                return "EUR";
            case "usa":
                return "USD";
            default:
                return "Unknown";
        }
    }
}

public class Office // Class to create an office
{
    public string Location { get; set; }

    public Office(string location)
    {
        Location = location;
    }
}

class Program // Main class to run the program
{
    static void Main(string[] args) // Main method to run the program
    {
        List<Asset> assets = new List<Asset>(); // List to store assets

        bool addMoreAssets = true; // Variable to control whether to add more assets

        while (addMoreAssets) // Loop to add multiple assets
        {
            Console.WriteLine("Welcome to the Asset Tracking System!");

            // Create assets and offices via console input
            try
            {
                // Specify office without needing to specify currency
                Office office = GetValidOffice();

                // Specify asset type, brand, model, purchase date, and price in USD
                string assetType = GetValidAssetType();
                string brand = GetValidBrand();
                string model = GetValidModel();
                DateTime purchaseDate = GetValidPurchaseDate();
                double priceInUSD = GetValidPriceInUSD();

                // Create asset with office
                Asset asset = null;
                if (assetType.ToLower() == "laptop")
                {
                    asset = new Laptop(brand, model, purchaseDate, priceInUSD, office);
                }
                else if (assetType.ToLower() == "mobil")
                {
                    asset = new Mobil(brand, model, purchaseDate, priceInUSD, office);
                }

                assets.Add(asset);

                // Ask if user wants to add more assets
                Console.Write("Do you want to add another asset? (yes/no): ");
                addMoreAssets = Console.ReadLine().ToLower() == "yes";
            }
            catch (Exception ex) // Catch errors and display error message
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            if (!addMoreAssets) // If user does not want to add more assets
            {
                // Show assets with office and converted currency in table form
                Console.WriteLine("\nList of assets with currency conversion:");

                // Display table headers
                Console.WriteLine($"{"Office",-12} | {"Asset Type",-15} | {"Brand",-15} | {"Model",-15} | {"Price (USD)",-15} | {"Price (Local)",-15} | {"Office Currency",-3} | {"Purchase Date",-12}");
                Console.WriteLine(new string('-', 120)); // Divider line

                // Display all assets
                foreach (var asset in assets)
                {
                    asset.PrintDetails();
                }

                // Ask if user wants to add more products
                Console.Write("Do you want to add more products? (yes/no): ");
                string response = Console.ReadLine().ToLower();
                if (response == "yes")
                {
                    addMoreAssets = true;
                }
            }
        }
    }

    // Methods for input validation
    static string GetValidAssetType()
    {
        string assetType;
        while (true)
        {
            Console.Write("Enter asset type (Laptop/Mobil): ");
            assetType = Console.ReadLine().ToLower();
            if (assetType == "laptop" || assetType == "mobil")
                break;
            else
                Console.WriteLine("Invalid asset type. Please enter either 'Laptop' or 'mobil'.");
        }
        return assetType;
    }

    static string GetValidBrand() // Method to validate brand
    {
        string brand;
        while (true)
        {
            Console.Write("Enter asset brand: ");
            brand = Console.ReadLine();
            if (!string.IsNullOrEmpty(brand))
                break;
            else
                Console.WriteLine("Brand cannot be empty.");
        }
        return brand;
    }

    static string GetValidModel() // Method to validate model
    {
        string model;
        while (true)
        {
            Console.Write("Enter asset model: ");
            model = Console.ReadLine();
            if (!string.IsNullOrEmpty(model))
                break;
            else
                Console.WriteLine("Model cannot be empty.");
        }
        return model;
    }

    static DateTime GetValidPurchaseDate() // Method to validate purchase date
    {
        DateTime purchaseDate;
        while (true)
        {
            Console.Write("Enter purchase date (yyyy-mm-dd): ");
            if (DateTime.TryParse(Console.ReadLine(), out purchaseDate))
                break;
            else
                Console.WriteLine("Invalid date format. Please use 'yyyy-mm-dd'.");
        }
        return purchaseDate;
    }

    static double GetValidPriceInUSD() // Method to validate price in USD
    {
        double price;
        while (true)
        {
            Console.Write("Enter price in USD: ");
            if (double.TryParse(Console.ReadLine(), out price) && price > 0)
                break;
            else
                Console.WriteLine("Invalid price. Please enter a valid positive number.");
        }
        return price;
    }

    static Office GetValidOffice() // Method to validate office location
    {
        string location;
        while (true)
        {
            Console.Write("Enter office location (Sverige, Tyskland, USA): ");
            location = Console.ReadLine().ToLower();
            if (location == "sverige" || location == "tyskland" || location == "usa")
                break;
            else
                Console.WriteLine("Invalid location. Please enter 'Sverige', 'Tyskland', or 'USA'.");
        }
        return new Office(location);
    }
}
