using System.Windows;
using WfmApp.Presentation.ViewModels;

namespace WfmApp.Presentation.Views;

public partial class MainWindow : Window
{
    public MainWindow(ShiftDashboardViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
