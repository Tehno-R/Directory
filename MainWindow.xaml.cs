using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using MyStructures;
using MyStructures.RBTree;

namespace wpfApp;

public partial class MainWindow
{
    public class Record
    {
        public Record(string passport, string fio, string licensePlate, string date)
        {
            Passport = passport;
            Fio = fio;
            LicensePlate = licensePlate;
            string[] date_ = date.Split('.');
            Date = date;
        }
        public Record(Key key)
        {
            Passport = key.Passport.ToString();
            Fio = key.Fio.ToString();
            LicensePlate = key.GosNum.ToString();
            Date = key.Date.ToString();
        }

        public string Passport { get; set; }
        public string Fio { get; set; }
        public string LicensePlate { get; set; }
        public string Date { get; set; }
    }
    public class LogRecord(LogRecord.Operation oper, string data, LogRecord.Result res, string message = "")
    {
        public enum Operation
        {
            Add,
            Remove
        }
        public enum Result
        {
            Successful,
            Unsuccessful
        }

        public string operation { get; } = oper.ToString();
        public string DataWith { get; } = data;
        public string ResultOf { get; } = $"{res}{message ?? ""}";

        public override string ToString()
        {
            return $"{operation} || {DataWith} || {ResultOf}";
        }
    }

    private List<LogRecord> debugList = [];

    private ObservableCollection<Record> Records { get; }
    
    public static RbTree Database { get; set; }
    
    public MainWindow()
    {
        InitializeComponent();
        ModalWindow modalWindow = new ModalWindow(this);
        modalWindow.ShowDialog();
        Records = new ObservableCollection<Record>();
        RecordsListView.ItemsSource = Records;
    }
    public void InitDatabase(int hashTableSize)
    {
        Database = new RbTree(hashTableSize);
    }
    private void ImportButton_Click(object sender, RoutedEventArgs e)
    {
        // Создание экземпляра OpenFileDialog
        string here = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            // Настройка фильтра для файлов (например, только текстовые файлы)
            Filter = "Текстовые файлы (*.txt)|*.txt",
            InitialDirectory = here
        };

        // Проверка, что пользователь выбрал файл
        if (openFileDialog.ShowDialog() != true)
        {
            MessageBox.Show("Ошибка открытия диалогового окна");
            return;
        }
        // Получение пути к выбранному файлу
        string selectedFilePath = openFileDialog.FileName;

