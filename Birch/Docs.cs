
public static class Docs
{
    public static string Documentation = @"
=== BIRCH PROGRAMMING LANGUAGE GUIDE v2 ===

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
    Example: str myText = ""Hello World"";
• Arrays - collections of num or str
    Example: num myArray [5];  # Creates myArray_0 through myArray_4 #

== VARIABLE SCOPING ==
GLOBAL SCOPE:
• Declared in a special 'global' function that runs before main
• Accessible from all functions throughout the program
• Persist for the entire program execution
• Required for while loop variables

Global variable declaration:
    .global{
        num globalCounter = 0;
        str globalMessage = ""Welcome"";
    }

LOCAL SCOPE:
• Declared within regular functions
• Only accessible within the declaring function
• Destroyed when function exits
• Cannot be used in while loops

== VARIABLE OPERATIONS ==
Declaration with initialization:
    num variableName = mathematicalExpression;
    str variableName = stringExpression;

Array declaration:
    num arrayName [size];     # Creates arrayName_0, arrayName_1, etc. #
    str arrayName [size];     # Minimum size is 2 #

Assignment (existing variables):
    variableName = newValue;

Variable deletion:
    del variableName;         # Remove variable from memory #
    del $pointerVar;          # Delete via pointer reference #

== PROJECT MANAGEMENT ==
Library importing (use in global function):
    import : libraryName;     # Imports libraryName.br from project folder #
    
Import rules:
• Only works in project mode (not single files)  
• Imported files must be in same directory as Index.br
• Best practice: use import statements in global function
• Runtime will display message if library file not found

== EXPRESSIONS ==
Mathematical: +, -, *, / with full parentheses support
    Example: num result = (x + y) * z / (a - b);
    
String concatenation: str1 + str2 + str3
    Example: str fullName = firstName + "" "" + lastName;

== POINTERS ==
Birch supports indirect variable access using the $ prefix.

Pointer Syntax:
    $pointerVariable - Accesses the variable whose name is stored in pointerVariable

How Pointers Work:
• A pointer variable contains the NAME of another variable as a string
• Using $pointerVar accesses the variable whose name is stored in pointerVar
• Pointers work with both num and str variables
• Pointers respect variable scoping (local vs global)

Pointer Examples:
    str targetName = ""myVariable"";  # Store variable name as string #
    num myVariable = 42;              # Create the target variable #
    num result = $targetName;         # result gets value of myVariable (42) #
    $targetName = 100;                # Sets myVariable to 100 #
    
    # Array access via pointers #
    str arrayIndex = ""myArray_2"";   # Point to array element #
    num value = $arrayIndex;          # Access array element indirectly #
    $arrayIndex = 999;                # Assign to array element #

Advanced Pointer Usage:
    # Dynamic variable selection #
    str choice = ""option1"";
    num option1 = 10;
    num option2 = 20;
    sys-console = $choice;            # Outputs value of option1 #
    $choice = 50;                     # Changes option1 to 50 #

Pointer Limitations:
• Pointer variable must contain a valid, existing variable name
• Cannot create pointer chains (pointers to pointers)
• $ symbol cannot be used in variable names
• Runtime errors occur if pointer references non-existent variables
• Pointer assignments work in both global and local scopes

== CONTROL FLOW ==
IF STATEMENTS:
    if leftValue operator rightValue : trueFunction | falseFunction;
    
    Example: if x < y : handleSmaller | handleLarger;
    Use 'pass' for no action: if x > 0 : doSomething | pass;
    
WHILE LOOPS:
    while leftValue operator rightValue : functionName;
    
    Example: while counter < limit : incrementLoop;
    
    IMPORTANT: While loops can ONLY use global variables!
    Local variables will cause runtime errors in while conditions.

COMPARISON OPERATORS:
    <   (less than)
    >   (greater than) 
    <=  (less than or equal)
    >=  (greater than or equal)
    ==  (equal to - uses floating point tolerance)
    !=  (not equal to - uses floating point tolerance)

Note: Comparisons can only happen between numbers

== FUNCTION CALLS ==
Regular function call:
    call : functionName;
    
System function call:
    syscall : systemFunctionName;

Variable deletion:
    del variableName;         # Delete variable from current scope #
    del $pointerVariable;     # Delete variable via pointer #

Functions cannot return values directly - use global variables for data sharing.
Use 'pass' as a function name to do nothing (useful in conditionals).

== SYSTEM FUNCTIONS (syscalls) ==
• write     - Output sys-console content to screen
• read      - Read user input into sys-console  
• toNum     - Convert STR-i to NUM-i (string to number conversion)
• toString  - Convert NUM-i to STR-i (number to string conversion)
• random    - Use NUM-i and NUM-x as a range, and set NUM-i as a random number
• describe  - Display this help text

== SYSTEM VARIABLES (RESERVED) ==
The following variables are automatically created and used by system functions:
• sys-console - Console I/O buffer (used by read/write)
• STR-i, STR-x, STR-c, STR-l - String scratch variables for syscalls
• NUM-i, NUM-x, NUM-c, NUM-l - Number scratch variables for syscalls

These variables are your interface to system functions:
- Put input values into these variables before syscalls
- Read output values from these variables after syscalls

WARNING: Modifying these variables may cause system functions to behave unexpectedly.
It is strongly recommended to declare your own global variables instead. All System calls
use these variables as arguments.

== ARRAY CREATION ==
**TWO METHODS AVAILABLE:**

Method 1 - Direct Declaration (RECOMMENDED):
    num myNumbers [5];        # Creates myNumbers_0 through myNumbers_4 #
    str myTexts [3];          # Creates myTexts_0 through myTexts_2 #

Method 2 - Legacy syscall method:
    STR-i = ""num"";           # Set array type #
    STR-x = ""myNumbers"";     # Set array name #
    NUM-i = 5;                 # Set array size #
    syscall : arr;             # Create the array #

Array Access:
• Elements are accessed as: arrayName_0, arrayName_1, arrayName_2, etc.
• Array indices start at 0
• No bounds checking - accessing invalid indices may cause errors
• Arrays can be used with pointers for dynamic access

Array Example:
    num scores [3];           # Create array #
    scores_0 = 85;            # Set first element #
    scores_1 = 92;            # Set second element #
    scores_2 = 78;            # Set third element #
    
