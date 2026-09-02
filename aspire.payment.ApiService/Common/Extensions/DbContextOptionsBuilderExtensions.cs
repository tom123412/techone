using Microsoft.EntityFrameworkCore;

public static class DbContextOptionsBuilderExtensions
{
    extension (DbContextOptionsBuilder options)
    {
        public DbContextOptionsBuilder UseCosmosConnectionStringOrManagedIdentity(string connectionString, string databaseName)
        {
            if (connectionString.Contains("AccountKey="))
            {
                return options.UseCosmos(connectionString, databaseName: databaseName);
            }
            else
            {
                var endpoint = GetConnectionStringProperty(connectionString, "AccountEndpoint");
                return options.UseCosmos(endpoint, new Azure.Identity.DefaultAzureCredential(), databaseName: databaseName);
            }
        }

        
        private static string GetConnectionStringProperty(string connectionString, string propertyName)
        {
            var match = System.Text.RegularExpressions.Regex.Match(connectionString, $"{propertyName}=(.*?)(;|$)");
            return match.Success ? match.Groups[1].Value : throw new ArgumentException($"Property {propertyName} not found in {connectionString}.");
        }
    }
}