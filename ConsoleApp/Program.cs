using ConsoleApp.Core.Abstract;
using ConsoleApp.Core.Concrete;

#region ARGUMENTS
Console.WriteLine("Hi, welcome to image download party !");
Console.WriteLine("Enter the number of images to download:");
string countImagesToDownload = Console.ReadLine();
Console.WriteLine("Enter the maximum parallel download limit:");
string countParallelDownloadLimit = Console.ReadLine();
Console.WriteLine("Enter the save path: ");
Console.WriteLine("the application output path is designed for your os desktop. Default value is 'output'");
string folderName = Console.ReadLine();
#endregion
#region VALIDATION
bool ValidateInput()
{
    int num1, num2;
    bool b1 = int.TryParse(countImagesToDownload, out num1);
    bool b2 = int.TryParse(countParallelDownloadLimit, out num2);
    return b1 && b2;
}
if (ValidateInput())
{
    var p1 = int.Parse(countImagesToDownload);
    var p2 = int.Parse(countParallelDownloadLimit);
    if (p1 > p2)
    {
        Init(new DownloadManager(), 
            new DownloadResult(),
            p1, 
            p2, folderName);    
    }
    else
    {
        Console.WriteLine("The number of images to download should be greater than the maximum parallel download limit.");
        Console.WriteLine("Program exited. Please try again.");
    }
}
else
{
    Console.WriteLine("arguments are not valid ! Program exited. Please try again");
}
#endregion


void Init(IDownloadManager manager, IDownloadResult downloadResult, int countImagesToDownload, int countParallelDownloadLimit, string outputFolderName)
{
    manager.TotalRequestFile = countImagesToDownload;
    manager.TotalRequestForParallelFile = countParallelDownloadLimit;
    manager.SourceURL = "https://picsum.photos/640/480";
    manager.DownloadResult = downloadResult;
    manager.TargetPath = !string.IsNullOrEmpty(outputFolderName) ? outputFolderName : "output";
    manager.PreventDuplicateOn = true;
    
    Console.WriteLine($"$Downloading {countImagesToDownload} images ({countParallelDownloadLimit} parallel download at most)");
    manager.Progress += (sender, args) =>
    {
        DownloadEventArgs a = args as DownloadEventArgs;
        Console.WriteLine($"Progress: {a.Message}/{countImagesToDownload}");
    };
    downloadResult = manager.Download();
        
    if (downloadResult.IsDownloaded)
    {
        Console.WriteLine("Operation completed successfully.");
    }
}









