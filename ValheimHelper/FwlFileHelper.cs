using System.Text;
using Microsoft.Extensions.Logging;

namespace ValheimHelper
{
    public class FwlFileHelper
    {
        private readonly ILogger<FwlFileHelper> _logger;

        public FwlFileHelper(ILogger<FwlFileHelper> logger)
        {
            _logger = logger;
        }

        public byte[] ReadFile(string path)
        {
            return File.ReadAllBytes(path);
        }

        public void WriteFile(string path, byte[] data)
        {
            File.WriteAllBytes(path, data);
        }

        public byte[] UpdateWorldName(byte[] data, string newWorldName)
        {
            var originalFileLength = data.Length;

            var newWorldNameBytes = Encoding.UTF8.GetBytes(newWorldName);
            
            var worldNameLength = (int)data[8];

            var newFileLength = originalFileLength - worldNameLength + newWorldNameBytes.Length;

            var outputBytes = new byte[newFileLength];

            // Set the new length of the rest of the file to the start of the file (-4 for 32 bit integer to start)
            BitConverter.GetBytes(newFileLength - 4).CopyTo(outputBytes, 0);

            // Insert the next 4 bytes from the original file
            Array.Copy(data, 4, outputBytes, 4, 4);

            // Set the world name length to the start of the world name
            outputBytes[8] = (byte)newWorldNameBytes.Length;

            Array.Copy(newWorldNameBytes, 0, outputBytes, 9, newWorldNameBytes.Length);

            Array.Copy(data, 9 + worldNameLength, outputBytes, 9 + newWorldNameBytes.Length, originalFileLength - 9 - worldNameLength);

            return outputBytes;
        }

        public string ReadWorldName(byte[] data)
        {
            var worldNameOffset = 0x08;
            var worldNameLength = (int)data[worldNameOffset];

            var worldNameBytes = new byte[worldNameLength];

            Array.Copy(data, worldNameOffset + 1, worldNameBytes, 0, worldNameLength);

            return Encoding.UTF8.GetString(worldNameBytes);
        }

        public void CreateNewFileWithNewWorldName(string sourcePath, string fwlFileName, string newWorldName)
        {
            var fullPath = Path.Combine(sourcePath, fwlFileName);

            var data = ReadFile(fullPath);

            var worldName = ReadWorldName(data);
            _logger.LogInformation($"World name: {worldName}");

            var newData = UpdateWorldName(data, newWorldName);

            var newFileName = Path.Combine(sourcePath, newWorldName + ".fwl");

            if (newFileName == fullPath)
            {
                throw new Exception("New file name is the same as the old file name");
            }   

            WriteFile(newFileName, newData);
        }
    }
}