using System.Collections.ObjectModel;
using Examen3_PM2.Models;
using Examen3_PM2.Controller;
using System.Windows.Input;
using Plugin.Maui.Audio;

namespace Examen3_PM2.ViewModels
{
    public class ViewNotesModel : BaseView
    {
        private bool _orderByDescending = true;
        private ObservableCollection<Models.Note> notes;
        private FirebaseControllers FirebaseControl = new FirebaseControllers();

        public ObservableCollection<Models.Note> memo
        {
            get { return notes; }
            set { notes = value; OnPropertyChanged(); }
        }

        private Models.Note _selectedmemory;

        public Models.Note SelectedMemory
        {
            get { return _selectedmemory; }
            set { _selectedmemory = value; OnPropertyChanged(); }
        }

        public bool OrderByDescending
        {
            get { return _orderByDescending; }
            set { _orderByDescending = value; OnPropertyChanged(); }
        }

        public ICommand GoToDetailsCommand { private set; get; }

        public ICommand DeleteCommand { private set; get; }

        public ICommand NewNoteCommand { private set; get; }

        public INavigation Navigation { get; set; }
        public ICommand SalirCommand { private set; get; }
        public ICommand HearCommand { private set; get; }

        public ICommand ChangeOrderByCommand => new Command(() =>
        {
            OrderByDescending = !OrderByDescending;
            loadNotes();
        });

        public ViewNotesModel(INavigation navigation)
        {
            Navigation = navigation;
            GoToDetailsCommand = new Command<Type>(async (pageType) => await GoToDetails(pageType, SelectedMemory));
            NewNoteCommand = new Command<Type>(async (pageType) => await NewNote(pageType));
            DeleteCommand = new Command(async () => await DeleteNote(SelectedMemory.Key));
            HearCommand = new Command(async () => await Escuchar(SelectedMemory.Key));

            loadNotes();
        }

        public async Task loadNotes()
        {
            List<Note> listNote;

            memo = new ObservableCollection<Note>();

            try
            {
                listNote = await FirebaseControl.GetListNote();

                if (OrderByDescending)
                {
                    listNote = listNote.OrderByDescending(p => p.Fecha).ToList();
                }
                else
                {
                    listNote = listNote.OrderBy(p => p.Fecha).ToList();
                }

                foreach (var product in listNote)
                {
                    Note productos = new Note
                    {
                        Key = product.Key,
                        Id_nota = product.Id_nota,
                        Descripcion = product.Descripcion,
                        Fecha = product.Fecha,
                        Foto = product.Foto,
                        Audio = product.Audio,
                    };

                    memo.Add(productos);
                }

            }
            catch (Exception ex)
            {
                //await Application.Current.MainPage.DisplayAlert("Atención", "Se produjo un error al obtener las Notas", "OK");
            }
        }


        public async Task GoToDetails(Type pageType, Note selectedMemory)
        {
            if (selectedMemory != null)
            {

                var page = (Page)Activator.CreateInstance(pageType);

                var viewModel = new MemoryNote();
                viewModel.SelectedMemory = selectedMemory;
                viewModel.Fecha = selectedMemory.Fecha;
                viewModel.Descripcion = selectedMemory.Descripcion;
                viewModel.Foto = selectedMemory.Foto;

                // Verificar si el audio no es nulo antes de pasarlo
                if (!string.IsNullOrEmpty(selectedMemory.Audio))
                {
                    viewModel.Audio = selectedMemory.Audio;
                    // Reproducir el audio
                    await Escuchar(selectedMemory.Key);
                }

                viewModel.Key = selectedMemory.Key;
                page.BindingContext = viewModel;

                await Navigation.PushAsync(page);
            }
        }

        public async Task NewNote(Type pageType)
        {
            var page = (Page)Activator.CreateInstance(pageType);

            var viewModel = new MemoryNote();
            viewModel.SelectedMemory = null;
            page.BindingContext = viewModel;
            await Navigation.PushAsync(page);
        }

        public async Task DeleteNote(string key)
        {
            if (SelectedMemory != null)
            {
                var tappedItem = memo.FirstOrDefault(item => item.Key == key);
                bool userConfirmed = await Application.Current.MainPage.DisplayAlert("Confirmación", "¿Está seguro de que desea eliminar esta Nota?", "Si", "No");
                if (userConfirmed)
                {
                    try
                    {

                        if (FirebaseControl != null)
                        {
                            bool success = await FirebaseControl.deleteNote(key);

                            if (success)
                            {
                                memo.Remove(tappedItem);
                                SelectedMemory = null;

                                await Application.Current.MainPage.DisplayAlert("Atención", "Nota Eliminada", "OK");
                            }
                        }

                    }
                    catch
                    {
                        await Application.Current.MainPage.DisplayAlert("Atención", "No se pudo eliminar la Nota", "OK");
                    }
                }

            }

        }
        public async Task Escuchar(string key)
        {
            if (SelectedMemory != null)
            {
                var tappedItem = memo.FirstOrDefault(item => item.Key == key);

                try
                {
                    // Obtener el audio en formato base64 del elemento seleccionado
                    string audioBase64 = tappedItem.Audio;

                    // Convertir el audio de base64 a bytes
                    byte[] audioBytes = Convert.FromBase64String(audioBase64);

                    // Crear un archivo temporal para el audio
                    string tempFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "temp_audio.mp3");

                    // Guardar los bytes del audio en el archivo temporal
                    File.WriteAllBytes(tempFilePath, audioBytes);

#if ANDROID
                    // Reproducir el audio solo en Android
                    Android.Media.MediaPlayer mediaPlayer = new Android.Media.MediaPlayer();
                    mediaPlayer.SetDataSource(tempFilePath);
                    mediaPlayer.Prepare();
                    mediaPlayer.Start();
#else
            // Agregar código para otras plataformas o mostrar un mensaje de que esta característica no está disponible
                    await Application.Current.MainPage.DisplayAlert("No disponible", "Reproducción de audio solo está disponible en Android.", "OK");
#endif
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo reproducir el audio.", "OK");
                }
            }
        }


    }
}
