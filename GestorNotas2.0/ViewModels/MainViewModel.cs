using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestorNotas2._0.Models;
using GestorNotas2._0.Services;
using GestorNotas2._0.Views;
using System.Collections.ObjectModel;
using System.Linq;

namespace GestorNotas2._0.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private ObservableCollection<Notas> _allNotas;

        [ObservableProperty]
        private ObservableCollection<Notas> notasCollection = new ObservableCollection<Notas>();

        [ObservableProperty]
        private string searchText = string.Empty;

        partial void OnSearchTextChanged(string value)
        {
            FilterNotas(value);
        }

        private NotasServices _service;

        public MainViewModel()
        {
            _service = new NotasServices();
            _allNotas = new ObservableCollection<Notas>();
            GetAll();
        }

        private void Alerta(string Titulo, string Mensaje)
        {
            MainThread.BeginInvokeOnMainThread(async () => await App.Current!.MainPage!.DisplayAlert(Titulo, Mensaje, "Aceptar"));
        }

        public void GetAll()
        {
            var getAll = _service.GetAll();

            _allNotas.Clear();
            notasCollection.Clear();

            if (getAll.Count > 0)
            {
                foreach (var nota in getAll)
                {
                    _allNotas.Add(nota);
                    notasCollection.Add(nota);
                }
            }
        }

        private void FilterNotas(string query)
        {
            notasCollection.Clear();

            if (string.IsNullOrWhiteSpace(query))
            {
                foreach (var nota in _allNotas)
                {
                    notasCollection.Add(nota);
                }
            }
            else
            {
                string lowerCaseQuery = query.ToLowerInvariant();

                var filteredNotas = _allNotas.Where(nota =>
                    (nota.Titulo?.ToLowerInvariant().Contains(lowerCaseQuery) ?? false) ||
                    (nota.Contenido?.ToLowerInvariant().Contains(lowerCaseQuery) ?? false)
                );

                foreach (var nota in filteredNotas)
                {
                    notasCollection.Add(nota);
                }
            }
        }

        [RelayCommand]
        private async Task GoToAddEditView()
        {
            await App.Current!.MainPage!.Navigation.PushAsync(new AddEditView());
        }

        [RelayCommand]
        private async Task SelectNotas(Notas notas)
        {
            try
            {
                const string ACTUALIZAR = "Actualizar";
                const string ELIMINAR = "Eliminar";

                string res = await App.Current!.MainPage!.DisplayActionSheet("OPCIONES", "Cancelar", null, ACTUALIZAR, ELIMINAR);

                if (res == ACTUALIZAR)
                {
                    await App.Current!.MainPage!.Navigation.PushAsync(new AddEditView(notas));
                    GetAll();
                }
                else if (res == ELIMINAR)
                {
                    bool respuesta = await App.Current!.MainPage!.DisplayAlert("ELIMINAR NOTA", "¿Desea eliminar la nota?", "Si", "No");

                    if (respuesta)
                    {
                        int del = _service.Delete(notas);

                        if (del > 0)
                        {
                            Alerta("ELIMINAR NOTA", "Nota eliminada correctamente");
                            GetAll();
                        }
                        else
                        {
                            Alerta("ELIMINAR NOTA", "No se eliminó la nota");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Alerta("ERROR", ex.Message);
            }
        }
    }
}