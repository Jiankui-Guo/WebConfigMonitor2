using System;
using System.IO;
using System.Security.Permissions;

public class Watcher
{
    private static FileSystemWatcher watcher = null;
    private static string dir = "";

    public static void Main()
    {
        Run();
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public static void Run()
    {
        string[] args = System.Environment.GetCommandLineArgs();

        // If a directory is not specified, exit program.
        if (args.Length != 2)
        {
            // Display the proper way to call the program.
            Console.WriteLine("Usage: Watcher.exe (directory)");
            return;
        }

        dir = args[1];

        string backDir = Path.Combine(dir, "CONFIGBAK");
        if (!Directory.Exists(backDir))
            Directory.CreateDirectory(backDir);

        //Save the first copy
        File.Copy(Path.Combine(dir, "web.config"),
            Path.Combine(backDir, DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + "_" + "web.config"));

        // Create a new FileSystemWatcher and set its properties.
        watcher = new FileSystemWatcher();
        watcher.Path = dir;
        /* Watch for changes in LastAccess and LastWrite times, and
           the renaming of files or directories. */
        watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Size;
        //| NotifyFilters.FileName | NotifyFilters.DirectoryName;
        // Only watch text files.
        watcher.Filter = "web.config";

        // Add event handlers.
        watcher.Changed += new FileSystemEventHandler(OnChanged);

        // Begin watching.
        watcher.EnableRaisingEvents = true;

        // Wait for the user to quit the program.
        Console.WriteLine("Press \'q\' to quit.");
        while (Console.Read() != 'q') ;
    }

    // Define the event handlers.
    private static void OnChanged(object source, FileSystemEventArgs e)
    {
        try
        {
            watcher.EnableRaisingEvents = false;
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);

            File.Copy(e.FullPath, 
                Path.Combine(Path.Combine(dir, "CONFIGBAK"), DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + "_" + e.Name));
        }
        finally
        {
            watcher.EnableRaisingEvents = true;
        }
    }

}
