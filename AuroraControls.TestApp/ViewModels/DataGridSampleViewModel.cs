using System.Collections.ObjectModel;

namespace AuroraControls.TestApp.ViewModels;

public class DataGridSampleViewModel
{
    public ObservableCollection<Person> People { get; }

    public DataGridSampleViewModel()
    {
        People = new ObservableCollection<Person>
        {
            new Person
            {
                Name = "John Doe",
                Age = 30,
                DateOfBirth = new DateTime(1994, 5, 15),
                Salary = 75000.50m,
                IsActive = true,
            },
            new Person
            {
                Name = "Jane Smith",
                Age = 28,
                DateOfBirth = new DateTime(1996, 8, 22),
                Salary = 82500.75m,
                IsActive = true,
            },
            new Person
            {
                Name = "Bob Wilson",
                Age = 45,
                DateOfBirth = new DateTime(1979, 3, 10),
                Salary = 95000.25m,
                IsActive = false,
            },
            new Person
            {
                Name = "Alice Johnson",
                Age = 35,
                DateOfBirth = new DateTime(1989, 11, 30),
                Salary = 88750.00m,
                IsActive = true,
            },
        };
    }
}