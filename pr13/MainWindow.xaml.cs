using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace pr13
{
    public partial class MainWindow : Window
    { 

        private int[,] matrix;
        private string FilePath = "matrix.txt";
        public MainWindow()
        {
            InitializeComponent();
        }
        private void CreateMatrixButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(RowsTextBox.Text, out int rows) && int.TryParse(ColsTextBox.Text, out int cols))
            {
                InputDataGrid.Columns.Clear();
                matrix = new int[rows, cols];

                for (int col = 0; col < cols; col++)
                {
                    InputDataGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = $"Столбец {col + 1}",
                        Binding = new System.Windows.Data.Binding($"[{col}]")
                    });
                }

                var itemsSource = new ObservableCollection<int[]>();
                for (int row = 0; row < rows; row++)
                {
                    var rowData = new int[cols];
                    itemsSource.Add(rowData);
                }

                InputDataGrid.ItemsSource = itemsSource;
                UpdateStatus();
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректные размеры матрицы.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateStatus()
        {
            StatusTextBlock.Text = $"Размер таблицы: {matrix?.GetLength(0) ?? 0} x {matrix?.GetLength(1) ?? 0}";
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(FilePath))
            {
                var numbers = File.ReadAllLines(FilePath).Select(int.Parse).ToArray();
                matrix = new int[numbers.Length, 1];
                for (int i = 0; i < numbers.Length; i++)
                {
                    matrix[i, 0] = numbers[i];
                }
                InputDataGrid.ItemsSource = numbers.Select(n => new { Value = n }).ToList();
                UpdateStatus();
            }
        }
    

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (StreamWriter writer = new StreamWriter(FilePath))
            {
                foreach (var item in InputDataGrid.Items)
                {
                    var value = (int)((dynamic)item).Value;
                    writer.WriteLine(value);
                }
            }
        }
        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            if (matrix == null || matrix.Length == 0)
            {
                ResultTextBlock.Text = "Нет исходных данных.";
                return;
            }

            // Считываем данные из DataGrid в матрицу
            int rowCount = ((ObservableCollection<int[]>)InputDataGrid.ItemsSource).Count;
            for (int i = 0; i < rowCount; i++)
            {
                var rowData = (int[])((ObservableCollection<int[]>)InputDataGrid.ItemsSource)[i];
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    matrix[i, j] = rowData[j];
                }
            }

            int firstOddColumn = GetFirstOddColumnIndex(matrix);
            ResultTextBlock.Text = firstOddColumn > 0 ? $"Первый столбец с нечетными числами: {firstOddColumn}" : "0";
        }

        private int GetFirstOddColumnIndex(int[,] matrix)
        {
            for (int col = 0; col < matrix.GetLength(1); col++)
            {
                bool allOdd = true;
                for (int row = 0; row < matrix.GetLength(0); row++)
                {
                    if (matrix[row, col] % 2 == 0) // Если число четное
                    {
                        allOdd = false;
                        break;
                    }
                }
                if (allOdd) return col + 1; // возвращаем индекс с 1
            }
            return 0; // нет таких столбцов
        }
    }
}