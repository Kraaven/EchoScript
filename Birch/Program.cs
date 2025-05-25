public static class Birch
{
    private static readonly Dictionary<string, List<string[]>> FunctionLibrary = new();

    private static readonly Dictionary<string, float> GlobalNumbers = new();
    private static readonly Dictionary<string, string> GlobalStrings = new();

    private static readonly HashSet<string> validOps = new() { "<", ">", "<=", ">=", "==", "!=" };

    public static void Main(string[] args)
    {
        #region FileInput

        var projectFolder = "";
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

        var initFilePath = "";
        var SingleFile = false;

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

            CrashError("Please provide a valid Birch file or a Directory with Index.br",
                new[] { "No file loaded for Instruction" });
        }

        var IndexFile = File.ReadAllText(initFilePath);

        #endregion

        #region Function Registration

        IndexFile = IndexFile.Replace("\n", "")
            .Replace("\t", "")
            .Replace("\r", "")
            .Replace("    ", "")
            .Replace("      ", "");

        {
            var tempBlock = "";
            var functionName = "";
            var isComment = false;
            var blockStarted = false;
            var InstructionLines = new List<string>();

            // Console.WriteLine(IndexFile);

            for (var i = 0; i < IndexFile.Length; i++)
            {
                var currentChar = IndexFile[i];

                if (currentChar == '#')
                {
                    isComment = !isComment;
                    continue;
                }

                if (isComment) continue;

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

                        RegisterFunction(functionName, InstructionLines);

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
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
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
        
        Console.WriteLine("Code Execution Completed Successfully\nPress any key to exit...");
        Console.ReadKey();
    }


    public static void RegisterFunction(string functionName, List<string> functionBlock)
    {
        // Console.WriteLine($"Function Name: {functionName}");
        var index = 0;

        List<string[]> FunctionInstructions = new();

        foreach (var instruction in functionBlock)
        {
            if (string.IsNullOrWhiteSpace(instruction)) continue;

            var SplitInstruction = new List<string>();
            var IsString = false;
            var token = "";

            for (var i = 0; i < instruction.Length; i++)
            {
                var c = instruction[i];
                if (c == '"' || c == '\'')
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

            if (token.Length > 0) SplitInstruction.Add(token);

            // Console.WriteLine($"[{index}] : ({string.Join(",", SplitInstruction)})");
            FunctionInstructions.Add(SplitInstruction.ToArray());
            index++;
        }

        if (functionName == "global")
            RunGlobalFunction(FunctionInstructions);
        else FunctionLibrary.Add(functionName, FunctionInstructions);
    }

    private static void RunGlobalFunction(List<string[]> functionInstructions)
    {
        foreach (var Instruction in functionInstructions)
        {
            //Variable Declaration
            switch (Instruction[0])
            {
                case "num":
                    if (Instruction.Length < 4) CrashError("Invalid Syntax, Instruction is incomplete", Instruction);
                    if (!IsVariableFormatValid(Instruction[1]))
                        CrashError("Invalid Syntax, Not a valid Variable Name", Instruction);
                    if (Instruction[2] != "=") CrashError("Invalid Syntax, no assigment operator", Instruction);

                    if (ConfirmNumVariable(Instruction[1], GlobalNumbers, GlobalStrings).isvariable)
                        CrashError("Invalid Syntax, variable already defined", Instruction);
                    else
                        GlobalNumbers.Add(Instruction[1],
                            EvaluateExpression(Instruction.Skip(3).ToArray(), GlobalNumbers, GlobalStrings));

                    continue;
                case "str":
                    if (Instruction.Length < 4) CrashError("Invalid Syntax, Instruction is incomplete", Instruction);
                    if (!IsVariableFormatValid(Instruction[1]))
                        CrashError("Invalid Syntax, Not a valid Variable Name", Instruction);
                    if (Instruction[2] != "=") CrashError("Invalid Syntax, no assigment operator", Instruction);

                    var affirm = ConfirmStrVariable(Instruction[0], GlobalStrings, GlobalNumbers);
                    if (affirm.isvariable) CrashError("Invalid Syntax, variable already defined", Instruction);
                    else
                        GlobalStrings.Add(Instruction[1],
                            EvaluateStringExpression(Instruction.Skip(3).ToArray(), GlobalStrings, GlobalNumbers));

                    continue;
            }

            //Variable Re-assignment
            if (Instruction.Length < 3) CrashError("Invalid Syntax, instruction is incomplete", Instruction);

            var variable = Instruction[0];
            var operation = Instruction[1];

            if (operation != "=")
            {
                CrashError("Invalid Syntax, instruction is nonsense", Instruction);
                return;
            }

            var strAffirm = ConfirmStrVariable(variable, GlobalStrings, GlobalNumbers);
            var numAffirm = ConfirmNumVariable(variable, GlobalNumbers, GlobalStrings);
            if (strAffirm.isvariable)
                strAffirm.varDict[variable] =
                    EvaluateStringExpression(Instruction.Skip(2).ToArray(), GlobalStrings, GlobalNumbers);
            if (numAffirm.isvariable)
                numAffirm.varDict[variable] =
                    EvaluateExpression(Instruction.Skip(2).ToArray(), GlobalNumbers, GlobalStrings);

            if (!strAffirm.isvariable && !numAffirm.isvariable)
                CrashError("Invalid Syntax, Variable does not exist", Instruction);
        }
    }

    public static void CallFunction(string functionName)
    {
        if (!FunctionLibrary.ContainsKey(functionName))
            CrashError($"Function {functionName} does not exist", new[] { "None" });
        Dictionary<string, float> LocalNumbers = new();
        Dictionary<string, string> LocalStrings = new();

        foreach (var Instruction in FunctionLibrary[functionName])
        {
            //Conditionals
            {
                float GetOperandValue(string token)
                {
                    if (float.TryParse(token, out var val)) return val;
                    var confirm = ConfirmNumVariable(token, LocalNumbers, LocalStrings);
                    if (!confirm.isvariable) CrashError("Invalid Operand, variable does not exist", Instruction);
                    return confirm.varDict[token];
                }

                float GetGlobalOperandValue(string token)
                {
                    if (float.TryParse(token, out var val)) return val;
                    var confirm = ConfirmNumVariable(token, LocalNumbers, LocalStrings);
                    if (!confirm.isvariable) CrashError("Invalid Operand, variable does not exist", Instruction);
                    if (confirm.varDict.Equals(LocalNumbers))
                        CrashError("Invalid Syntax, While loops can only use global variables", Instruction);
                    return confirm.varDict[token];
                }

                void CallIfValid(string func)
                {
                    if (func == "pass") return;
                    if (!FunctionLibrary.ContainsKey(func)) CrashError("Function does not exist", Instruction);
                    CallFunction(func);
                }

                switch (Instruction[0])
                {
                    case "if":
                        if (Instruction is not ["if", _, _, _, ":", _, "|", _])
                            CrashError("Invalid Syntax, if condition must follow 'if v1 op v2 : func | func'",
                                Instruction);

                        var IF_leftOperand = Instruction[1];
                        var IF_op = Instruction[2];
                        var IF_rightOperand = Instruction[3];

                        var trueFunction = Instruction[5];
                        var falseFunction = Instruction[7];

                        if (!validOps.Contains(IF_op)) CrashError("Invalid operator in if condition", Instruction);
                        var IF_left = GetOperandValue(IF_leftOperand);
                        var IF_right = GetOperandValue(IF_rightOperand);


                        var IF_condition = IF_op switch
                        {
                            "<" => IF_left < IF_right,
                            ">" => IF_left > IF_right,
                            "<=" => IF_left <= IF_right,
                            ">=" => IF_left >= IF_right,
                            "==" => Math.Abs(IF_left - IF_right) < 0.001f,
                            "!=" => Math.Abs(IF_left - IF_right) > 0.001f,
                            _ => false
                        };

                        CallIfValid(IF_condition ? trueFunction : falseFunction);
                        continue;


                    case "while":
                        if (Instruction is not ["while", _, _, _, ":", _])
                            CrashError("Invalid Syntax, while conditional must follow 'while v op v : func'",
                                Instruction);

                        var WHILE_leftOperand = Instruction[1];
                        var WHILE_op = Instruction[2];
                        var WHILE_rightOperand = Instruction[3];

                        var WHILE_CALLFUNCTION = Instruction[5];

                        if (!validOps.Contains(WHILE_op))
                            CrashError("Invalid operator in while conditional", Instruction);
                        var WHILE_left = GetGlobalOperandValue(WHILE_leftOperand);
                        var WHILE_right = GetGlobalOperandValue(WHILE_rightOperand);


                        var WHILE_condition = WHILE_op switch
                        {
                            "<" => WHILE_left < WHILE_right,
                            ">" => WHILE_left > WHILE_right,
                            "<=" => WHILE_left <= WHILE_right,
                            ">=" => WHILE_left >= WHILE_right,
                            "==" => Math.Abs(WHILE_left - WHILE_right) < 0.001f,
                            "!=" => Math.Abs(WHILE_left - WHILE_right) > 0.001f,
                            _ => false
                        };

                        while (WHILE_condition)
                        {
                            CallFunction(WHILE_CALLFUNCTION);

                            WHILE_left = GetGlobalOperandValue(WHILE_leftOperand);
                            WHILE_right = GetGlobalOperandValue(WHILE_rightOperand);

                            WHILE_condition = WHILE_op switch
                            {
                                "<" => WHILE_left < WHILE_right,
                                ">" => WHILE_left > WHILE_right,
                                "<=" => WHILE_left <= WHILE_right,
                                ">=" => WHILE_left >= WHILE_right,
                                "==" => Math.Abs(WHILE_left - WHILE_right) < 0.001f,
                                "!=" => Math.Abs(WHILE_left - WHILE_right) > 0.001f,
                                _ => false
                            };
                        }


                        continue;
                }
            }


            //Function Calling
            switch (Instruction[0])
            {
                case "call":
                    if (Instruction.Length < 3 || Instruction.Length > 3)
                        CrashError("Invalid Syntax, invalid number of arguments", Instruction);
                    if (Instruction[1] != ":") CrashError("Invalid Syntax, No : Operator", Instruction);

                    // Console.WriteLine($"Calling function {Instruction[2]}");
                    CallFunction(Instruction[2]);
                    continue;
                case "syscall":
                    if (Instruction.Length < 3 || Instruction.Length > 3)
                        CrashError("Invalid Syntax, invalid number of arguments", Instruction);
                    if (Instruction[1] != ":") CrashError("Invalid Syntax, No : Operator", Instruction);

                    RunSystemInstruction(Instruction[2]);

                    continue;
            }

            //Variable Declaration
            switch (Instruction[0])
            {
                case "num":
                    if (Instruction.Length < 4) CrashError("Invalid Syntax, Instruction is incomplete", Instruction);
                    if (!IsVariableFormatValid(Instruction[1]))
                        CrashError("Invalid Syntax, Not a valid Variable Name", Instruction);
                    if (Instruction[2] != "=") CrashError("Invalid Syntax, no assigment operator", Instruction);

                    if (ConfirmNumVariable(Instruction[1], LocalNumbers, LocalStrings).isvariable)
                        CrashError("Invalid Syntax, variable already defined", Instruction);
                    else
                        LocalNumbers.Add(Instruction[1],
                            EvaluateExpression(Instruction.Skip(3).ToArray(), LocalNumbers, LocalStrings));

                    continue;
                case "str":
                    if (Instruction.Length < 4) CrashError("Invalid Syntax, Instruction is incomplete", Instruction);
                    if (!IsVariableFormatValid(Instruction[1]))
                        CrashError("Invalid Syntax, Not a valid Variable Name", Instruction);
                    if (Instruction[2] != "=") CrashError("Invalid Syntax, no assigment operator", Instruction);

                    var affirm = ConfirmStrVariable(Instruction[0], LocalStrings, LocalNumbers);
                    if (affirm.isvariable) CrashError("Invalid Syntax, variable already defined", Instruction);
                    else
                        LocalStrings.Add(Instruction[1],
                            EvaluateStringExpression(Instruction.Skip(3).ToArray(), LocalStrings, LocalNumbers));

                    continue;
            }

            //Variable Re-assignment
            if (Instruction.Length < 3) CrashError("Invalid Syntax, instruction is incomplete", Instruction);

            var variable = Instruction[0];
            var operation = Instruction[1];

            if (operation != "=")
            {
                CrashError("Invalid Syntax, instruction is nonsense", Instruction);
                return;
            }

            var strAffirm = ConfirmStrVariable(variable, LocalStrings, LocalNumbers);
            var numAffirm = ConfirmNumVariable(variable, LocalNumbers, LocalStrings);
            if (strAffirm.isvariable)
                strAffirm.varDict[variable] =
                    EvaluateStringExpression(Instruction.Skip(2).ToArray(), LocalStrings, LocalNumbers);
            if (numAffirm.isvariable)
                numAffirm.varDict[variable] =
                    EvaluateExpression(Instruction.Skip(2).ToArray(), LocalNumbers, LocalStrings);

            if (!strAffirm.isvariable && !numAffirm.isvariable)
                CrashError("Invalid Syntax, Variable does not exist", Instruction);
        }
    }

    public static (bool isvariable, Dictionary<string, string> varDict) ConfirmStrVariable(string variableName,
        Dictionary<string, string> localStrings, Dictionary<string, float> localNumbers)
    {
        if (variableName.StartsWith("$") && variableName.Length > 1)
        {
            var pointerVarName = variableName.Substring(1);
            var STRaffirm = ConfirmStrVariable(pointerVarName, localStrings, localNumbers);
            if (STRaffirm.isvariable) return (true, STRaffirm.varDict);
            else return (false, null);
        }

        if (GlobalStrings.ContainsKey(variableName)) return (true, GlobalStrings);
        if (localStrings.ContainsKey(variableName)) return (true, localStrings);
        return (false, null);
    }

    public static (bool isvariable, Dictionary<string, float> varDict) ConfirmNumVariable(string variableName,
        Dictionary<string, float> localNumbers, Dictionary<string, string> localStrings)
    {
        if (variableName.StartsWith("$") && variableName.Length > 1)
        {
            var pointerVarName = variableName.Substring(1);

            var Paffirm = ConfirmStrVariable(pointerVarName, localStrings, localNumbers);

            if (Paffirm.isvariable)
            {
                var NumAffirm = ConfirmNumVariable(Paffirm.varDict[pointerVarName], localNumbers, localStrings);
                if (NumAffirm.isvariable) return (true, NumAffirm.varDict);
                else CrashError("Invalid Syntax, pointer does not point to a num variable", [variableName]);
            }
            else
            {
                return (false, null);
            }
        }

        if (GlobalNumbers.ContainsKey(variableName)) return (true, GlobalNumbers);
        if (localNumbers.ContainsKey(variableName)) return (true, localNumbers);
        return (false, null);
    }

    public static string GetStringValue(string token, Dictionary<string, string> localStrings,
        Dictionary<string, float> localNumbers)
    {
        if (token.StartsWith("$") && token.Length > 1)
        {
            var Paffirm = ConfirmStrVariable(token, localStrings, localNumbers);
            if (!Paffirm.isvariable) CrashError("Syntax Error, pointer does not route a valid variable", [token]);
            var pointerVarName = token.Substring(1);
            var HiddenAffirm = ConfirmStrVariable(pointerVarName, localStrings, localNumbers);
            return HiddenAffirm.varDict[pointerVarName];
        }

        var affirm = ConfirmStrVariable(token, localStrings, localNumbers);
        if (affirm.isvariable) return affirm.varDict[token];
        return token;
    }

    public static string EvaluateStringExpression(string[] expression, Dictionary<string, string> localStrings,
        Dictionary<string, float> localNumbers)
    {
        // Console.WriteLine($"$Expression : {String.Join("|", expression)}");
        // if(localStrings.ContainsKey("name")) Console.WriteLine(localStrings["name"]);

        switch (expression.Length)
        {
            case 0:
                CrashError("Syntax error, Given String Expression is empty", expression);
                return "";
            case 1: return GetStringValue(expression[0], localStrings, localNumbers);
            case > 1:
                if (expression.Length % 2 == 0) CrashError("Invalid Syntax, wrong string expression", expression);
                for (var i = 0; i < expression.Length; i++)
                    if (i % 2 == 1 && expression[i] != "+")
                        CrashError("Invalid Syntax, wrong string expression", expression);

                var result = "";
                for (var i = 0; i < expression.Length; i += 2)
                    result += GetStringValue(expression[i], localStrings, localNumbers);

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
                    Console.WriteLine(">> Input not valid");
                    break;
                }

                GlobalStrings["sys-console"] = input;
                break;
            case "toNum":
                float number = 0;
                if (float.TryParse(GlobalStrings["STR-i"], out var result)) number = result;

                GlobalNumbers["NUM-i"] = number;
                break;
            case "toString":
                GlobalStrings["STR-i"] = GlobalNumbers["NUM-i"].ToString();
                break;
            case "random":
                GlobalNumbers["NUM-i"] = 0;
                break;
            case "arr":
                if (!GlobalStrings["STR-i"].Equals("num") && !GlobalStrings["STR-i"].Equals("str"))
                    CrashError("Invalid Syntax, not a valid variable type", ["arr", GlobalStrings["STR-i"]]);
                if (GlobalStrings["STR-x"].Equals("") || !IsVariableFormatValid(GlobalStrings["STR-x"]))
                    CrashError("Invalid Syntax, not a valid variable name", ["arr", GlobalStrings["STR-x"]]);
                if (GlobalNumbers["NUM-i"] is 0 or 1)
                    CrashError("Invalid Syntax, arrays cannot be declared with 0 or 1 elements",
                        ["arr", GlobalNumbers["Num-i"].ToString()]);

                if (GlobalStrings["STR-i"].Equals("str"))
                    for (var i = 0; i < GlobalNumbers["NUM-i"]; i++)
                        GlobalStrings.Add($"{GlobalStrings["STR-x"]}_{i}", "");
                else
                    for (var i = 0; i < GlobalNumbers["NUM-i"]; i++)
                        GlobalNumbers.Add($"{GlobalStrings["STR-x"]}_{i}", 0);

                break;


            case "describe":
                Console.WriteLine(@"
=== BIRCH PROGRAMMING LANGUAGE GUIDE ===

== FILE STRUCTURE ==
• Files use .br extension
• Projects require Index.br as entry point  
• Single files can be run directly
• Must contain a 'main' function as the program entry point
• Code execution begins from the main function

== BASIC SYNTAX ==
Functions are defined with:
    .functionName{
        instruction1;
        instruction2;
    }

Comments use # delimiters:
    # This is a comment #
    
All instructions must end with semicolons (;)
Whitespace, tabs, and newlines are ignored during parsing

== DATA TYPES ==
• num - floating point numbers
    Example: num myNumber = 42.5;
• str - text strings  
    Example: str myText = 'Hello World';
• Arrays - collections of num or str (created via syscall : arr)

== VARIABLE SCOPING ==
GLOBAL SCOPE:
• Declared in a special 'global' function that runs before main
• Accessible from all functions throughout the program
• Persist for the entire program execution
• Required for while loop variables

Global variable declaration:
    .global{
        num globalCounter = 0;
        str globalMessage = Welcome;
    }

LOCAL SCOPE:
• Declared within regular functions
• Only accessible within the declaring function
• Destroyed when function exits
• Cannot be used in while loops

== VARIABLE OPERATIONS ==
Declaration:
    num variableName = mathematicalExpression
    str variableName = stringExpression

Assignment (existing variables):
    variableName = newValue

== EXPRESSIONS ==
Mathematical: +, -, *, / with full parentheses support
    Example: num result = (x + y) * z / (a - b);
    
String concatenation: str1 + str2 + str3
    Example: str fullName = firstName + ' ' + lastName;

== POINTERS (Advanced Feature) ==
Birch supports indirect variable access using the $ prefix.

Pointer Syntax:
    $pointerVariable - Accesses the variable whose name is stored in pointerVariable

How Pointers Work:
• A pointer variable contains the NAME of another variable as a string
• Using $pointerVar accesses the variable whose name is stored in pointerVar
• Pointers work with both num and str variables
• Pointers respect variable scoping (local vs global)

Pointer Examples:
    str targetName = myVariable;     # Store variable name as string #
    num myVariable = 42;             # Create the target variable #
    num result = $targetName;        # result gets value of myVariable (42) #
    $targetName = 100;               # Sets myVariable to 100 #
    
    # Array access via pointers #
    str arrayIndex = myArray_2;      # Point to array element #
    num value = $arrayIndex;         # Access array element indirectly #

Pointer Limitations:
• Pointer variable must contain a valid, existing variable name
• Cannot create pointer chains (pointers to pointers)
• $ symbol cannot be used in variable names
• Runtime errors occur if pointer references non-existent variables

== CONTROL FLOW ==
IF STATEMENTS:
    if leftValue operator rightValue : trueFunction | falseFunction
    
    Example: if x < y : handleSmaller | handleLarger;
    
WHILE LOOPS:
    while leftValue operator rightValue : functionName
    
    Example: while counter < limit : incrementLoop;
    
    IMPORTANT: While loops can ONLY use global variables!
    Local variables will cause runtime errors in while conditions.

COMPARISON OPERATORS:
    <   (less than)
    >   (greater than) 
    <=  (less than or equal)
    >=  (greater than or equal)
    ==  (equal to)
    !=  (not equal to)

Note : Comparision can only happen between numbers

== FUNCTION CALLS ==
Regular function call:
    call : functionName
    
System function call:
    syscall : systemFunctionName

Functions cannot return values directly - use global variables for data sharing.

== SYSTEM FUNCTIONS (syscalls) ==
• write     - Output sys-console content to screen
• read      - Read user input into sys-console  
• toNum     - Convert STR-i to NUM-i (string to number)
• toString  - Convert NUM-i to STR-i (number to string)
• random    - Generate random number and store in NUM-i
• arr       - Create array (type in STR-i, name in STR-x, size in NUM-i)
• describe  - Display this help text

== SYSTEM VARIABLES (RESERVED) ==
The following variables are used internally by system functions:
• sys-console - Console I/O buffer (used by read/write)
• STR-i, STR-x, STR-c, STR-l - String scratch variables for syscalls
• NUM-i, NUM-x, NUM-c, NUM-l - Number scratch variables for syscalls

WARNING: Modifying these variables may cause system functions to behave unexpectedly.
It is strongly recommended to declare your own global variables instead. All System calls
use these variables as arguments.

== ARRAY CREATION ==
Arrays must be created using the syscall system:

Steps to create an array:
1. Set STR-i to 'num' or 'str' (array type)
2. Set STR-x to desired array name
3. Set NUM-i to array size (minimum 2 elements)
4. Execute: syscall : arr

Array elements are accessed as: arrayName_0, arrayName_1, arrayName_2, etc.

Example array creation:
    STR-i = num;           # Set array type #
    STR-x = myNumbers;     # Set array name #
    NUM-i = 5;             # Set array size #
    syscall : arr;         # Create the array #
    
    # Now you can use: myNumbers_0, myNumbers_1, myNumbers_2, myNumbers_3, myNumbers_4 #

== VARIABLE NAMING RULES ==
• No dots (.), commas (,), spaces, or question marks (?)
• No dollar signs ($) - reserved for pointer syntax
• No numeric characters allowed anywhere in the name
• Case sensitive (myVar and MyVar are different)
• Must not conflict with reserved keywords

== COMPLETE PROGRAM EXAMPLE ==
    .global{
        num counter = 0;
        str appName = 'Birch Calculator';
    }
    
    .main{
        sys-console = 'appName';
        syscall : write;
        
        call : getUserInput;
        call : processInput;
        call : showResult;
    }
    
    .getUserInput{
        sys-console = 'Enter a number: ';
        syscall : write;
        syscall : read;
        STR-i = sys-console;
        syscall : toNum;
        counter = NUM-i;
    }
    
    .processInput{
        counter = counter * 2;
    }
    
    .showResult{
        NUM-i = counter;
        syscall : toString;
        sys-console = 'Result: ' + STR-i;
        syscall : write;
    }

== DEBUGGING TIPS ==
• Error messages show the exact instruction that failed
• Check variable names for invalid characters
• Ensure all variables are declared before use
• Remember while loops require global variables only
• Verify function names exist before calling them
• Check that array indices don't exceed array bounds

== COMMON PATTERNS ==
Input/Output:
    syscall : read;          # Get user input
    # Process sys-console
    syscall : write;         # Display output

Number conversion:
    STR-i = 'someString';
    syscall : toNum;
    # NUM-i now contains the numeric value

Conditional execution:
    if condition == 1 : executeTrue | pass;  # Use 'pass' for no action

Loop with global counter:
    while globalCounter < 10 : loopBody;

== PROGRAM EXECUTION FLOW ==
1. Parse and register all functions
2. Execute 'global' function (if present) to initialize global variables
3. Execute 'main' function to start program
4. Functions call other functions as needed
5. Program ends when main function completes

=== END OF GUIDE ===
");

                break;
            default:
                CrashError("Invalid Syntax, system function does not exist", ["None"]);
                break;
        }
    }

    public static void CrashError(string ErrorMsg, string[] Instruction)
    {
        Console.WriteLine(
            $"ERROR: {ErrorMsg}\nInstruction Data : {string.Join(' ', Instruction)} [{Instruction.Length}]");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
        Environment.Exit(0);
    }

    public static float EvaluateExpression(string[] arguments, Dictionary<string, float> localNumbers,
        Dictionary<string, string> localStrings)
    {
        var postfix = InfixToPostfix(arguments, localNumbers, localStrings);
        return EvaluatePostfix(postfix);
    }

    private static readonly Dictionary<string, int> precedence = new()
    {
        { "+", 1 },
        { "-", 1 },
        { "*", 2 },
        { "/", 2 }
    };

    private static List<string> InfixToPostfix(string[] tokens, Dictionary<string, float> localNumbers,
        Dictionary<string, string> localStrings)
    {
        var output = new List<string>();
        var operators = new Stack<string>();

        foreach (var token in tokens)
            if (float.TryParse(token, out _))
            {
                output.Add(token);
            }
            else if (precedence.ContainsKey(token))
            {
                while (operators.Count > 0 && precedence.ContainsKey(operators.Peek()) &&
                       precedence[operators.Peek()] >= precedence[token])
                    output.Add(operators.Pop());
                operators.Push(token);
            }
            else if (token == "(")
            {
                operators.Push(token);
            }
            else if (token == ")")
            {
                while (operators.Count > 0 && operators.Peek() != "(") output.Add(operators.Pop());
                if (operators.Count == 0 || operators.Pop() != "(")
                    throw new ArgumentException("Mismatched parentheses");
            }

            else if (token.StartsWith('$') && token.Length > 1)
            {
                var Paffirm = ConfirmNumVariable(token, localNumbers, localStrings);
                if (Paffirm.isvariable)
                {
                    var strAffirm = ConfirmStrVariable(token.Substring(1), localStrings, localNumbers);
                    output.Add(Paffirm.varDict[strAffirm.varDict[token.Substring(1)]].ToString());
                }
                else
                {
                    CrashError("Invalid Syntax, Variable does not point to a number", [token]);
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
                CrashError("Invalid Syntax, expression has wrong sequence of operators or invalid variable calls",
                    tokens);
            }

        while (operators.Count > 0)
        {
            var op = operators.Pop();
            if (op == "(" || op == ")") throw new ArgumentException("Mismatched parentheses");
            output.Add(op);
        }

        return output;
    }

    private static float EvaluatePostfix(List<string> postfix)
    {
        var stack = new Stack<float>();

        foreach (var token in postfix)
            if (float.TryParse(token, out var num))
            {
                stack.Push(num);
            }
            else
            {
                var right = stack.Pop();
                var left = stack.Pop();
                switch (token)
                {
                    case "+": stack.Push(left + right); break;
                    case "-": stack.Push(left - right); break;
                    case "*": stack.Push(left * right); break;
                    case "/": stack.Push(left / right); break;
                    default: throw new InvalidOperationException($"Invalid operator {token}");
                }
            }

        return stack.Pop();
    }

    private static bool IsVariableFormatValid(string VarName)
    {
        if (VarName.Contains('.')) return false;
        if (VarName.Contains(',')) return false;
        if (VarName.Contains(' ')) return false;
        if (VarName.Contains('?')) return false;
        if (VarName.Contains('$')) return false;
        foreach (var ch in VarName)
            if (char.IsDigit(ch))
                return false;
        return true;
    }
}