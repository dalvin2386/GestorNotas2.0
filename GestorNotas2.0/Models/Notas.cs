

using SQLite;

namespace GestorNotas2._0.Models
{
    public class Notas
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public string? Titulo { get; set; }
        public string? Contenido { get; set; }
    }
}
