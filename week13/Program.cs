using System;
using System.Collections.Generic;
using System.Linq;

public abstract class Asset // Abstrakt klass för att skapa en gemensam klass för alla Assets (Laptop och mobil)
{
    public string Brand { get; set; } // Varumärke
    public string Model { get; set; } // Modell
    public DateTime PurchaseDate { get; set; } // Inköpsdatum
    public double PriceInUSD { get; set; } // Pris i USD som senare används för att konvertera till lokal valuta
    public string AssetType { get; set; } // Typ av tillgång (Laptop eller mobil)
    public Office OfficeLocation { get; set; } // Lägg till OfficeLocation direkt här

    public Asset(string brand, string model, DateTime purchaseDate, double priceInUSD, Office office) // Konstruktor för att skapa en ny asset
    {
        Brand = brand;
        Model = model;
        PurchaseDate = purchaseDate;
        PriceInUSD = priceInUSD;
        OfficeLocation = office;
        AssetType = this.GetType().Name; // Dynamisk typ beroende på subklass
    }

    public abstract void PrintDetails(); // Abstrakt metod för att skriva ut detaljer om tillgången

    protected void SetColorBasedOnAge(DateTime purchaseDate) // Centraliserad metod för att sätta färg baserat på tillgångens ålder
    {
        int yearsSincePurchase = DateTime.Now.Year - purchaseDate.Year;
        int monthsSincePurchase = DateTime.Now.Month - purchaseDate.Month;

        if (monthsSincePurchase < 0)
        {
            yearsSincePurchase--;
            monthsSincePurchase += 12;
        }

        // Färgsättning baserat på antal år och månader
        if (yearsSincePurchase > 3)
        {
            Console.ForegroundColor = ConsoleColor.Black; // Svart för mer än 3 år
        }
        else if (yearsSincePurchase == 3 && monthsSincePurchase > 0)
        {
            Console.ForegroundColor = ConsoleColor.Red; // Röd för mer än 2 år och 9 månader men mindre än 3 år
        }
        else if (yearsSincePurchase == 2 && monthsSincePurchase >= 6)
        {
            Console.ForegroundColor = ConsoleColor.Yellow; // Gul för mer än 2 år och 6 månader men mindre än 2 år och 9 månader
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.White; // Vit för mindre än 2 år och 6 månader
        }
    }
}

public class Laptop : Asset // Subklass för att skapa en ny tillgång av typen Laptop
{
    public Laptop(string brand, string model, DateTime purchaseDate, double priceInUSD, Office office)
        : base(brand, model, purchaseDate, priceInUSD, office) {}

    public override void PrintDetails() // Överskugga metoden PrintDetails för att skriva ut detaljer om en laptop
    {
        double localPrice = CurrencyConverter.ConvertToCurrency(PriceInUSD, OfficeLocation.Location); // Konvertera priset till lokal valuta
        string currencySymbol = CurrencyConverter.GetCurrencySymbol(OfficeLocation.Location); // Hämta valutasymbol

        // Anropa den centraliserade metoden för att sätta färgen
        SetColorBasedOnAge(PurchaseDate);

        // Användning av jämn bredd i alla kolumner
        Console.WriteLine($"{OfficeLocation.Location,-12} | {this.GetType().Name,-15} | {Brand,-15} | {Model,-15} | {PriceInUSD,-15:F2} | {localPrice,-15:F2} | {currencySymbol,-3} | {PurchaseDate.ToShortDateString(),-12}");

        Console.ResetColor();
    }
}

public class Mobil : Asset // Subklass för mobil
{
    public Mobil(string brand, string model, DateTime purchaseDate, double priceInUSD, Office office)
        : base(brand, model, purchaseDate, priceInUSD, office) {}

