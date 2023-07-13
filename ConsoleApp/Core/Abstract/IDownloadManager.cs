namespace ConsoleApp.Core.Abstract;

public interface IDownloadManager
{
    string SourceURL { get; set; }
    IDownloadResult DownloadResult { get; set; }
    IDownloadResult Download();
    string ByteArrayToHex(byte[] byteArray);
    string TargetPath { get; set; }

    void ByteArrayToFile(byte[] byteArray, string path);
    
    int TotalRequestFile { get; set; }
    int TotalRequestForParallelFile { get; set; }
    

    event EventHandler Progress;

    bool PreventDuplicateOn { get; set; }
}