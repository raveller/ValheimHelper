using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace ValheimHelper.Tests;

public class FwlRewriteTests
{
    private readonly ITestOutputHelper _output;
    private readonly FwlFileHelper _fwlFileHelper;

    public FwlRewriteTests(ITestOutputHelper output)
    {
        _output = output;
        
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

        var factory = serviceProvider.GetService<ILoggerFactory>();

        var logger = factory.CreateLogger<FwlFileHelper>();
            
        _fwlFileHelper = new FwlFileHelper(logger);
    }
    

    [Fact]
    public void ReadWorldName()
    {

        var sourcePath = @"C:\Users\ravel\Downloads\Vierhalla";

        var fwlFileName = "Vierhalla20240521a.fwl - Not internally renamed";

        var fullPath = Path.Combine(sourcePath, fwlFileName);

        var data = _fwlFileHelper.ReadFile(fullPath);

        Assert.Equal("world", _fwlFileHelper.ReadWorldName(data));
    }


    [Fact]
    public void NewFileWithNewWorldName()
    {
        var sourcePath = @"C:\Users\ravel\Downloads\Vierhalla";

        var fwlFileName = "Vierhalla20240521a.fwl - Not internally renamed";

        var fullPath = Path.Combine(sourcePath, fwlFileName);

        var data = _fwlFileHelper.ReadFile(fullPath);

        var newName = "Vierhalla20240521f";

        var newData = _fwlFileHelper.UpdateWorldName(data, newName);

        var newFileName = Path.Combine(sourcePath, newName + ".fwl");

        _fwlFileHelper.WriteFile(newFileName, newData);

        var readNewData = _fwlFileHelper.ReadFile(newFileName);

        Assert.Equal(newName, _fwlFileHelper.ReadWorldName(readNewData));
    }

    [Fact]
    public void NewWorldWithShortMethod()
    {
        var sourcePath = @"C:\Users\ravel\Downloads\Vierhalla";

        var fwlFileName = "Vierhalla20240521a.fwl - Not internally renamed";
        var newName = "Vierhalla20240521g";

        _fwlFileHelper.CreateNewFileWithNewWorldName(sourcePath, fwlFileName, newName);

        var readNewData = _fwlFileHelper.ReadFile(Path.Combine(sourcePath, newName + ".fwl"));

        Assert.Equal(newName, _fwlFileHelper.ReadWorldName(readNewData));
    }
    
    [Fact]
    public void NewWorlds()
    {
        var sourcePath = @"C:\Users\ravel\Downloads\Vierhalla";

        var fwlFileName = "world.fwl";
        var newName = "Vierhalla20240529";

        for (var x = 'a'; x <= 'f'; x++)
        {
            _fwlFileHelper.CreateNewFileWithNewWorldName(sourcePath, fwlFileName, newName + x);
            var readNewData = _fwlFileHelper.ReadFile(Path.Combine(sourcePath, newName + x + ".fwl"));

            Assert.Equal(newName + x, _fwlFileHelper.ReadWorldName(readNewData));
            File.Copy(Path.Combine(sourcePath, "world.db"), Path.Combine(sourcePath, newName + x + ".db"));
        }
    }
}