using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examen3_PM2.Models
{
    public class Note
    {
        public string? Key { get; set; }
        public int? Id_nota { get; set; }
        public DateTime Fecha { get; set; }
        public string? Descripcion { get; set; }
        public string? Foto { get; set; }
        public string? Audio { get; set; }
    }
}
