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
    
    object k = 1;
    HashSet<string> hashSet = new HashSet<string>();
    int flagDuplicateCount = 0;

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
            
            if (CheckImageUnique(byteArray))
            {
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string specificFolder = Path.Combine(folder, this.TargetPath + $"/{k}.jpg");
                ByteArrayToFile(byteArray, specificFolder);
                
                if (Progress != null)
                {
                    Progress.Invoke(this, new DownloadEventArgs(){Message = $"{k}"});
                }
                k = (int)k + 1;                
            }
        });
    }

    private bool CheckImageUnique(byte[] byteArray)
    {
        lock (hashSet)
        {
            string base64str = Convert.ToBase64String(byteArray, 0, byteArray.Length);
            bool res = hashSet.Add(base64str);
            
            if (PreventDuplicateOn)
            {
                if (!res)
                {
                    flagDuplicateCount += 1;
                    Console.WriteLine("repetitive image blocked by app security");
                }
                return res;
            }
            else
            {
                if (!res)
                {
                    Console.WriteLine("repetitive image detected ! app security is off !");
                }
                return true;
            }
        }
    }
    

    #endregion
    
    #region PUBLIC FUNCTIONS
public IDownloadResult Download()
    {
        try
        {
            CheckTargetPath();
            int total = this.TotalRequestFile;
            int parallel = this.TotalRequestForParallelFile;
            
            DownloadPart(total, parallel);
            void DownloadPart(int total, int parallel)
            {
                int reminder = CalculateIterateReminder(total, parallel);
                flagDuplicateCount = 0;
                while (parallel <= total)
                {
                    GetImageFromResourceAsParallel(parallel); 
                    total -= parallel;                 
                };

                if (reminder > 0 || flagDuplicateCount > 0)
                {
                    total = reminder + flagDuplicateCount;
                    if (parallel < total)
                    {
                        DownloadPart(total, parallel);
                    }
                    else
                    {
                        DownloadPart(total, total);
                        //GetImageFromResourceAsParallel(total); 
                    }
                }
            }
            
            DownloadResult.IsDownloaded = true;
            //DownloadResult.ResultMessage = "an image downloaded successfully";
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
    private int CalculateIterateReminder(int total, int parallel)
    {
        bool totalRequestFileIsDivided = total % parallel == 0; 
        return !totalRequestFileIsDivided ? total % parallel : 0;
    }

    public event EventHandler? Progress;
    public bool PreventDuplicateOn { get; set; }

    #endregion
    
}