using System;
using System.Collections.Generic;
using System.IO;

public class Vehicle
{
    public int Id { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public double RentalPricePerDay { get; set; }

    public Vehicle(int id, string make, string model, int year, double rentalPricePerDay)
    {
        Id = id;
        Make = make;
        Model = model;
        Year = year;
        RentalPricePerDay = rentalPricePerDay;
    }
}

public class Car : Vehicle
{
    public Car(int id, string make, string model, int year, double rentalPricePerDay)
        : base(id, make, model, year, rentalPricePerDay)
    {
    }
}

public class Rental
{
    public int CarId { get; set; }
    public string CustomerName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public Rental(int carId, string customerName, DateTime startDate, DateTime endDate)
    {
        CarId = carId;
        CustomerName = customerName;
        StartDate = startDate.Date;
        EndDate = endDate.Date;
    }
}

public class CarRentalService
{
    public List<Car> Cars { get; set; } = new List<Car>();
    public List<Rental> Rentals { get; set; } = new List<Rental>();

    public void AddCar(Car car)
    {
        Cars.Add(car);
    }

    public void AddRental(Rental rental)
    {
        Rentals.Add(rental);
    }

    public void SaveToSql(string filename)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.WriteLine("CREATE TABLE IF NOT EXISTS Cars (");
                writer.WriteLine("    Id INT PRIMARY KEY,");
                writer.WriteLine("    Make VARCHAR(255),");
                writer.WriteLine("    Model VARCHAR(255),");
                writer.WriteLine("    Year INT,");
                writer.WriteLine("    RentalPricePerDay DOUBLE");
                writer.WriteLine(");");
                writer.WriteLine();
                writer.WriteLine("CREATE TABLE IF NOT EXISTS Rentals (");
                writer.WriteLine("    CarId INT,");
                writer.WriteLine("    CustomerName VARCHAR(255),");
                writer.WriteLine("    StartDate DATE,");
                writer.WriteLine("    EndDate DATE,");
                writer.WriteLine("    FOREIGN KEY (CarId) REFERENCES Cars(Id)");
                writer.WriteLine(");");
                writer.WriteLine();

                foreach (var car in Cars)
                {
                    writer.WriteLine($"INSERT INTO Cars (Id, Make, Model, Year, RentalPricePerDay) VALUES ({car.Id}, '{car.Make}', '{car.Model}', {car.Year}, {car.RentalPricePerDay});");
                }

                foreach (var rental in Rentals)
                {
                    writer.WriteLine($"INSERT INTO Rentals (CarId, CustomerName, StartDate, EndDate) VALUES ({rental.CarId}, '{rental.CustomerName}', '{rental.StartDate:yyyy-MM-dd}', '{rental.EndDate:yyyy-MM-dd}');");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while saving to SQL file: {ex.Message}");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        CarRentalService rentalService = new CarRentalService();

        while (true)
        {
            Console.WriteLine("1. Add Car");
            Console.WriteLine("2. Add Rental");
            Console.WriteLine("3. Display All Cars");
            Console.WriteLine("4. Display Car Details");
            Console.WriteLine("5. Display Rental Details");
            Console.WriteLine("6. Calculate Rental Cost");
            Console.WriteLine("7. Save to SQL");
            Console.WriteLine("0. Exit");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Invalid input, please enter a number.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    AddCar(rentalService);
                    break;
                case 2:
                    AddRental(rentalService);
                    break;
                case 3:
                    DisplayAllCars(rentalService);
                    break;
                case 4:
                    DisplayCarDetails(rentalService);
                    break;
                case 5:
                    DisplayRentalDetails(rentalService);
                    break;
                case 6:
                    CalculateRentalCost(rentalService);
                    break;
                case 7:
                    SaveToSql(rentalService);
                    break;
                case 0:
                    return;
                default:
                    Console.WriteLine("Invalid choice, try again.");
                    break;
            }
        }
    }

