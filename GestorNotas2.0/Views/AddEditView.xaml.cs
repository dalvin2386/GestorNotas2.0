using GestorNotas2._0.Models;
using GestorNotas2._0.ViewModels;

namespace GestorNotas2._0.Views;

public partial class AddEditView : ContentPage
{
    private AddEditViewModel viewModel;
    public AddEditView()
    {
        InitializeComponent();
        viewModel = new AddEditViewModel();
        this.BindingContext = viewModel;
    }

    public AddEditView(Notas notas)
    {
        InitializeComponent();
        viewModel = new AddEditViewModel(notas);
        this.BindingContext = viewModel;
    }
}