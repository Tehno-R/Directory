using System.Collections.ObjectModel;

namespace wpfApp;

public partial class Debug
{
    private ObservableCollection<MainWindow.LogRecord> Records { get; set; }

    public Debug(List<MainWindow.LogRecord> list)
    {
        InitializeComponent();
        Records = new ObservableCollection<MainWindow.LogRecord>();
        DebugListView.ItemsSource = Records;
        foreach (var rec in list)
        {
            Records.Add(rec);
        }
    }
    
    
}