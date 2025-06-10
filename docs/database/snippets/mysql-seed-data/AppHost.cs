var builder = DistributedApplication.CreateBuilder(args);

var catalogDbName = "catalog";

var mysql = builder.AddMySql("mysql")
                   .WithEnvironment("MYSQL_DATABASE", catalogDbName)
                   .WithDataVolume()
                   .WithLifetime(ContainerLifetime.Persistent)
                   .WithPhpMyAdmin();

var creationScript = $$"""
USE catalog;
CREATE TABLE IF NOT EXISTS `catalog`
(
    `id` int(11) NOT NULL AUTO_INCREMENT,
    `name` varchar(255) NOT NULL,
    `description` varchar(255) NOT NULL,
    `price` DECIMAL(18,2) NOT NULL,
    PRIMARY KEY (`id`)
);

-- Insert some sample data into the Catalog table only if the table is empty
INSERT INTO catalog (name, description, price)
SELECT *
FROM (
        SELECT '.NET Bot Black Hoodie', 'This hoodie will keep you warm while looking cool and representing .NET!', 19.5 UNION ALL
        SELECT '.NET Black & White Mug', 'The perfect place to keep your favorite beverage while you code.', 8.5 UNION ALL
        SELECT 'Prism White T-Shirt', "It's a t-shirt, it's white, and it can be yours.", 12
    ) data
-- This clause ensures the rows are only inserted if the table is empty
WHERE NOT EXISTS (SELECT NULL FROM catalog)
""";

var mysqldb = mysql.AddDatabase(catalogDbName)
                   .WithCreationScript(creationScript);
                   
builder.Build().Run();
