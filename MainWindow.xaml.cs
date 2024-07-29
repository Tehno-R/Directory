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
    
    public class LogRecord
    {
        public LogRecord(OperationType operationType, string data, Result res, string message = "")
        {
            Operation = operationType.ToString();
            DataWith = data;
            ResultOf = $"{res}{message}";
        }

        public enum OperationType
        {
            Add,
            Remove
        }
        public enum Result
        {
            Successful,
            Unsuccessful
        }

        public string Operation { get; }
        public string DataWith { get; }
        public string ResultOf { get; }

        public override string ToString()
        {
            return $"{Operation} || {DataWith} || {ResultOf}";
        }
    }

    private readonly List<LogRecord> _debugList = [];
    private ObservableCollection<Record> ObservCollect { get; }
    public static RbTree Database { get; private set; } = null!; // не до конца уверен что так правильно делать
    private string Here { get; }
    
    public MainWindow()
    {
        InitializeComponent();
        ModalWindow modalWindow = new ModalWindow(this);
        modalWindow.ShowDialog();
        ObservCollect = new ObservableCollection<Record>();
        RecordsListView.ItemsSource = ObservCollect;
        Here = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
    }
    
    /// <summary>
    ///  Call only from modal window and create database instance
    /// </summary>
    /// <param name="hashTableSize">Integer value size of hashtable that would be use in database</param>
    public static void InitDatabase(int hashTableSize)
    {
        Database = new RbTree(hashTableSize);
    }
    
    /// <summary>
    /// Read input file and push in directory, otherwise show message about error
    /// </summary>
    /// <param name="selectedFilePath">String input file path</param>
    /// <returns>
    /// LogRecord.Result.Successful if <c>all</c> data from file successful added in directory, otherwise
    /// LogRecord.Result.Unsuccessful
    /// </returns>
    private LogRecord.Result ImportFromFile(string selectedFilePath)
    {
        List<Key> allKeys = [];

        var lines = File.ReadAllLines(selectedFilePath);

        // Извлечение данных из каждой строки
        foreach (var line in lines)
        {
            // Разделение строки на поля
            var fields = line.Split(' ');

            // Проверка кол-ва элементов разделённых пробелом
            if (fields.Length != 7)
            {
                MessageBox.Show("Неверный формат строки: " + line + "\nВозможнно где то стоит лишний " +
                                "пробел или его наоборот не хватает");
                return LogRecord.Result.Unsuccessful;
            }

            // Извлечение полей
            var serie = fields[0];
            var number = fields[1];
            var surname = fields[2];
            var name = fields[3];
            var patronymic = fields[4];
            var licensePlate = fields[5];
            var date = fields[6];

            // Проверка каждого поля на корректность
            if (!Checker.IsValidPassport($"{serie} {number}"))
            {
                MessageBox.Show("Неверный формат серии и номера: " + line);
                return LogRecord.Result.Unsuccessful;
            }
            if (!(Checker.IsValidName(surname) && Checker.IsValidName(name) && Checker.IsValidName(patronymic)))
            {
                MessageBox.Show("Неверный формат ФИО: " + line);
                return LogRecord.Result.Unsuccessful;
            }
            if (!Checker.IsValidLicensePlate(licensePlate))
            {
                MessageBox.Show("Неверный формат гос.номера: " + line);
                return LogRecord.Result.Unsuccessful;
            }
            if (!Checker.IsValidDate(date))
            {
                MessageBox.Show("Неверный формат даты: " + line);
                return LogRecord.Result.Unsuccessful;
            }
            
            Passport passport = new Passport($"{serie} {number}");
            Fio fio = new Fio(surname, name, patronymic);
            GosNum gosnum = new GosNum(licensePlate);
            MyDate myDate = new MyDate(date);

            allKeys.Add(new Key(passport, fio, gosnum, myDate));
        }
        
        // Добавление данных в справочник
        foreach (var key in allKeys)
        {
            Database.Insert(key);
            _debugList.Add(new LogRecord(LogRecord.OperationType.Add, key.ToString(),
                LogRecord.Result.Successful));
        }

        return LogRecord.Result.Successful;
    }
    
    /// <summary>
    /// Collect input from UI input fields and convert to Key
    /// </summary>
    /// <returns>Key with param from input fields or default key</returns>
    private Tuple<Key, string> ConvertToKey()
    {
        string pass = PassportTextBox.Text;
        string fioText = FioTextBox.Text;
        string[] fioSep = fioText.Split();
        string gosnumText = GosNumberTextBox.Text;
        string dateText = DatePicker.Text;
        
        if (!Checker.IsValidPassport(pass))
        {
            MessageBox.Show("Некорректные паспортные данные");
            return new Tuple<Key, string>(Key.DefaultKey, $"{pass} {fioText} {gosnumText} {dateText}");
        }
        
        if (!(fioSep.Length == 3 && 
              Checker.IsValidName(fioSep[0]) && Checker.IsValidName(fioSep[1]) && Checker.IsValidName(fioSep[2])))
        {
            MessageBox.Show("Некорректные ФИО");
            return new Tuple<Key, string>(Key.DefaultKey, $"{pass} {fioText} {gosnumText} {dateText}");
        }
        
        if (!Checker.IsValidLicensePlate(gosnumText))
        {
            MessageBox.Show("Некорректный гос.номер");
            return new Tuple<Key, string>(Key.DefaultKey, $"{pass} {fioText} {gosnumText} {dateText}");
        }
        
        if (!Checker.IsValidDate(dateText))
        {
            MessageBox.Show("Некорректная дата");
            return new Tuple<Key, string>(Key.DefaultKey, $"{pass} {fioText} {gosnumText} {dateText}");
        }
        
        Passport passport = new Passport(pass);
        Fio fio = new Fio(fioSep[0], fioSep[1],fioSep[2]);
        GosNum gosnum = new GosNum(gosnumText);
        MyDate date = new MyDate(dateText);
        
        return new Tuple<Key, string>(new Key(passport, fio, gosnum, date), "");
    }
    
    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        Tuple<Key,string> key = ConvertToKey();
        if (key.Item1 == Key.DefaultKey)
        {
            _debugList.Add(new LogRecord(LogRecord.OperationType.Add, key.Item2,
                LogRecord.Result.Unsuccessful));
            return;
        }
        bool added = Database.Insert(key.Item1);
        _debugList.Add(new LogRecord(LogRecord.OperationType.Add, key.ToString(),
            added ? LogRecord.Result.Successful:LogRecord.Result.Unsuccessful));
        if (added) RefreshList();
        SaveLogs();
    }
    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        Tuple<Key,string> key = ConvertToKey();
        if (key.Item1 == Key.DefaultKey)
        {
            _debugList.Add(new LogRecord(LogRecord.OperationType.Remove, key.Item2,
                LogRecord.Result.Unsuccessful));
            return;
        }
        bool deleted = Database.Delete(key.Item1);
        _debugList.Add(new LogRecord(LogRecord.OperationType.Remove, key.Item1.ToString(),
            deleted ? LogRecord.Result.Successful:LogRecord.Result.Unsuccessful));
        if (deleted) RefreshList();
        SaveLogs();
    }
    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        Tuple<Key,string> key = ConvertToKey();
        if (key.Item1 == Key.DefaultKey) return;
        string date1 = DateFrom.Text;
        string date2 = DateTo.Text;
        if (!(Checker.IsValidDate(date1) && Checker.IsValidDate(date2))) return;
        Key key1 = new Key(key.Item1.Passport, key.Item1.Fio, key.Item1.GosNum, new MyDate(date1));
        Key key2 = new Key(key.Item1.Passport, key.Item1.Fio, key.Item1.GosNum, new MyDate(date2));
        List<Key> results = Database.Find(key1, key2);
        List<Record> records = [];
        foreach (var res in results)
        {
            records.Add(new Record(res));
        }

        SearchRes window = new SearchRes(records);
        window.Show();
    }
    private void ImportButton_Click(object sender, RoutedEventArgs e)
    {
        // Создание экземпляра OpenFileDialog
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            // Настройка фильтра для файлов
            Filter = "Текстовые файлы (*.txt)|*.txt",
            InitialDirectory = Here
        };
        openFileDialog.ShowDialog();
        
        // Проверка, что пользователь выбрал файл
        if (openFileDialog.FileName == "") return;
        
        // Получение пути к выбранному файлу
        string selectedFilePath = openFileDialog.FileName;

        LogRecord.Result result = ImportFromFile(selectedFilePath);
        if (result == LogRecord.Result.Successful)
        {
            RefreshList();
            SaveLogs();            
        }
    }
    private void ExportButton_Click(object sender, RoutedEventArgs e)
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "Text files (*.txt)|*.txt";
        saveFileDialog.InitialDirectory = Here;
        saveFileDialog.Title = "Save your file";

        // Отображение диалогового окна и обработка результата
        saveFileDialog.ShowDialog();
        if (saveFileDialog.FileName == "") return;

        string filePath = saveFileDialog.FileName;
        List<Key> keys = Database.GetAllKeys();
        string dataToSave = "";
        foreach (var key in keys)
        {
            dataToSave += key.ToString() + '\n';
        }
        File.WriteAllText(filePath, dataToSave);
    }
    private void DebugButton_Click(object sender, RoutedEventArgs e) 
    {
        Debug window = new Debug(_debugList);
        window.Show();
    }
    private void AlternateViewButton_Click(object sender, RoutedEventArgs e)
    {
        AlterView window = new AlterView();
        window.Show();
    }
    private void RefreshList()
    {
        List<Key> keys = Database.GetAllKeys();

        ObservCollect.Clear();
        foreach (var key in keys)
        {
            ObservCollect.Add(new Record(key));
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
        string dataToSave = "";
        foreach (var log in _debugList)
        {
            dataToSave += log.ToString() + '\n';
        }
        File.WriteAllText(Here + "\\Logs.txt", dataToSave);
    }
}