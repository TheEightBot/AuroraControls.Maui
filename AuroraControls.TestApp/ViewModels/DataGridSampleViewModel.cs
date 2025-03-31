using System.Collections.ObjectModel;

namespace AuroraControls.TestApp.ViewModels;

public class DataGridSampleViewModel
{
    private static readonly string[] FirstNames = { "John", "Jane", "Bob", "Alice", "Michael", "Sarah", "David", "Emily", "James", "Emma", "William", "Olivia", "Daniel", "Sophia", "Matthew", "Ava" };
    private static readonly string[] LastNames = { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Wilson", "Anderson", "Taylor", "Thomas", "Moore" };

    public ObservableCollection<Person> People { get; }

    public DataGridSampleViewModel()
    {
        var random = new Random(42); // Use fixed seed for reproducible data
        var startDate = new DateTime(1960, 1, 1);
        var endDate = new DateTime(2000, 12, 31);
        var dateRange = (endDate - startDate).Days;

        People = new ObservableCollection<Person>();

        for (int i = 0; i < 3000; i++)
        {
            var firstName = FirstNames[random.Next(FirstNames.Length)];
            var lastName = LastNames[random.Next(LastNames.Length)];
            var birthDate = startDate.AddDays(random.Next(dateRange));
            var age = (int)((DateTime.Now - birthDate).TotalDays / 365.25);
            var baseSalary = 50000m;
            var salaryVariation = random.Next(-10000, 30001);

            People.Add(new Person
            {
                Name = $"{firstName} {lastName}",
                Age = age,
                DateOfBirth = birthDate,
                Salary = baseSalary + salaryVariation + (random.Next(100) / 100m),
                IsActive = random.Next(100) < 85, // 85% chance of being active
            });
        }
    }
}
