namespace ConsoleApp.Core.Concrete;

public class DownloadResult : Abstract.IDownloadResult
{
    public string ResultMessage { get; set; }
    /// <summary>
    /// Return true if download completed successfully
    /// Return false if download completed with errors
    /// </summary>
    public bool IsDownloaded { get; set; }

    public object ResultObject { get; set; }
}