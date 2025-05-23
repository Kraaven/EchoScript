public static class Birch
{
    private static Dictionary<string, List<string[]>> FunctionLibrary = new();
    
    private static Dictionary<string, float> GlobalNumbers = new();
    private static Dictionary<string, string> GlobalStrings = new();

    // private static Dictionary<string, float> LocalNumbers = new();
    // private static Dictionary<string, string> LocalStrings = new();
    
    public static void Main(String[] args)
    {
        #region FileInput
        
        string projectFolder = "";
        if (args.Length == 0)
        {
            Console.WriteLine("Initiating Birch Terminal CommandLine");

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
                Console.WriteLine("Given path does not point to a valid Birch file");
            }
            else
            {
                Console.WriteLine("Given path does not point to a valid Birch Project");
                Console.WriteLine("Make sure to provide a Script file or a Directory with Index.br");
            }

            CrashError("Please provide a valid Birch file or a Directory with Index.br");
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

        #region SystemSetup
        
        // GlobalNumbers.Add("", 0);
        GlobalStrings.Add("str-in", "");
        GlobalStrings.Add("str-out", "");
        
        GlobalNumbers.Add("num-in", 0);
        GlobalNumbers.Add("num-out", 0);
        
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

            Console.WriteLine($"[{index}] : ({string.Join(",", SplitInstruction)})");
            FunctionInstructions.Add(SplitInstruction.ToArray());
            index++;
        }
        
        FunctionLibrary.Add(functionName, FunctionInstructions);
    }

    public static void CallFunction(string functionName)
    {
        if (!FunctionLibrary.ContainsKey(functionName)) CrashError($"Function {functionName} does not exist");
        Dictionary<string, float> LocalNumbers = new();
        Dictionary<string, string> LocalStrings = new();
        
        foreach (var Instruction in FunctionLibrary[functionName])
        {

            //Function Calling
            switch (Instruction[0])
            {
                case "call":
                    if(Instruction.Length < 3 || Instruction.Length > 3) CrashError("Invalid Syntax, invalid number of arguments");
                    if(Instruction[1] != ":") CrashError("Invalid Syntax, No : Operator");
                    
                    Console.WriteLine($"Calling function {Instruction[2]}");
                    CallFunction(Instruction[2]);
                    continue;
                case "syscall":
                    if(Instruction.Length < 3 || Instruction.Length > 3) CrashError("Invalid Syntax, invalid number of arguments");
                    if(Instruction[1] != ":") CrashError("Invalid Syntax, No : Operator");

                    RunSystemInstruction(Instruction[2]);
                    
                    continue;
            }
            
            //Variable Declaration
            switch (Instruction[0])
            {
                case "num":
                    if(Instruction.Length < 4) CrashError("Invalid Syntax, Instruction is incomplete");
                    if(!IsVariableValid(Instruction[1], LocalNumbers, LocalStrings)) CrashError("Invalid Syntax, Not a valid Variable Name");
                    if(Instruction[2] != "=") CrashError("Invalid Syntax, no assigment operator");
                    
                    // if (float.TryParse(Instruction[3], out var numResult)) LocalNumbers.Add(Instruction[1], numResult);
                    // else CrashError("Invalid Syntax, argument must be a number");

                    string[] Expression = new string[Instruction.Length - 3];
                    for (int i = 0; i < Expression.Length; i++)
                    {
                        Expression[i] = Instruction[i + 3];
                    }
                    
                    LocalNumbers.Add(Instruction[1], EvaluateExpression(Expression, LocalNumbers, LocalStrings));
                    // Console.WriteLine(string.Join("|", Expression));
                    // Console.WriteLine($"{Instruction[1]} added with value {LocalNumbers[Instruction[1]]}"); 
                    
                    continue;
                case "str":
                    if(Instruction.Length < 4) CrashError("Invalid Syntax, Instruction is incomplete");
                    if(!IsVariableValid(Instruction[1], LocalNumbers, LocalStrings)) CrashError("Invalid Syntax, Not a valid Variable Name");
                    if(Instruction.Length != 4) CrashError("Invalid Syntax, multiple tokens for string declaration");
                    if(Instruction[2] != "=") CrashError("Invalid Syntax, no assigment operator");
                    
                    
                    if (GlobalStrings.ContainsKey(Instruction[3]))
                    {
                        AssignString( Instruction[3], Instruction,LocalNumbers, LocalStrings, GlobalStrings);
                    }
                    else if (LocalStrings.ContainsKey( Instruction[3]))
                    {
                        AssignString( Instruction[3], Instruction,LocalNumbers, LocalStrings, LocalStrings);
                    }
                    else
                    {
                        LocalStrings.Add(Instruction[1], Instruction[3]);
                    }
                    
                    // Console.WriteLine($"{Instruction[1]} added with value {LocalStrings[Instruction[1]]}");

                    continue;
                
                default:
                    break;
                
            }
            
            //Variable Re-assignment
            if(Instruction.Length < 3) CrashError("Invalid Syntax, instruction is incomplete");
            
            string variable = Instruction[0];
            string operation = Instruction[1];

            if (operation != "=")
            {
                CrashError("Invalid Syntax, instruction is nonsense");
                return;
            }

            // Handle numeric assignments
            if (GlobalNumbers.ContainsKey(variable))
            {
                AssignNumber(variable, Instruction,LocalNumbers, LocalStrings, GlobalNumbers);
            }
            else if (LocalNumbers.ContainsKey(variable))
            {
                AssignNumber(variable, Instruction,LocalNumbers, LocalStrings, LocalNumbers);
            }
            // Handle string assignments
            else if (GlobalStrings.ContainsKey(variable))
            {
                // Console.WriteLine($"Global Strings contains variable {variable}");
                AssignString(variable, Instruction,LocalNumbers, LocalStrings, GlobalStrings);
            }
            else if (LocalStrings.ContainsKey(variable))
            {
                AssignString(variable, Instruction,LocalNumbers, LocalStrings, LocalStrings);
            }
            else
            {
                CrashError("Invalid Syntax, variable does not exist");
            }
            
        }
    }

    static void AssignNumber(string variable, string[] instruction, Dictionary<string, float> LocalNumbers, Dictionary<string, string> LocalStrings,  Dictionary<string, float> targetDict)
    {
        if (!targetDict.ContainsKey(variable))
            CrashError("Invalid Syntax, Variable does not exist");

        var expression = instruction.Skip(2).ToArray();
        targetDict[variable] = EvaluateExpression(expression, LocalNumbers, LocalStrings);
    }

    static void AssignString(string variable, string[] instruction, Dictionary<string, float> LocalNumbers, Dictionary<string, string> LocalStrings, Dictionary<string, string> targetDict)
    {
        if (!targetDict.ContainsKey(variable))
            CrashError("Invalid Syntax, Variable does not exist");

        string valueKey = instruction[2];

        if (LocalStrings.ContainsKey(valueKey))
        {
            targetDict[variable] = LocalStrings[valueKey];
        }
        else if (GlobalStrings.ContainsKey(valueKey))
        {
            targetDict[variable] = GlobalStrings[valueKey];
        }
        else
        {
            targetDict[variable] = valueKey;
        }
    }

    private static void RunSystemInstruction(string functionName)
    {
        switch (functionName)
        {
            case "write": 
                Console.WriteLine($">> {GlobalStrings["str-in"]}");
                break;
            case "read":
                var input = Console.ReadLine();
                if (input == null)
                {
                    Console.WriteLine($">> Input not valid");
                    break;
                }
                GlobalStrings["str-out"] = input;
                break;
            case "toNum":
                break;
            case "toString":
                break;
            default:
                CrashError("Invalid Syntax, system function does not exist");
                break;
            
        }
    }

    public static void CrashError(string ErrorMsg)
    {
        Console.WriteLine($"ERROR: {ErrorMsg}");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
        Environment.Exit(0);
    }
    
    public static float EvaluateExpression(string[] arguments, Dictionary<string, float> localNumbers, Dictionary<string, string> localStrings)
    {
        List<string> postfix = InfixToPostfix(arguments, localNumbers, localStrings);
        return EvaluatePostfix(postfix);
    }

    private static readonly Dictionary<string, int> precedence = new Dictionary<string, int>
        {
            { "+", 1 },
            { "-", 1 },
            { "*", 2 },
            { "/", 2 }
        };
    private static List<string> InfixToPostfix(string[] tokens, Dictionary<string, float> localNumbers, Dictionary<string, string> localStrings)
    {
        List<string> output = new List<string>();
        Stack<string> operators = new Stack<string>();
        
        foreach (var token in tokens)
        {
            if (float.TryParse(token, out _))
            {
                output.Add(token);
            }
            else if (precedence.ContainsKey(token))
            {
                while (operators.Count > 0 && precedence.ContainsKey(operators.Peek()) &&
                       precedence[operators.Peek()] >= precedence[token])
                {
                    output.Add(operators.Pop());
                }
                operators.Push(token);
            }
            else if (token == "(")
            {
                operators.Push(token);
            }
            else if (token == ")")
            {
                while (operators.Count > 0 && operators.Peek() != "(")
                {
                    output.Add(operators.Pop());
                }
                if (operators.Count == 0 || operators.Pop() != "(")
                {
                    throw new ArgumentException("Mismatched parentheses");
                }
            }
            else if (GlobalNumbers.ContainsKey(token))
            {
                output.Add(localNumbers[token].ToString());
            }
            else if (localNumbers.ContainsKey(token))
            {
                output.Add(localNumbers[token].ToString());
            }
            else
            {
                CrashError("Invalid Syntax, expression has wrong sequence of operators or invalid variable calls");
            }
        }

        while (operators.Count > 0)
        {
            var op = operators.Pop();
            if (op == "(" || op == ")")
            {
                throw new ArgumentException("Mismatched parentheses");
            }
            output.Add(op);
        }

        return output;
    }

    private static float EvaluatePostfix(List<string> postfix)
    {
        Stack<float> stack = new Stack<float>();

        foreach (string token in postfix)
        {
            if (float.TryParse(token, out float num))
            {
                stack.Push(num);
            }
            else
            {
                float right = stack.Pop();
                float left = stack.Pop();
                switch (token)
                {
                    case "+": stack.Push(left + right); break;
                    case "-": stack.Push(left - right); break;
                    case "*": stack.Push(left * right); break;
                    case "/": stack.Push(left / right); break;
                    default: throw new InvalidOperationException($"Invalid operator {token}");
                }
            }
        }

        return stack.Pop();
    }

    private static bool IsVariableValid(string variableName, Dictionary<string, float> localNumbers, Dictionary<string, string> localStrings)
    {
        if(variableName.Contains(' ')) return false;
        if(variableName.Contains('.')) return false;
        foreach (var ch in variableName) if(char.IsDigit(ch)) return false;
        
        if(localNumbers.ContainsKey(variableName)) return false;
        if(localStrings.ContainsKey(variableName)) return false;
        
        if(GlobalNumbers.ContainsKey(variableName)) return false;
        if(GlobalStrings.ContainsKey(variableName)) return false;

        return true;
    }


}