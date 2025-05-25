# BIRCH PROGRAMMING LANGUAGE GUIDE

## FILE STRUCTURE
- Files use .br extension
- Projects require Index.br as entry point  
- Single files can be run directly
- Must contain a 'main' function as the program entry point
- Code execution begins from the main function

## BASIC SYNTAX
Functions are defined with:
```
    .functionName{
        instruction1;
        instruction2;
    }
```
Comments use # delimiters:
    ``` # This is a comment # ```
    
All instructions must end with semicolons (;)
Whitespace, tabs, and newlines are ignored during parsing

## DATA TYPES
- num - floating point numbers
    Example: num myNumber = 42.5;
- str - text strings  
    Example: str myText = "Hello World";
- Arrays - collections of num or str (created via syscall : arr)

## VARIABLE SCOPING
### GLOBAL SCOPE:
- Declared in a special 'global' function that runs before main
- Accessible from all functions throughout the program
- Persist for the entire program execution
- Required for while loop variables

Global variable declaration:
```
    .global{
        num globalCounter = 0;
        str globalMessage = Welcome;
    }
```
### LOCAL SCOPE:
- Declared within regular functions
- Only accessible within the declaring function
- Destroyed when function exits
- Cannot be used in while loops

## VARIABLE OPERATIONS
Declaration:
```
    num variableName = mathematicalExpression
    str variableName = stringExpression
```
Assignment (existing variables):
```
    variableName = newValue
```
## EXPRESSIONS
Mathematical: +, -, *, / with full parentheses support
    _Example: num result = (x + y) * z / (a - b);_
    
String concatenation: str1 + str2 + str3
    _Example: str fullName = firstName + ' ' + lastName;_

## POINTERS (Advanced Feature)
Birch supports indirect variable access using the $ prefix.

Pointer Syntax:
    _$pointerVariable - Accesses the variable whose name is stored in pointerVariable_

### How Pointers Work:
- A pointer variable contains the NAME of another variable as a string
- Using $pointerVar accesses the variable whose name is stored in pointerVar
- Pointers work with both num and str variables
- Pointers respect variable scoping (local vs global)

Pointer Examples:
    str targetName = myVariable;     # Store variable name as string #
    num myVariable = 42;             # Create the target variable #
    num result = $targetName;        # result gets value of myVariable (42) #
    $targetName = 100;               # Sets myVariable to 100 #
    
    # Array access via pointers #
    str arrayIndex = myArray_2;      # Point to array element #
    num value = $arrayIndex;         # Access array element indirectly #

### Pointer Limitations:
- Pointer variable must contain a valid, existing variable name
- Cannot create pointer chains (pointers to pointers)
- $ symbol cannot be used in variable names
- Runtime errors occur if pointer references non-existent variables

## CONTROL FLOW
IF STATEMENTS:
    if leftValue operator rightValue : trueFunction | falseFunction
    
    Example: if x < y : handleSmaller | handleLarger;
    
WHILE LOOPS:
    while leftValue operator rightValue : functionName
    
    Example: while counter < limit : incrementLoop;

    IMPORTANT: While loops can ONLY use global variables!
    Local variables will cause runtime errors in while conditions.

### COMPARISON OPERATORS:
    <   (less than)
    >   (greater than) 
    <=  (less than or equal)
    >=  (greater than or equal)
    ==  (equal to)
    !=  (not equal to)

Note : Comparision can only happen between numbers

## FUNCTION CALLS
Regular function call:
    call : functionName
    
System function call:
    syscall : systemFunctionName

Functions cannot return values directly - use global variables for data sharing.

## SYSTEM FUNCTIONS (syscalls)
- write     - Output sys-console content to screen
- read      - Read user input into sys-console  
- toNum     - Convert STR-i to NUM-i (string to number)
- toString  - Convert NUM-i to STR-i (number to string)
- random    - Generate random number and store in NUM-i
- arr       - Create array (type in STR-i, name in STR-x, size in NUM-i)
- describe  - Display this help text

## SYSTEM VARIABLES (RESERVED)
The following variables are used internally by system functions:
- sys-console - Console I/O buffer (used by read/write)
- STR-i, STR-x, STR-c, STR-l - String scratch variables for syscalls
- NUM-i, NUM-x, NUM-c, NUM-l - Number scratch variables for syscalls

**WARNING: Modifying these variables may cause system functions to behave unexpectedly.
It is strongly recommended to declare your own global variables instead. All System calls
use these variables as arguments.**

## ARRAY CREATION
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

## VARIABLE NAMING RULES
- No dots (.), commas (,), spaces, or question marks (?)
- No dollar signs ($) - reserved for pointer syntax
- No numeric characters allowed anywhere in the name
- Case sensitive (myVar and MyVar are different)
- Must not conflict with reserved keywords

## COMPLETE PROGRAM EXAMPLE
```
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
```
## DEBUGGING TIPS
- Error messages show the exact instruction that failed
- Check variable names for invalid characters
- Ensure all variables are declared before use
- Remember while loops require global variables only
- Verify function names exist before calling them
- Check that array indices don't exceed array bounds

## COMMON PATTERNS
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

## PROGRAM EXECUTION FLOW
1. Parse and register all functions
2. Execute 'global' function (if present) to initialize global variables
3. Execute 'main' function to start program
4. Functions call other functions as needed
5. Program ends when main function completes

# Note (Conclusion)
- This project is over, i dont think i plan to add any more features (maybe file importing)
- This project was done completely by me, no AI was used to write any C# code.
- Documentation was AI cuz i am too lazy to do that
- Feel free to look at the code (keep medication in hand, you will need it after reading)
- This is a successor for PINESCRIPT, cuz that was also pretty cool, but very simple
