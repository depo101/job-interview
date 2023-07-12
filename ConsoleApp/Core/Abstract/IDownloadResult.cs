namespace ConsoleApp.Core.Abstract;

public interface IDownloadResult
{
    string ResultMessage { get; set; }
    bool IsDownloaded { get; set; }
    
    object ResultObject { get; set; }
}