    static void AddCar(CarRentalService rentalService)
    {
        try
        {
            Console.Write("Enter car ID: ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("Enter car make: ");
            string make = Console.ReadLine();
            Console.Write("Enter car model: ");
            string model = Console.ReadLine();
            Console.Write("Enter car year: ");
            int year = int.Parse(Console.ReadLine());
            Console.Write("Enter rental price per day: ");
            double rentalPricePerDay = double.Parse(Console.ReadLine());

            Car car = new Car(id, make, model, year, rentalPricePerDay);
            rentalService.AddCar(car);
            Console.WriteLine("Car added successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while adding car: {ex.Message}");
        }
    }

    static void AddRental(CarRentalService rentalService)
    {
        try
        {
            Console.Write("Enter car ID: ");
            int carId = int.Parse(Console.ReadLine());
            Console.Write("Enter customer name: ");
            string customerName = Console.ReadLine();
            Console.Write("Enter rental start date (yyyy-MM-dd): ");
            DateTime startDate = DateTime.Parse(Console.ReadLine());
            Console.Write("Enter rental end date (yyyy-MM-dd): ");
            DateTime endDate = DateTime.Parse(Console.ReadLine());

            Rental rental = new Rental(carId, customerName, startDate, endDate);
            rentalService.AddRental(rental);
            Console.WriteLine("Rental added successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while adding rental: {ex.Message}");
        }
    }

    static void DisplayAllCars(CarRentalService rentalService)
    {
        var cars = rentalService.Cars;
        if (cars.Count > 0)
        {
            foreach (var car in cars)
            {
                Console.WriteLine($"ID: {car.Id}, Make: {car.Make}, Model: {car.Model}, Year: {car.Year}, Rental Price per Day: {car.RentalPricePerDay}");
            }
        }
        else
        {
            Console.WriteLine("No cars found.");
        }
    }

    static void DisplayCarDetails(CarRentalService rentalService)
    {
        try
        {
            Console.Write("Enter car ID: ");
            int carId = int.Parse(Console.ReadLine());
            var car = rentalService.Cars.Find(c => c.Id == carId);

            if (car != null)
            {
                Console.WriteLine($"Make: {car.Make}, Model: {car.Model}, Year: {car.Year}, Rental Price per Day: {car.RentalPricePerDay}");
            }
            else
            {
                Console.WriteLine("Car not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while displaying car details: {ex.Message}");
        }
    }

    static void DisplayRentalDetails(CarRentalService rentalService)
    {
        try
        {
            Console.Write("Enter car ID: ");
            int carId = int.Parse(Console.ReadLine());
            Console.Write("Enter customer name: ");
            string customerName = Console.ReadLine();

            var rentals = rentalService.Rentals.FindAll(r => r.CarId == carId && r.CustomerName == customerName);

            if (rentals.Count > 0)
            {
                foreach (var rental in rentals)
                {
                    Console.WriteLine($"Customer: {rental.CustomerName}, Start Date: {rental.StartDate:yyyy-MM-dd}, End Date: {rental.EndDate:yyyy-MM-dd}");
                }
            }
            else
            {
                Console.WriteLine("Rental not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while displaying rental details: {ex.Message}");
        }
    }

    static void CalculateRentalCost(CarRentalService rentalService)
    {
        try
        {
            Console.Write("Enter car ID: ");
            int carId = int.Parse(Console.ReadLine());
            Console.Write("Enter number of rental days: ");
            int days = int.Parse(Console.ReadLine());

            var car = rentalService.Cars.Find(c => c.Id == carId);

            if (car != null)
            {
                double totalCost = car.RentalPricePerDay * days;
                Console.WriteLine($"Total rental cost for {days} days: {totalCost}");
            }
            else
            {
                Console.WriteLine("Car not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while calculating rental cost: {ex.Message}");
        }
    }

    static void SaveToSql(CarRentalService rentalService)
    {
        try
        {
            Console.Write("Enter filename to save: ");
            string filename = Console.ReadLine();
            rentalService.SaveToSql(filename);
            Console.WriteLine("Data saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while saving data: {ex.Message}");
        }
    }
}
