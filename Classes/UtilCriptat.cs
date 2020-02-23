using SQLite.Net.Cipher.Interfaces;
using SQLite;
using SQLite.Net.Cipher.Model;

namespace PrevenireRiscIT.Classes
{
    public class UtilCriptat:IModel
    {
        [PrimaryKey, Column("ID")]
        public string Id { get; set; }
        [MaxLength(100), Secure]
        public string NumeClient { get; set; }
        [MaxLength(50), Secure]
        public string Parola { get; set; }
        [Unique, Secure]
        public string AdresaEmail { get; set; }
    }
}