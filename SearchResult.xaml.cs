using System.Collections.ObjectModel;

namespace wpfApp;

public partial class SearchRes
{
    public SearchRes(List<MainWindow.Record> records)
    {
        InitializeComponent();
        SearchResList.ItemsSource = new ObservableCollection<MainWindow.Record>(records);
    }
}