    public override void PrintDetails()  // Överskugga metoden PrintDetails för att skriva ut detaljer om en mobil
    {
        double localPrice = CurrencyConverter.ConvertToCurrency(PriceInUSD, OfficeLocation.Location);
        string currencySymbol = CurrencyConverter.GetCurrencySymbol(OfficeLocation.Location);

        // Anropa den centraliserade metoden för att sätta färgen
        SetColorBasedOnAge(PurchaseDate);

        // Användning av jämn bredd i alla kolumner
        Console.WriteLine($"{OfficeLocation.Location,-12} | {this.GetType().Name,-15} | {Brand,-15} | {Model,-15} | {PriceInUSD,-15:F2} | {localPrice,-15:F2} | {currencySymbol,-3} | {PurchaseDate.ToShortDateString(),-12}");

        Console.ResetColor();
    }
}

public static class CurrencyConverter // Klass för att konvertera priser till lokal valuta
{
    public static double ConvertToCurrency(double priceInUSD, string country) // Metod för att konvertera priser till lokal valuta
    {
        double conversionRate = 1;

        switch (country.ToLower()) // Konverteringsfaktorer för olika länder
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

        return priceInUSD * conversionRate; // Konvertera priset till lokal valuta
    }

    public static string GetCurrencySymbol(string country) // Metod för att Skriva in valutasymbol istället för Land
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

public class Office // klass för att skapa kontor
{
    public string Location { get; set; }

    public Office(string location)
    {
        Location = location;
    }
}

class Program // Huvudklassen för att köra programmet
{
    static void Main(string[] args) // Main-metoden för att köra programmet
    {
        List<Asset> assets = new List<Asset>(); // Lista för att lagra tillgångar

        bool addMoreAssets = true; // Variabel för att lägga till fler tillgångar

        while (addMoreAssets) // Loop för att lägga till fler tillgångar
        {
            Console.WriteLine("Welcome to the Asset Tracking System!");

            // Skapa tillgångar och kontor via inmatning i konsolen
            try
            {
                // Ange kontor utan att behöva ange valuta
                Office office = GetValidOffice();

                // Ange typ av tillgång, varumärke, modell, inköpsdatum och pris i USD
                string assetType = GetValidAssetType();
                string brand = GetValidBrand();
                string model = GetValidModel();
                DateTime purchaseDate = GetValidPurchaseDate();
                double priceInUSD = GetValidPriceInUSD();

                // Skapa tillgång med kontor
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

                // Fråga om användaren vill lägga till fler tillgångar
                Console.Write("Do you want to add another asset? (yes/no): ");
                addMoreAssets = Console.ReadLine().ToLower() == "yes";
            }
            catch (Exception ex) // Fånga fel och skriv ut felmeddelande
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            if (!addMoreAssets) // Om användaren inte vill lägga till fler tillgångar
            {
                // Visa tillgångar med kontor och konverterad valuta i tabellform
                Console.WriteLine("\nList of assets with currency conversion:");

                // Visa tabellrubriker
                Console.WriteLine($"{"Office",-12} | {"Asset Type",-15} | {"Brand",-15} | {"Model",-15} | {"Price (USD)",-15} | {"Price (Local)",-15} | {"Office Currency",-3} | {"Purchase Date",-12}");
                Console.WriteLine(new string('-', 120)); // Separationslinje

                // Visa alla tillgångar
                foreach (var asset in assets)
                {
                    asset.PrintDetails();
                }

                // Fråga om användaren vill lägga till fler tillgångar
                Console.Write("Do you want to add more products? (yes/no): ");
                string response = Console.ReadLine().ToLower();
                if (response == "yes")
                {
                    addMoreAssets = true;
                }
            }
        }
    }
    // Metoder för validering av inmatning
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
    static string GetValidBrand() // Metod för att validera varumärke
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

    static string GetValidModel() // Metod för att validera modell
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

    static DateTime GetValidPurchaseDate() // Metod för att validera inköpsdatum
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

    static double GetValidPriceInUSD() // Metod för att validera pris i USD
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

    static Office GetValidOffice() // Metod för att validera kontor
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