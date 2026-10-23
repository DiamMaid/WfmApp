using System.Windows;
using WfmApp.Presentation.ViewModels;

namespace WfmApp.Presentation;

public partial class MainWindow : Window
{
    public MainWindow(ShiftPlanningViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}