    str index = ""scores_1"";  # Point to second element #
    num currentScore = $index; # Get value (92) #
    $index = 95;               # Update second element #
    
    del $index;               # Delete the array element #

== VARIABLE NAMING RULES ==
• No dots (.), commas (,), spaces, or question marks (?)
• No dollar signs ($) - reserved for pointer syntax
• No numeric characters allowed anywhere in the name
• Case sensitive (myVar and MyVar are different)
• Must not conflict with reserved keywords or system variables

== MEMORY MANAGEMENT ==
The del statement removes variables from memory:

Basic deletion:
    del variableName;         # Remove variable from current scope #

Pointer-based deletion:
    str target = ""myVar"";
    num myVar = 100;
    del $target;              # Deletes myVar #

Array element deletion:
    str elementName = ""array_5"";
    del $elementName;         # Delete specific array element #

Important notes:
• Deleted variables cannot be accessed afterward
• Attempting to use deleted variables causes runtime errors
• Memory is freed immediately upon deletion
• Works with both local and global variables
• Pointer deletion affects the target variable, not the pointer itself

== COMPLETE PROGRAM EXAMPLE ==
    
.global{
    num secret = 0;
    num guess = 0;
    num attempts = 0;
    num max = 0;
}

.main{
    call : askRange;

    NUM-i = 0;
    NUM-x = max;
    syscall : random;
    secret = NUM-i;

    call : greet;

    while guess != secret : askGuess;

    call : successMessage;
    call : cleanup;
}

.askRange{
    sys-console = ""Enter the maximum number (must be > 1):"";
    syscall : write;

    syscall : read;
    STR-i = sys-console;
    syscall : toNum;
    max = NUM-i;

    if max <= 1 : rangeTooSmall | pass;
}

.rangeTooSmall{
    sys-console = ""Invalid range. Please enter a number greater than 1."";
    syscall : write;
    call : askRange;
}

.greet{
    NUM-i = max;
    syscall : toString;
    str MAXNUM = STR-i;
    
    sys-console = ""Guess the secret number (between 0 and "" + MAXNUM + ""): "";
    syscall : write;
    
    del MAXNUM;               # Clean up temporary variable #
}

.askGuess{
    syscall : read;
    STR-i = sys-console;
    syscall : toNum;
    guess = NUM-i;

    attempts = attempts + 1;

    if guess < secret : tooLow | pass;
    if guess > secret : tooHigh | pass;
}

.tooLow{
    sys-console = ""Too low! Try again:"";
    syscall : write;
}

.tooHigh{
    sys-console = ""Too high! Try again:"";
    syscall : write;
}

.successMessage{
    sys-console = ""Correct! Attempts: "";
    syscall : write;

    NUM-i = attempts;
    syscall : toString;
    sys-console = STR-i;
    syscall : write;
}

.cleanup{
    del secret;
    del guess;
    del attempts;
    del max;
}

== DEBUGGING TIPS ==
• Error messages show the exact instruction that failed
• Check variable names for invalid characters (especially numbers)
• Ensure all variables are declared before use
• Remember while loops require global variables only
• Verify function names exist before calling them
• Check pointer variables contain valid variable names
• Array indices start at 0 and use underscore notation
• Use sys-console for debugging output
• Be careful with del - deleted variables cannot be recovered

== COMMON PATTERNS ==
Input/Output:
    sys-console = ""Your message here"";
    syscall : write;         # Display message
    syscall : read;          # Get user input (stored in sys-console)

Number conversion:
    STR-i = sys-console;     # Put string in STR-i
    syscall : toNum;         # Convert to number in NUM-i
    myVariable = NUM-i;      # Store the number

String conversion:
    NUM-i = myNumber;        # Put number in NUM-i
    syscall : toString;      # Convert to string in STR-i
    sys-console = STR-i;     # Use the string

Conditional execution:
    if condition == 1 : executeTrue | pass;  # Use 'pass' for no action

Loop with global counter:
    while globalCounter < 10 : loopBody;

Dynamic variable access:
    str varName = ""myVariable"";
    num value = $varName;    # Read dynamically
    $varName = newValue;     # Write dynamically

Memory cleanup:
    del temporaryVariable;   # Free memory when done
    del $pointerToVariable;  # Delete via pointer

Project structure:
    .global{
        import : utilities;   # Import utilities.br
        import : gameLogic;   # Import gameLogic.br
        num sharedData = 0;
    }

== PROGRAM EXECUTION FLOW ==
1. Parse and register all functions from source file(s)
2. Initialize system variables (sys-console, STR-i, NUM-i, etc.)
3. Execute 'global' function (if present) to initialize global variables
4. Execute 'main' function to start program execution
5. Functions call other functions as needed using call : functionName
6. Variables can be deleted during execution to free memory
7. Program ends when main function completes successfully

=== END OF GUIDE ===
";
}