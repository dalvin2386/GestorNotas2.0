using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GestorNotas2._0.Models;
using GestorNotas2._0.Services;
using GestorNotas2._0.Views;
using GestorNotas2._0.Messages;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel.Communication;

namespace GestorNotas2._0.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private ObservableCollection<Notas> _allNotas = new ObservableCollection<Notas>();

        [ObservableProperty]
        private ObservableCollection<Notas> notasCollection = new ObservableCollection<Notas>();

        [ObservableProperty]
        private string searchText = string.Empty;

        partial void OnSearchTextChanged(string value)
        {
            FilterNotas(value);
        }

        private readonly NotasServices _service;

        public MainViewModel()
        {
            _service = new NotasServices();
            LoadAllNotas();

            WeakReferenceMessenger.Default.Register<NoteSavedMessage>(this, (recipient, message) =>
            {
                if (message.Value)
                {
                    LoadAllNotas();
                }
            });
        }

        private void Alerta(string titulo, string mensaje)
        {
            MainThread.BeginInvokeOnMainThread(async () => await App.Current!.MainPage!.DisplayAlert(titulo, mensaje, "Aceptar"));
        }

        public void LoadAllNotas()
        {
            var notesFromDb = _service.GetAll();

            _allNotas.Clear();
            foreach (var nota in notesFromDb)
            {
                _allNotas.Add(nota);
            }

            FilterNotas(SearchText);
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

                var filtered = _allNotas.Where(nota =>
                    (nota.Titulo?.ToLowerInvariant().Contains(lowerCaseQuery) ?? false) ||
                    (nota.Contenido?.ToLowerInvariant().Contains(lowerCaseQuery) ?? false)
                );

                foreach (var nota in filtered)
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
            if (notas == null) return;

            try
            {
                const string ACTUALIZAR = "Actualizar";
                const string ELIMINAR = "Eliminar";
                const string COMPARTIR = "Compartir";

                string res = await App.Current!.MainPage!.DisplayActionSheet("OPCIONES", "Cancelar", null, ACTUALIZAR, ELIMINAR, COMPARTIR);

                if (res == ACTUALIZAR)
                {
                    await App.Current!.MainPage!.Navigation.PushAsync(new AddEditView(notas));
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
                            LoadAllNotas();
                        }
                        else
                        {
                            Alerta("ELIMINAR NOTA", "No se eliminó la nota");
                        }
                    }
                }
                else if (res == COMPARTIR)
                {
                    await ShareNote(notas);
                }
            }
            catch (Exception ex)
            {
                Alerta("ERROR", $"Ocurrió un error: {ex.Message}");
            }
        }

        private async Task ShareNote(Notas notaToShare)
        {
            if (notaToShare == null) return;

            try
            {
                string shareText = $"*Título:* {notaToShare.Titulo}\n\n*Contenido:*\n{notaToShare.Contenido}";

                await Share.RequestAsync(new ShareTextRequest
                {
                    Text = shareText,
                    Title = "Compartir Nota",
                    Subject = $"Nota de GestorNotas: {notaToShare.Titulo}"
                });
            }
            catch (Exception ex)
            {
                Alerta("Error al Compartir", $"No se pudo compartir la nota: {ex.Message}");
            }
        }
    }
}