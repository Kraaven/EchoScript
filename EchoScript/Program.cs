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
        IndexFile = IndexFile.Replace("\n", "")
            .Replace("\t", "")
            .Replace("\r", "")
            .Replace("    ", "")
            .Replace("      ", "");
        
        
        Dictionary<string, string[]> FunctionLibrary = new Dictionary<string, string[]>();

        {
            string tempBlock = "";
            string functionName = "";
            bool blockStarted = false;
            List<string> InstructionLines = new List<string>();

            for (int i = 0; i < IndexFile.Length; i++)
            {
                char currentChar = IndexFile[i];

                switch (currentChar)
                {
                    case '.':
                        tempBlock = "";
                        break;

                    case '{':
                        functionName = tempBlock;
                        tempBlock = "";
                        break;

                    case ';':
                        InstructionLines.Add(tempBlock.Trim());
                        tempBlock = "";
                        break;

                    case '}':
                        FunctionLibrary.Add(functionName, InstructionLines.ToArray());

                        tempBlock = "";
                        functionName = "";
                        InstructionLines.Clear();
                        break;

                    default:
                        tempBlock += currentChar;
                        break;
                }
            }
        }

        Console.WriteLine(FunctionLibrary.Count);
        Console.WriteLine(String.Join(", ", FunctionLibrary.Keys));
        
        Console.WriteLine(String.Join(" -> ", FunctionLibrary["main"]));
        
    }
}