using System.Net;
using System.Net.Mime;
using System.Text;
using ConsoleApp.Core.Abstract;


namespace ConsoleApp.Core.Concrete;

public class DownloadManager : Abstract.IDownloadManager
{
    #region FIELDS AND PROPERTIES
    public string SourceURL { get; set; }
    public IDownloadResult DownloadResult { get; set; }
    public string TargetPath { get; set; }
    public int TotalRequestFile { get; set; }
    public int TotalRequestForParallelFile { get; set; }
    int IterateReminderCount { get; set; }
    object k = 1;
    

    #endregion

    #region PRIVATE FUNCTIONS
    private void GetImageFromResourceAsParallel(int parallelCount)
    {
        lock (k)
        {
            
        }
        Parallel.For(0, parallelCount, i =>
        {
            HttpClient client = new HttpClient();
            Task<byte[]> response = client.GetByteArrayAsync(this.SourceURL);
            var byteArray = response.Result;
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string specificFolder = Path.Combine(folder, this.TargetPath + $"/{k}.jpg");
            ByteArrayToFile(byteArray, specificFolder);
            
            if (Progress != null)
            {
                Progress.Invoke(this, new DownloadEventArgs(){Message = $"{k}"});
            }
            
            k = (int)k + 1;
        });
    }
    

    #endregion
    
    #region PUBLIC FUNCTIONS
public IDownloadResult Download()
    {
        CheckTargetPath();
        CalculateIterateTime();
        try
        {
            while (this.TotalRequestForParallelFile <= this.TotalRequestFile)
            {
                GetImageFromResourceAsParallel(this.TotalRequestForParallelFile);
                this.TotalRequestFile -= this.TotalRequestForParallelFile;                
            };

            if (IterateReminderCount > 0)
            {
                GetImageFromResourceAsParallel(this.IterateReminderCount);
            }
            
            DownloadResult.IsDownloaded = true;
            DownloadResult.ResultMessage = "an image downloaded successfully";
            //DownloadResult.ResultObject = ByteArrayToHex(byteArray);
            return DownloadResult;
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    public string ByteArrayToHex(byte[] byteArray)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var b in byteArray)
        {
            sb.Append($"{b:X}");
            sb.Append(" ");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Create directory if not exists
    /// </summary>
    public void CheckTargetPath()
    {
        string folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string specificFolder = Path.Combine(folder, this.TargetPath);

        bool checkPath = Path.Exists(specificFolder);
        if (!checkPath)
        {
            System.IO.Directory.CreateDirectory(specificFolder);
        }
    }
    
    public void ByteArrayToFile(byte[] byteArray, string path)
    {
        //todo: kod tekrarı düzeltilecek
        MemoryStream memoryStream = new System.IO.MemoryStream(byteArray);
        FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        memoryStream.WriteTo(fs);
        memoryStream.Close();
        fs.Close();
    }
    public void CalculateIterateTime()
    {
        bool totalRequestFileIsDivided = this.TotalRequestFile % this.TotalRequestForParallelFile == 0; 
        IterateReminderCount = !totalRequestFileIsDivided ? this.TotalRequestFile % this.TotalRequestForParallelFile : 0;
    }

    public event EventHandler? Progress;

    

    #endregion
    
}