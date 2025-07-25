using ASToolkit.Parsing.Core.Interfaces;
using ASToolkit.Parsing.Csv.Extensions;
using ASToolkit.Parsing.Excel.Extensions;
using ASToolkit.Parsing.Extensions;
using ASToolkit.Parsing.Json.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ASToolkit.ParsingTests.Extensions;

public class DependencyInjectionTest
{
    [Fact]
    public void ConfigureParsingFactory_ShouldRegisterAllParsers()
    {
        var services = new ServiceCollection();
        
        services.AddParsingFactory(builder =>
        {
            builder.UseCsvParser();
            builder.UseExcelParser();
            builder.UseJsonParser();
        });

        var provider = services.BuildServiceProvider();
        
        Assert.NotEmpty(provider.GetServices<IParser>());
    }
    [Fact]
    public void ConfigureSerializerFactory_ShouldRegisterAllSerializers()
    {
        var services = new ServiceCollection();
        
        services.AddSerializerFactory(builder =>
        {
            builder.UseCsvSerializer();
            builder.UseExcelSerializer();
            builder.UseJsonSerializer();
        });

        var provider = services.BuildServiceProvider();
        Assert.NotEmpty(provider.GetServices<ISerializer>());
    }
    [Fact]
    public void ConfigureParsingToolkit_ShouldRegisterAllParsersAndSerializers()
    {
        var services = new ServiceCollection();
        
        services.ConfigureParsingToolkit();

        var provider = services.BuildServiceProvider();
        
        Assert.NotEmpty(provider.GetServices<IParser>());
        Assert.NotEmpty(provider.GetServices<ISerializer>());
    }
}