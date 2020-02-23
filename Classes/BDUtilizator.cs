using SQLite.Net.Cipher.Data;
using SQLite.Net.Interop;

namespace PrevenireRiscIT.Classes
{
    public class BDUtilizator : SecureDatabase
    {
        public BDUtilizator(ISQLitePlatform _platforma, string _locatieBD, string _encript) : base(_platforma, _locatieBD, _encript)
        {
        }

        protected override void CreateTables()
        {
            CreateTable<UtilCriptat>();
        }
    }
}