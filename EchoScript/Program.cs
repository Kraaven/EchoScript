public class ES
{
    public static void Main(String[] args)
    {
        #region FileInput
        
        string projectFolder = "";
        if (args.Length == 0)
        {
            Console.WriteLine("Initiating EchoScript Terminal CommandLine");

            do
            {
                Console.Write("Relative directory to project/file? : ");
                projectFolder = Console.ReadLine();
            } while (projectFolder == null || projectFolder.Trim().Equals(""));
        }
        else
        {
            projectFolder = args[0];
        }

        string initFilePath = "";
        bool SingleFile = false;

        if (projectFolder.Contains(".es"))
        {
            SingleFile = true;
            initFilePath = projectFolder;
        }
        else
        {
            initFilePath = Path.Combine(projectFolder, "Index.es");
        }
        if (!File.Exists(initFilePath))
        {
            if (SingleFile)
            {
                Console.WriteLine("Given path does not point to a valid EchoScript file");
            }
            else
            {
                Console.WriteLine("Given path does not point to a valid EchoScript Project");
                Console.WriteLine("Make sure to provide a Script file or a Directory with Index.es");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            return;
        }
        
        string IndexFile = File.ReadAllText(initFilePath);
        #endregion
        
        Console.WriteLine(IndexFile);
    }
}