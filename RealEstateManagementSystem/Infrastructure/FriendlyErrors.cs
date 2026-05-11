using Microsoft.Data.SqlClient;

namespace RealEstateManagementSystem.Infrastructure;

/// <summary>
/// Maps technical failures to readable messages for end users without exposing internals.
/// </summary>
public static class FriendlyErrors
{
    public const string DatabaseUnavailable =
        "We couldn't connect to the database right now. Check your internet connection or try again shortly.";

    public const string SaveFailedGeneric =
        "We couldn't save your information. Please check your entries and try again.";

    public static string Describe(Exception ex)
    {
        if (ex is SqlException sql)
            return DescribeSql(sql);

        return SaveFailedGeneric;
    }

    public static string DescribeSql(SqlException sql)
    {
        return sql.Number switch
        {
            2 or -1 or -2 or 53 or 258 or 10060 or 4060 => DatabaseUnavailable,
            547 => "That change conflicts with existing data. Remove linked records or pick different values.",
            2627 or 2601 => "This record already exists (duplicate). Use a different identifier.",
            18456 => "The database account could not sign in. If this persists, contact your administrator.",
            40613 => SaveFailedGeneric,
            _ => SaveFailedGeneric
        };
    }
}
