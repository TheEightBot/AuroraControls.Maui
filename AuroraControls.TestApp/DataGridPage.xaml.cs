using System.Collections.ObjectModel;

namespace AuroraControls.TestApp
{
    public partial class DataGridPage : ContentPage
    {
        private ObservableCollection<Person> _people;
        private Random _random = new Random();

        public DataGridPage()
        {
            InitializeComponent();

            // Generate columns for the complex model
            DataGrid.Columns = new List<GridColumn>
            {
                new GridColumn { Title = "Name", Width = 150 },
                new GridColumn { Title = "Age", Width = 100 },
                new GridColumn { Title = "Country", Width = 150 },
                new GridColumn { Title = "Is Active", Width = 100 },
                new GridColumn { Title = "Editable Text", Width = 200 },
            };

            var countries = new[] { "USA", "Canada", "UK", "Australia", "Germany" };

            // Generate a more complex data model using ObservableCollection
            _people = new ObservableCollection<Person>(
                Enumerable.Range(1, 100).Select(i => new Person
                {
                    Name = $"Person {i}",
                    Age = 20 + (i % 50),
                    Country = countries[i % 5],
                    IsActive = i % 2 == 0,
                    EditableText = $"Editable {i}",
                }));

            // Set the ItemsSource to the ObservableCollection
            DataGrid.ItemsSource = _people;

            // Define the CellTemplate to render different cell types
            DataGrid.CellTemplate =
                (object item, int col) =>
                {
                    var person = item as Person;
                    return col switch
                    {
                        0 => new TextCell { Text = person?.Name ?? string.Empty },
                        1 => new NumberCell { Number = person?.Age ?? 0 },
                        2 => new TextCell { Text = person?.Country ?? string.Empty },
                        3 => new CheckboxCell { IsChecked = person?.IsActive ?? false },
                        4 => new EditableTextCell
                        {
                            Text = person?.EditableText ?? string.Empty,
                            TextChanged = (rowIndex, colIndex, newText) =>
                            {
                                if (person != null)
                                {
                                    person.EditableText = newText;
                                }
                            },
                        },
                        _ => (DataGridCell)null,
                    };
            };
        }

        private void UpdateButton_Clicked(object sender, EventArgs e)
        {
            if (_people == null || _people.Count == 0)
            {
                return;
            }

            var countries = new[] { "USA", "Canada", "UK", "Australia", "Germany" };

            // Perform random updates to demonstrate ObservableCollection change detection
            for (int i = 0; i < 5; i++)
            {
                int action = _random.Next(4);
                switch (action)
                {
                    case 0: // Add a new item
                        int newId = _people.Count + 1;
                        _people.Add(new Person
                        {
                            Name = $"New Person {newId}",
                            Age = 20 + _random.Next(50),
                            Country = countries[_random.Next(5)],
                            IsActive = _random.Next(2) == 0,
                            EditableText = $"New Editable {newId}",
                        });
                        break;

                    case 1: // Remove an item
                        if (_people.Count > 1)
                        {
                            int removeIndex = _random.Next(_people.Count);
                            _people.RemoveAt(removeIndex);
                        }

                        break;

                    case 2: // Update an item
                        int updateIndex = _random.Next(_people.Count);
                        _people[updateIndex].Age = 20 + _random.Next(50);
                        _people[updateIndex].IsActive = !_people[updateIndex].IsActive;
                        _people[updateIndex].Country = countries[_random.Next(5)];

                        // For property changed to work, Person would need to implement INotifyPropertyChanged
                        // This just updates the value in the object
                        break;

                    case 3: // Replace an item
                        int replaceIndex = _random.Next(_people.Count);
                        _people[replaceIndex] = new Person
                        {
                            Name = $"Replaced Person {replaceIndex}",
                            Age = 20 + _random.Next(50),
                            Country = countries[_random.Next(5)],
                            IsActive = _random.Next(2) == 0,
                            EditableText = $"Replaced Editable {replaceIndex}",
                        };
                        break;
                }
            }
        }

        public class Person
        {
            public string Name { get; set; } = string.Empty;

            public int Age { get; set; }

            public string Country { get; set; } = string.Empty;

            public bool IsActive { get; set; }

            public string EditableText { get; set; } = string.Empty;
        }
    }
}
