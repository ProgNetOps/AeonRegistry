
using System.Data.Common;

namespace AeonRegistry.Data;

public static class DataUtility
{
    public static string GetConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(nameof(DbConnection));
        return connectionString!;
    }
}
