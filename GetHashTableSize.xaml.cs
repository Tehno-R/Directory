using System.ComponentModel;
using System.Windows;

namespace wpfApp;

public partial class ModalWindow
{
    private MainWindow MainWindow;
    private bool flag = false;
    public ModalWindow(MainWindow window)
    {
        InitializeComponent();
        MainWindow = window;
    }
    void Submit_Click(object sender, RoutedEventArgs routedEventArgs)
    {
        int size;
        try
        {
            size = int.Parse(HtSize.Text);
        }
        catch (Exception e)
        {
            MessageBox.Show("Введите число от 10 до 100");
            return;
        }

        if (size is >= 10 and <= 100)
        {
            flag = true;
            MainWindow.InitDatabase(size);
            Close();
        }
        else {MessageBox.Show("Введите число от 10 до 100");}
    }

    private void ModalWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        if (!flag) MainWindow.Close();
    }
}