namespace GraduationProjectModels
{
    public static class AppSettings
    {
        private static string _connectionString;
        public static string DbConnectionString
        {
            get => _connectionString?.Replace("Connection Time", "");
            set => _connectionString = value;
        }
    }
}
