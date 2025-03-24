namespace AspireApp.Api;

internal static class Products
{
    public static readonly Product[] DefaultProducts =
    [
        new (1, "Laptop", 999.99m),
        new (2, "Keyboard", 49.99m),
        new (3, "Mouse", 24.99m),
    ];
}
