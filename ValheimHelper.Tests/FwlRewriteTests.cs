using System.IO;
using Xunit.Abstractions;

namespace ValheimHelper.Tests;

public class FwlRewriteTests
{
    private readonly ITestOutputHelper _output;
    private readonly FwlFileHelper _fwlFileHelper;

    public FwlRewriteTests(ITestOutputHelper output)
    {
        _output = output;

        _fwlFileHelper = new FwlFileHelper(XUnitLogger.CreateLogger<FwlFileHelper>(output));
    }
    

    [Fact]
    public void ReadWorldName()
    {
        var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var sourcePath = $@"{userFolder}\Downloads\Vierhalla";

        var fwlFileName = "world.fwl";

        var fullPath = Path.Combine(sourcePath, fwlFileName);

        var data = _fwlFileHelper.ReadFile(fullPath);

        Assert.Equal("world", _fwlFileHelper.ReadWorldName(data));
    }


    [Fact]
    public void NewFileWithNewWorldName()
    {
        var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var sourcePath = Path.Combine(userFolder, @"Downloads\Vierhalla");

        var fwlFileName = "world.fwl";

        var fullPath = Path.Combine(sourcePath, fwlFileName);

        var data = _fwlFileHelper.ReadFile(fullPath);

        var newName = "TestNewName001";

        var newData = _fwlFileHelper.UpdateWorldName(data, newName);

        var newFileName = Path.Combine(sourcePath, newName + ".fwl");

        _fwlFileHelper.WriteFile(newFileName, newData);

        var readNewData = _fwlFileHelper.ReadFile(newFileName);

        Assert.Equal(newName, _fwlFileHelper.ReadWorldName(readNewData));
    }

    [Fact]
    public void NewWorldWithShortMethod()
    {
        var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var sourcePath = Path.Combine(userFolder, @"Downloads\Vierhalla");

        var fwlFileName = "world.fwl";
        var newName = "TestNewName002";

        _fwlFileHelper.CreateNewFileWithNewWorldName(sourcePath, fwlFileName, newName);

        var readNewData = _fwlFileHelper.ReadFile(Path.Combine(sourcePath, newName + ".fwl"));

        Assert.Equal(newName, _fwlFileHelper.ReadWorldName(readNewData));
    }
    
    [Fact]
    public void NewWorlds()
    {
        var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var sourcePath = Path.Combine(userFolder, @"Downloads\Vierhalla");
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var localWorldsPath = Path.Combine(Directory.GetParent(appData).ToString(), @"LocalLow\IronGate\Valheim\worlds_local");
        var fwlFileName = "world.fwl";
        var newName = "TestNewName003";
        var maxVariant = 'b';

        CopyWorldToNewName(fwlFileName, sourcePath, newName);
        
        for (var x = 'a'; x < maxVariant; x++)
            CopyWorldToNewName(fwlFileName, sourcePath, $"{newName}-{x}", localWorldsPath);
    }

    private void CopyWorldToNewName(string fwlFileName, string sourcePath, string newName, string localWorldsPath = null)
    {
        var baseFileName = Path.GetFileNameWithoutExtension(fwlFileName);

        _fwlFileHelper.CreateNewFileWithNewWorldName(sourcePath, fwlFileName, newName);

        var newFwlFilePath = Path.Combine(sourcePath, $"{newName}.fwl");
        var readNewData = _fwlFileHelper.ReadFile(newFwlFilePath);

        Assert.Equal(newName, _fwlFileHelper.ReadWorldName(readNewData));

        var dbCopyFilePath = Path.Combine(sourcePath, $"{newName}.db");
        File.Copy(Path.Combine(sourcePath, $"{baseFileName}.db"), dbCopyFilePath);

        if (string.IsNullOrWhiteSpace(localWorldsPath)) return;
        
        File.Move(newFwlFilePath, Path.Combine(localWorldsPath, $"{newName}.fwl"));
        File.Move(dbCopyFilePath, Path.Combine(localWorldsPath, $"{newName}.db"));
    }
}