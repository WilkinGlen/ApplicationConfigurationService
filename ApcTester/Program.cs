// See https://aka.ms/new-console-template for more information

using ApplicationConfigurationService;
using Microsoft.Extensions.Configuration;

var configService = new ConfigurationService();
var config = configService.Configuration;

var resp1 = config["Individual"];
var _ = config.GetSection("Logging").GetSection("LogLevel")["Default"];
var _ = config.GetConnectionString("ConnString1");

Console.WriteLine(resp1);