        ImportFromFile(selectedFilePath);
        RefreshList();
        SaveLogs();
    }
    private void ImportFromFile(string selectedFilePath)
    {
        List<Key> allKeys = new List<Key>();
        try
        {
            var lines = File.ReadAllLines(selectedFilePath);

            foreach (var line in lines)
            {
                // Разделение строки на поля
                var fields = line.Split(' ');

                // Проверка корректности числа полей
                if (fields.Length != 7)
                {
                    MessageBox.Show("Неверный формат строки: " + line);
                    return;
                }

                // Извлечение и преобразование полей
                var serie = fields[0];
                var number = fields[1];
                var surname = fields[2];
                var name = fields[3];
                var patronymic = fields[4];
                var licensePlate = fields[5];
                var date = fields[6];
                if (!(Checker.IsValidPassport($"{serie} {number}") &&
                      Checker.IsValidName(surname) && Checker.IsValidName(name) && Checker.IsValidName(patronymic) &&
                      Checker.IsValidLicensePlate(licensePlate) &&
                      Checker.IsValidDate(date)))
                {
                    MessageBox.Show("Неверный формат данных: " + line);
                    return;
                }
                
                Passport passport_ = new Passport($"{serie} {number}");
                Fio fio_ = new Fio(surname, name,patronymic);
                GosNum gosnum_ = new GosNum(licensePlate);
                MyDate date_ = new MyDate(date);

                Key key = new Key(passport_, fio_, gosnum_, date_);
                
                allKeys.Add(key);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ошибка при чтении файла");
            return;
        }

        foreach (var key in allKeys)
        {
            bool added = Database.Insert(key);
            debugList.Add(new LogRecord(LogRecord.Operation.Add, key.ToString(),
                added ? LogRecord.Result.Successful:LogRecord.Result.Unsuccessful));
        }

    }
    /// <summary>
    /// Convert input from inputLines and convert to Key
    /// </summary>
    /// <returns>Key with param from inputLine or default key</returns>
    private Key? ConvertToKey()
    {
        string pass = PassportTextBox.Text;
        if (!Checker.IsValidPassport(pass))
        {
            MessageBox.Show("Некорректные паспортные данные. Формат: 0000 000000");
            return null;
        }
        
        string fio = FioTextBox.Text;
        string[] fioSep = fio.Split();
        if (!(fioSep.Length == 3 && 
              Checker.IsValidName(fioSep[0]) && Checker.IsValidName(fioSep[1]) && Checker.IsValidName(fioSep[2])))
        {
            MessageBox.Show("Некорректное ФИО. Поле должно содержать 3 слова состоящие только буквы.");
            return null;
        }
        
        string gosnum = GosNumberTextBox.Text;
        if (!Checker.IsValidLicensePlate(gosnum))
        {
            MessageBox.Show("Некорректный гос.номер. Поле должно соответствовать формату A123BC");
            return null;
        }
        
        string date = DatePicker.Text;
        if (!Checker.IsValidDate(date))
        {
            MessageBox.Show("Некорректная дата. " +
                            "Поле должно содержать 3 числа разделённых через точку состоящие только цифр.");
            return null;
        }
        
        Passport passport_ = new Passport(pass);
        Fio fio_ = new Fio(fioSep[0], fioSep[1],fioSep[2]);
        GosNum gosnum_ = new GosNum(gosnum);
        MyDate date_ = new MyDate(date);
        
        Key key = new Key(passport_, fio_, gosnum_, date_);
        return key;
    }
    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        Key? key = ConvertToKey();
        if (key == null) return;
        bool added = Database.Insert(key);
        debugList.Add(new LogRecord(LogRecord.Operation.Add, key.ToString(),
            added ? LogRecord.Result.Successful:LogRecord.Result.Unsuccessful));
        RefreshList();
        SaveLogs();
    }
    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        Key? key = ConvertToKey();
        if (key == null) return;
        bool deleted = false;
        deleted = Database.Delete(key);
        debugList.Add(new LogRecord(LogRecord.Operation.Remove, key.ToString(),
            deleted ? LogRecord.Result.Successful:LogRecord.Result.Unsuccessful));
        RefreshList();
        SaveLogs();
    }
    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        Key? key = ConvertToKey();
        if (key == null) return;
        string date1 = DateFrom.Text;
        string date2 = DateTo.Text;
        if (!(Checker.IsValidDate(date1) && Checker.IsValidDate(date2))) return;
        Key key1 = new Key(key.Passport, key.Fio, key.GosNum, new MyDate(date1));
        Key key2 = new Key(key.Passport, key.Fio, key.GosNum, new MyDate(date2));
        List<Key> results = Database.Find(key1, key2);
        List<Record> records = new List<Record>();
        foreach (var res in results)
        {
            if (res != null) records.Add(new Record(res));
        }

        SearchRes window = new SearchRes(records);
        window.Show();
    }
    private void ExportButton_Click(object sender, RoutedEventArgs e)
    {
        // Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string here = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "Text files (*.txt)|*.txt";
        saveFileDialog.InitialDirectory = here;
        saveFileDialog.Title = "Save your file";

        // Отображение диалогового окна и обработка результата
        if (saveFileDialog.ShowDialog() == true)
        {
            string filePath = saveFileDialog.FileName;
            // Пример данных для записи в файл
            List<Key> keys = Database.GetAllKeys();
            string dataToSave = "";
            foreach (var key in keys)
            {
                dataToSave += key.ToString() + '\n';
            }
            File.WriteAllText(filePath, dataToSave);
        }
        
    }
    private void DebugButton_Click(object sender, RoutedEventArgs e)
    {
        Debug window = new Debug(debugList);
        // window.Show();
    }
    private void AlternateViewButton_Click(object sender, RoutedEventArgs e)
    {
        var window = new AlterView();
        if (window != null) window.Show();
    }
    private void RefreshList()
    {
        List<Key> keys = Database.GetAllKeys();

        Records.Clear();
        foreach (var key in keys)
        {
            Records.Add(new Record(key.Passport.ToString(), key.Fio.ToString(), key.GosNum.ToString(), 
                key.Date.ToString()));
        }
    }
    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
    {
        Record data = (Record)RecordsListView.SelectedItem;
        if (data == null) return;
        
        PassportTextBox.Text = data.Passport;
        FioTextBox.Text = data.Fio;
        GosNumberTextBox.Text = data.LicensePlate;
        DatePicker.Text = data.Date;
    }
    void SaveLogs()
    {
        string here = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Logs.txt";
        string dataToSave = "";
        foreach (var log in debugList)
        {
            dataToSave += log.ToString() + '\n';
        }
        File.WriteAllText(here, dataToSave);
    }
}