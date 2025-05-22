public static class Birch
{
    private static Dictionary<string, List<string[]>> FunctionLibrary = new();
    
    private static Dictionary<string, float> GlobalNumbers = new();
    private static Dictionary<string, string> GlobalStrings = new();

    private static Dictionary<string, float> LocalNumbers = new();
    private static Dictionary<string, float> LocalStrings = new();
    
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

        if (projectFolder.Contains(".br"))
        {
            SingleFile = true;
            initFilePath = projectFolder;
        }
        else
        {
            initFilePath = Path.Combine(projectFolder, "Index.br");
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
                Console.WriteLine("Make sure to provide a Script file or a Directory with Index.br");
            }

            CrashError("Please provide a valid EchoScript file or a Directory with Index.br");
        }
        
        string IndexFile = File.ReadAllText(initFilePath);
        #endregion

        #region Function Registration
        
        IndexFile = IndexFile.Replace("\n", "")
            .Replace("\t", "")
            .Replace("\r", "")
            .Replace("    ", "")
            .Replace("      ", "");
        
        
        

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
                        // FunctionLibrary.Add(functionName, InstructionLines.ToArray());
                        RegisterFunction(functionName,InstructionLines );

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

        // Console.WriteLine(FunctionLibrary.Count);
        // Console.WriteLine(String.Join(", ", FunctionLibrary.Keys));

        if (!FunctionLibrary.ContainsKey("main"))
        {
            Console.WriteLine("File does not contain a function named 'main'");
            Console.WriteLine("Press any key to exit..."); Console.ReadKey(); 
            return;
        }
        
        #endregion
        
        CallFunction("main");
    }


    public static void RegisterFunction(string functionName, List<string> functionBlock)
    {

        Console.WriteLine($"Function Name: {functionName}");
        int index = 0;
        
        List<string[]> FunctionInstructions = new();

        foreach (var instruction in functionBlock)
        {
            if (string.IsNullOrWhiteSpace(instruction)) continue;

            List<string> SplitInstruction = new List<string>();
            bool IsString = false;
            string token = "";

            for (int i = 0; i < instruction.Length; i++)
            {
                char c = instruction[i];
                if (c == '"')
                {
                    IsString = !IsString;
                    continue;
                }
                if (char.IsWhiteSpace(c) && !IsString)
                {
                    if (token.Length > 0)
                    {
                        SplitInstruction.Add(token);
                        token = "";
                    }
                }
                else
                {
                    token += c;
                }
            }

            if (token.Length > 0)
            {
                SplitInstruction.Add(token);
            }

            // Console.WriteLine($"[{index}] : ({string.Join(",", SplitInstruction)})");
            FunctionInstructions.Add(SplitInstruction.ToArray());
            index++;
        }
        
        FunctionLibrary.Add(functionName, FunctionInstructions);
    }

    public static void CallFunction(string functionName)
    {
        if (!FunctionLibrary.ContainsKey(functionName)) CrashError($"Function {functionName} does not exist");
        
        
    }

    public static void CrashError(string ErrorMsg)
    {
        Console.WriteLine($"ERROR: {ErrorMsg}");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
        Environment.Exit(0);
    }


}