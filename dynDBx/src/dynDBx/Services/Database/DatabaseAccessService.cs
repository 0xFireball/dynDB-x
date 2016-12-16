using LiteDB;

namespace dynDBx.Services.Database
{
    public static class DatabaseAccessService
    {
        public static string GetDataStoreKey(int storeId) => $"ds{storeId}";

        private static LiteDatabase _dbInstance;

        public static LiteDatabase OpenOrCreateDefault()
        {
            if (_dbInstance == null)
            {
                //_dbInstance = new LiteDatabase($"kqanalytics.lidb;Password={KQRegistry.ServerConfiguration.DatabaseEncryptionPassword}");
                _dbInstance = new LiteDatabase($"kqanalytics.lidb");
            }
            return _dbInstance;
        }
    }
}