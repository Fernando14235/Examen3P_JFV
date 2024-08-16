using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Examen3_PM2.Models;
using Firebase.Database;
using Firebase.Database.Query;


namespace Examen3_PM2.Controller
{
    public class FirebaseControllers
    {
        private FirebaseClient client = new FirebaseClient("https://e3pm2-3c5cf-default-rtdb.firebaseio.com/");

        public FirebaseControllers()
        {

        }

        public async Task<bool> CreateNote(Note nota)
        {
            if (string.IsNullOrEmpty(nota.Key))
            {
                try
                {
                    Console.WriteLine("Intentando crear una nueva nota...");
                    var notas = await client.Child("Nota").OnceAsync<Note>();
                    Console.WriteLine($"Número de notas existentes: {notas.Count}");
                    if (notas.Count == 0 || notas != null)
                    {
                        Console.WriteLine("No hay Notas existentes. Creando una nueva Nota...");
                        await client.Child("Nota").PostAsync(new Note
                        {
                            Id_nota = nota.Id_nota,
                            Descripcion = nota.Descripcion,
                            Fecha = nota.Fecha,
                            Foto = nota.Foto,
                            Audio = nota.Audio,
                        });

                        Console.WriteLine("Nota creada con éxito.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Ya existen Notas en la base de datos.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al intentar crear una nueva Nota: {ex.Message}");
                    return false;
                }
            }
            else
            {
                try
                {
                    Console.WriteLine("Intentando actualizar una Nota existente...");
                    await client.Child("Nota").Child(nota.Key).PutAsync(nota);
                    Console.WriteLine("Nota actualizada con éxito.");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al intentar actualizar la Nota: {ex.Message}");
                    return false;
                }
            }

            return false;
        }

        // Método para actualizar una nota
        public async Task<bool> UpdateNote(string key, Models.Note note)
        {
            try
            {
                await client
                    .Child("Notes")
                    .Child(key)
                    .PutAsync(note);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Note>> GetListNote()
        {
            var productos = await client.Child("Nota").OnceSingleAsync<Dictionary<string, Note>>();

            return productos.Select(x => new Note
            {
                Key = x.Key,
                Id_nota = x.Value.Id_nota,
                Descripcion = x.Value.Descripcion,
                Fecha = x.Value.Fecha,
                Foto = x.Value.Foto,
                Audio = x.Value.Audio,
            }).ToList();
        }

        public async Task<bool> deleteNote(string key)
        {
            try
            {
                await client.Child("Nota").Child(key).DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
