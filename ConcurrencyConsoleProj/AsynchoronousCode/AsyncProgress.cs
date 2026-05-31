namespace ConcurrencyConsoleProj.AsynchoronousCode;

public class AsyncProgress
{
    public static async Task ProgressBar()
    {
        var progress = new Progress<int>();
        progress.ProgressChanged += (s, e) =>
        {
            Console.WriteLine(e);
        };
        
        await DoProgress(progress);
    }

    private static Task DoProgress(IProgress<int>? progress)
    {
        var progressPercent = 0;

        while (progressPercent <= 100)
        {
            progress?.Report(progressPercent);
            progressPercent++;
        }

        return Task.CompletedTask;
    }
}