using SQLite;
using SQLite.Net.Cipher.Interfaces;
using SQLite.Net.Cipher.Model;

namespace PrevenireRiscIT.Classes
{
    public class Utilizator
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [MaxLength(100)]
        public string NumeClient { get; set; }
        [MaxLength(50)]
        public string Parola { get; set; }
        [Unique]
        public string AdresaEmail { get; set; }
    }
}