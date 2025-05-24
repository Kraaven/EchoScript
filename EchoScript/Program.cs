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

            CrashError("Please provide a valid Birch file or a Directory with Index.br", new []{"No file loaded for Instruction"});
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
        
        GlobalStrings.Add("sys-console", "");
        
        GlobalStrings.Add("STR-i", "");
        GlobalStrings.Add("STR-x", "");
        GlobalStrings.Add("STR-c", "");
        GlobalStrings.Add("STR-l", "");
        
        GlobalNumbers.Add("NUM-i", 0);
        GlobalNumbers.Add("NUM-x", 0);
        GlobalNumbers.Add("NUM-c", 0);
        GlobalNumbers.Add("NUM-l", 0);
        
        #endregion
        CallFunction("main");
    }


    public static void RegisterFunction(string functionName, List<string> functionBlock)
    {

        // Console.WriteLine($"Function Name: {functionName}");
        int index = 0;
        
        List<string[]> FunctionInstructions = new();

        foreach (var instruction in functionBlock)
        {
            if (string.IsNullOrWhiteSpace(instruction)) continue;

            List<string> SplitInstruction = new List<string>();
            bool IsString = false;
            bool IsComment = false;
            string token = "";

            for (int i = 0; i < instruction.Length; i++)
            {
                char c = instruction[i];
                if (c == '"')
                {
                    IsString = !IsString;
                    continue;
                }

                if (c == '#')
                {
                    IsComment = !IsComment;
                    continue;
                }

                if (IsComment)
                {
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
        if (!FunctionLibrary.ContainsKey(functionName)) CrashError($"Function {functionName} does not exist", new []{"None"});
        Dictionary<string, float> LocalNumbers = new();
        Dictionary<string, string> LocalStrings = new();
        
        foreach (var Instruction in FunctionLibrary[functionName])
        {

            //Function Calling
            switch (Instruction[0])
            {
                case "call":
                    if(Instruction.Length < 3 || Instruction.Length > 3) CrashError("Invalid Syntax, invalid number of arguments", Instruction);
                    if(Instruction[1] != ":") CrashError("Invalid Syntax, No : Operator", Instruction);
                    
                    // Console.WriteLine($"Calling function {Instruction[2]}");
                    CallFunction(Instruction[2]);
                    continue;
                case "syscall":
                    if(Instruction.Length < 3 || Instruction.Length > 3) CrashError("Invalid Syntax, invalid number of arguments", Instruction);
                    if(Instruction[1] != ":") CrashError("Invalid Syntax, No : Operator", Instruction);

                    RunSystemInstruction(Instruction[2]);
                    
                    continue;
            }
            
            //Variable Declaration
            switch (Instruction[0])
            {
                case "num":
                    if(Instruction.Length < 4) CrashError("Invalid Syntax, Instruction is incomplete", Instruction);
                    if(!IsVariableFormatValid(Instruction[1])) CrashError("Invalid Syntax, Not a valid Variable Name", Instruction);
                    if(Instruction[2] != "=") CrashError("Invalid Syntax, no assigment operator", Instruction);
                    
                    if(ConfirmNumVariable(Instruction[1], LocalNumbers).isvariable) CrashError("Invalid Syntax, variable already defined", Instruction);
                    else LocalNumbers.Add(Instruction[1], EvaluateExpression(Instruction.Skip(3).ToArray(), LocalNumbers));
                    
                    continue;
                case "str":
                    if(Instruction.Length < 4) CrashError("Invalid Syntax, Instruction is incomplete", Instruction);
                    if(!IsVariableFormatValid(Instruction[1])) CrashError("Invalid Syntax, Not a valid Variable Name", Instruction);
                    if(Instruction[2] != "=") CrashError("Invalid Syntax, no assigment operator", Instruction);
                    
                    var affirm = ConfirmStrVariable(Instruction[0], LocalStrings);
                    if(affirm.isvariable) CrashError("Invalid Syntax, variable already defined", Instruction);
                    else LocalStrings.Add(Instruction[1], EvaluateStringExpression(Instruction.Skip(3).ToArray(), LocalStrings));

                    continue;
                
                default:
                    break;
            }
            
            //Variable Re-assignment
            if(Instruction.Length < 3) CrashError("Invalid Syntax, instruction is incomplete", Instruction);
            
            string variable = Instruction[0];
            string operation = Instruction[1];

            if (operation != "=")
            {
                CrashError("Invalid Syntax, instruction is nonsense", Instruction);
                return;
            }
            
            var strAffirm = ConfirmStrVariable(variable, LocalStrings);
            var numAffirm = ConfirmNumVariable(variable, LocalNumbers);
            if (strAffirm.isvariable)
                strAffirm.varDict[variable] = EvaluateStringExpression(Instruction.Skip(2).ToArray(), LocalStrings);
            if (numAffirm.isvariable)
                numAffirm.varDict[variable] = EvaluateExpression(Instruction.Skip(2).ToArray(), LocalNumbers);
            
            if(!strAffirm.isvariable && !numAffirm.isvariable) CrashError("Invalid Syntax, Variable does not exist", Instruction);

        }
    }

    public static (bool isvariable, Dictionary<string, string> varDict) ConfirmStrVariable(string variableName, Dictionary<string, string> localStrings)
    {
        if(GlobalStrings.ContainsKey(variableName)) return (true, GlobalStrings);
        if(localStrings.ContainsKey(variableName)) return (true, localStrings);
        return (false, null);
    }
    
    public static (bool isvariable, Dictionary<string, float> varDict) ConfirmNumVariable(string variableName, Dictionary<string, float> localNumbers)
    {
        if(GlobalNumbers.ContainsKey(variableName)) return (true, GlobalNumbers);
        if(localNumbers.ContainsKey(variableName)) return (true, localNumbers);
        return (false, null);
    }

    public static string GetStringValue(string token, Dictionary<string, string> localStrings)
    {
        var affirm = ConfirmStrVariable(token, localStrings);
        if(affirm.isvariable) return affirm.varDict[token];
        return token;
    }

    public static string EvaluateStringExpression(string[] expression, Dictionary<string, string> localStrings)
    {
        // Console.WriteLine($"$Expression : {String.Join("|", expression)}");
        // if(localStrings.ContainsKey("name")) Console.WriteLine(localStrings["name"]);
            
        switch (expression.Length)
        {
            case 0:
                CrashError("Syntax error, Given String Expression is empty", expression);
                return "";
            case 1: return GetStringValue(expression[0], localStrings);
            case >1:
                if(expression.Length % 2 == 0) CrashError("Invalid Syntax, wrong string expression", expression);
                for (int i = 0; i < expression.Length; i++) 
                    if (i % 2 == 1 && expression[i] !="+") CrashError("Invalid Syntax, wrong string expression", expression);
                
                string result = "";
                for (int i = 0; i < expression.Length; i += 2)
                {
                    result += GetStringValue(expression[i], localStrings);
                }

                return result;
                break;
        }
        return "";
    }

    private static void RunSystemInstruction(string functionName)
    {
        switch (functionName)
        {
            case "write": 
                Console.WriteLine($">> {GlobalStrings["sys-console"]}");
                break;
            case "read":
                var input = Console.ReadLine();
                if (input == null)
                {
                    Console.WriteLine($">> Input not valid");
                    break;
                }
                GlobalStrings["sys-console"] = input;
                break;
            case "toNum":
                float number = 0;
                if (float.TryParse(GlobalStrings["STR-i"], out float result))
                {
                    number = result;
                }

                GlobalNumbers["NUM-i"] = number;
                break;
            case "toString":
                GlobalStrings["STR-i"] = GlobalNumbers["NUM-i"].ToString();
                break;
            case "random":
                GlobalNumbers["NUM-i"] = 0;
                break;
            default:
                CrashError("Invalid Syntax, system function does not exist", new []{"None"});
                break;
            
        }
    }

    public static void CrashError(string ErrorMsg, string[] Instruction)
    {
        Console.WriteLine($"ERROR: {ErrorMsg}\nInstruction Data : {String.Join(' ', Instruction)} [{Instruction.Length}]");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
        Environment.Exit(0);
    }
    
    public static float EvaluateExpression(string[] arguments, Dictionary<string, float> localNumbers)
    {
        List<string> postfix = InfixToPostfix(arguments, localNumbers);
        return EvaluatePostfix(postfix);
    }

    private static readonly Dictionary<string, int> precedence = new Dictionary<string, int>
        {
            { "+", 1 },
            { "-", 1 },
            { "*", 2 },
            { "/", 2 }
        };
    private static List<string> InfixToPostfix(string[] tokens, Dictionary<string, float> localNumbers)
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
                output.Add(GlobalNumbers[token].ToString());
            }
            else if (localNumbers.ContainsKey(token))
            {
                output.Add(localNumbers[token].ToString());
            }
            else
            {
                CrashError($"Invalid Syntax, expression has wrong sequence of operators or invalid variable calls", tokens);
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

    // private static bool IsVariableValid(string variableName, Dictionary<string, float> localNumbers, Dictionary<string, string> localStrings)
    // {
    //     if(variableName.Contains(' ')) return false;
    //     if(variableName.Contains('.')) return false;
    //     foreach (var ch in variableName) if(char.IsDigit(ch)) return false;
    //     
    //     if(localNumbers.ContainsKey(variableName)) return false;
    //     if(localStrings.ContainsKey(variableName)) return false;
    //     
    //     if(GlobalNumbers.ContainsKey(variableName)) return false;
    //     if(GlobalStrings.ContainsKey(variableName)) return false;
    //
    //     return true;
    // }

    private static bool IsVariableFormatValid(string VarName)
    {
        if (VarName.Contains('.')) return false;
        if(VarName.Contains(',')) return false;
        if(VarName.Contains(' ')) return false;
        if(VarName.Contains('?')) return false;
        foreach (var ch in VarName) if(char.IsDigit(ch)) return false;
        return true;
    }


}