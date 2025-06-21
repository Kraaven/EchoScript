# Birch Programming Language

![Birch Logo](https://img.shields.io/badge/Birch-v2.0-green) ![Platform](https://img.shields.io/badge/platform-cross--platform-lightgrey)

Birch is a unique, minimalist programming language designed for simplicity and educational purposes. With its distinctive syntax and innovative features like pointer-based variable access, Birch offers a fresh approach to programming that emphasizes clarity and explicit control flow.

## Table of Contents

-   [Quick Start]( #quick-start)
-   [Installation](#installation)
-   [Language Overview](#language-overview)
-   [File Structure](#file-structure)
-   [Basic Syntax](#basic-syntax)
-   [Data Types](#data-types)
-   [Variable Scoping](#variable-scoping)
-   [Variable Operations](#variable-operations)
-   [Pointers](#pointers)
-   [Control Flow](#control-flow)
-   [Functions](#functions)
-   [System Functions](#system-functions)
-   [System Variables](#system-variables)
-   [Arrays](#arrays)
-   [Project Management](#project-management)
-   [Memory Management](#memory-management)
-   [Variable Naming Rules](#variable-naming-rules)
-   [Common Patterns](#common-patterns)
-   [Program Execution Flow](#program-execution-flow)
- [Conclusion](#conclusion)

## Quick Start

Get started with Birch in minutes:

1.  **Download**: Get the latest release from the [releases page]()
2.  **Extract**: Unzip the executable interpreter
3.  **Create**: Write your first Birch program
4.  **Run**: Execute using the Birch interpreter
5.  **Optional**: Install the VsCode extension file also provided in the release page for convenient syntax highlighting

### Your First Program

Create a file called `hello.br`:

```birch
.main{
    sys-console = "Hello, World!";
    syscall : write;
}

```

Run with: `birch hello.br`

## Installation

### Download and Setup

1.  Navigate to the [releases page](https://claude.ai/releases)
2.  Download the appropriate executable for your platform
3.  Extract the archive to your desired location
4.  Add the executable to your system PATH (optional but recommended)

### System Requirements

-   **Operating System**: Windows or Linux
-   **Dependencies**: None - Birch is a standalone executable
-   **Memory**: Minimal RAM requirements
-   **Storage**: About 70MB disk space

## Language Overview

Birch is characterized by several unique design decisions:

### Key Features

-   **Explicit Syntax**: Every instruction must end with a semicolon
-   **Function-Based**: All code is organized into functions
-   **Pointer System**: Indirect variable access using string-based pointers
-   **Global/Local Scoping**: Clear distinction between global and local variables
-   **System Integration**: Built-in system functions for I/O and utility operations
-   **Memory Management**: Explicit variable deletion with the `del` statement
-   **Project Support**: Import system for multi-file projects

### Design Philosophy

Birch prioritizes:

-   **Explicitness** over implicit behavior
-   **Simplicity** over feature richness
-   **Functional** over object orientated
-   **Clear semantics** over syntactic sugar

## File Structure

### File Extensions

All Birch source files use the `.br` extension:

-   `Index.br` - Project entry point
-   `program.br` - Single file programs
-   `utilities.br` - Library files

### Project Structure

For multi-file projects:

```
project/
├── Index.br          # Entry point (required)
├── utilities.br      # Library file
├── gameLogic.br      # Another library file
└── data.br           # Another Another Library file

```

### Entry Points

**Single File Programs:**

-   Can be run directly
-   Must contain a `main` function
-   No imports allowed

**Project Programs:**

-   Require `Index.br` as the entry point
-   Support importing other `.br` files
-   All files must be in the same directory

## Basic Syntax

### Function Definition

Functions are the primary code organization unit in Birch:

```birch
.functionName{
    instruction1;
    instruction2;
    instruction3;
}

```

### Comments

Comments use hash delimiters:

```birch
# This is a comment #
# Comments can span multiple words and spaces #

```

### Instructions

-   All instructions must end with semicolons (`;`)
-   Whitespace, tabs, and newlines are ignored during parsing
-   Instructions are executed sequentially within functions
-   Spaces are allowed inside strings within " "

### Code Style

Birch is flexible with formatting:

```birch
# These are equivalent #
.myFunction{num x = 5;num y = 10;num z = x + y;}

.myFunction{
    num x = 5;
    num y = 10;
    num z = x + y;
}

```

## Data Types

Birch supports two primary data types plus arrays:

### Numbers (`num`)

Floating-point numbers for all numeric operations:

```birch
num myNumber = 42.5;
num temperature = -10.0;
num result = 0;

```

**Characteristics:**

-   All numbers are floating-point
-   Support positive and negative values
-   Used for mathematical calculations
-   Variable must be initialised

### Strings (`str`)

Text data enclosed in double quotes:

```birch
str myText = "Hello World";
str firstName = "John";
str message = " ";  # Empty string #

```

**Characteristics:**

-   Must be enclosed in double quotes (`"`)
-   Support concatenation with `+` operator
-   Can contain spaces and special characters
-   Empty strings are valid

### Arrays

Collections of numbers or strings:

```birch
num myNumbers [5];    # Creates myNumbers_0 through myNumbers_4
str myTexts [3];      # Creates myTexts_0 through myTexts_2

```

**Array Details:**

-   Elements are accessed with underscore notation: `arrayName_index`
-   Indices start at 0
-   Minimum array size is 2 
-   No bounds checking at runtime

## Variable Scoping

Birch has a strict two-tier scoping system:

### Global Scope

Variables declared in the special `global` function:

```birch
.global{
    num globalCounter = 0;
    str globalMessage = "Welcome";
    num gameScore = 100;
}

```

**Global Variable Properties:**

-   Accessible from all functions
-   Persist for entire program execution
-   **Required** for while loop variables
-   Initialized before `main` function runs
-   Can be modified by any function

### Local Scope

Variables declared within regular functions:

```birch
.myFunction{
    num localVar = 42;
    str localText = "temporary";
    # These variables only exist within myFunction #
}

```

**Local Variable Properties:**

-   Only accessible within the declaring function
-   Destroyed automatically when function exits
-   **Cannot** be used in while loops
-   Faster access than global variables
-   Isolated from other functions (even recursion)

### Scoping Rules

**Critical Limitation:**

```birch
# THIS WILL CAUSE A RUNTIME ERROR #
.badExample{
    num counter = 0;
}

.badExample2{
	counter = counter + 1;
}

# CORRECT APPROACH #
.global{
    num counter = 0;
}

.goodExample{
    counter = counter + 1;
    # Works as counter is global #
}

```

## Variable Operations

### Declaration and Initialization

**Numbers:**

```birch
num variableName = mathematicalExpression;
num score = 100;
num average = (total + bonus) / count;

```

**Strings:**

```birch
str variableName = stringExpression;
str greeting = "Hello";
str fullName = firstName + " " + lastName;

```

**Arrays:**

```birch
num arrayName [5];     # Creates indexed elements #
str textArray [10];     # Minimum size is 2 #

```

Array can be decalared with a number, variable or pointer.

```
STR-i = "num";
STR-x = "Scores_array";
NUM-i = size_variable;
syscall : arr;
```

### Assignment

For existing variables:

```birch
variableName = newValue;

# Examples #
score = 200;
message = "Game Over";
array_0 = 42;

```

### Mathematical Expressions

Full arithmetic support with parentheses:

```birch
num result = (x + y) * z / (a - b);
num compound = ((a + b) * c) - (d / e);
num simple = x + y - z;

```

**Supported Operators:**

-   `+` Addition
-   `-` Subtraction
-   `*` Multiplication
-   `/` Division
-   `()` Parentheses for grouping

### String Concatenation

Combine strings with the `+` operator:

```birch
str fullName = firstName + " " + lastName;
str message = "Score: " + scoreText + " points";
str path = directory + "/" + filename + ".txt";
str arrVal = arrName + "_" + IndexNumConvertedToString;
```

## Pointers

One of Birch's most distinctive features is its pointer system, which provides indirect variable access. It is somewhat inspired by PHP.

### Pointer Concept

Pointers work by storing variable names as strings, then using the `$` prefix to access the referenced variable:

```birch
str pointerVariable = "targetVariable";  # Store variable name #
num targetVariable = 42;                 # Create target variable #
num value = $pointerVariable;            # Access via pointer -> 42 #

```

### Basic Pointer Usage

**Reading Values:**

```birch
str targetName = "myVariable";
num myVariable = 100;
num result = $targetName;        # result gets 100 #

```

**Writing Values:**

```birch
str targetName = "myVariable";  
num myVariable = 50;
$targetName = 200;               # myVariable becomes 200

```

### Array Access via Pointers

Pointers excel at dynamic array access:

```birch
num scores [5];                  # Create array #
scores_0 = 85;
scores_1 = 92;
scores_2 = 78;

str arrayIndex = "scores_1";     # Point to second element  #
num currentScore = $arrayIndex;  # Gets 92  #
$arrayIndex = 95;                # Updates scores_1 to 95  #

```

### Dynamic Variable Selection

Use pointers for runtime variable selection:

```birch
str choice = "option1";
num option1 = 10;
num option2 = 20;
num option3 = 30;

sys-console = $choice;           # Outputs value of option1 (10) #
choice = "option2";              # Change target #
sys-console = $choice;           # Outputs value of option2 (20) #

```

### Advanced Pointer Patterns

**Conditional Variable Access:**

```birch
str playerType = "beginner";
num beginnerScore = 100;
num expertScore = 1000;

if difficulty == 1 : setBeginnerTarget | setExpertTarget;
num currentScore = $playerType;  # Gets appropriate score #

```

**Dynamic Array Indexing:**

```birch
num data [10];
num index = 3;
NUM-i = index;
syscall : toString;
str arrayRef = "data_" + STR-i;  # Creates "data_3" #
num value = $arrayRef;           # Access data_3 #

```

### Pointer Limitations

**Important Restrictions:**

-   Pointer variable must contain a valid, existing variable name
-   Cannot create pointer chains (pointers to pointers)
-   `$` symbol cannot be used in variable names
-   Runtime errors occur if pointer references non-existent variables
-   Pointers respect scoping rules (global vs local)

**Error Example:**

```birch
str badPointer = "nonExistentVar";
num value = $badPointer;         # Runtime error! #

```

## Control Flow

Birch provides conditional execution and loops with a unique syntax:

### If Statements

Conditional execution using comparison operators:

```birch
if leftValue operator rightValue : trueFunction | falseFunction;

```

**Examples:**

```birch
if x < y : handleSmaller | handleLarger;
if score >= 100 : celebrate | tryAgain;
if lives > 0 : continueGame | gameOver;

```

**Using `pass` for No Action:**

```birch
if x > 0 : doSomething | pass;   # Do nothing if false #
if ready == 1 : pass | waitMore; # Do nothing if true #

```

### While Loops

Repetitive execution based on conditions:

```birch
while leftValue operator rightValue : functionName;

```

**Examples:**

```birch
while counter < limit : incrementLoop;
while health > 0 : gameLoop;

```

**Critical Requirement:**

```birch
# GLOBAL VARIABLES REQUIRED FOR WHILE LOOPS #
.global{
    num counter = 0;
    num limit = 10;
}

.main{
    while counter < limit : incrementCounter;  # This works #
}

# LOCAL VARIABLES WILL CAUSE ERRORS #
.badExample{
    num localCounter = 0;  # Local variable #
    while localCounter < 10 : increment;  # Runtime error! #
}

```

**Note** : All conditionals ( if | while ), use numbers or number variables directly. Strings, or numerical expressions are not supported 
### Comparison Operators

All comparison operators work with numbers:

-   `<` Less than
-   `>` Greater than
-   `<=` Less than or equal
-   `>=` Greater than or equal
-   `==` Equal to (uses floating-point tolerance)
-   `!=` Not equal to (uses floating-point tolerance)

**Floating-Point Equality:**

```birch
# Birch handles floating-point comparison safely #
num a = 0.1 + 0.2;
num b = 0.3;
if a == b : equal | notEqual;  # Handles floating-point precision

```
**Note** : Floating point comparison has a precision of ```0.001f```

### Control Flow Patterns

**Simple Branching:**

```birch
if userInput == 1 : option1 | checkOtherOptions;

.checkOtherOptions{
    if userInput == 2 : option2 | option3;
}

```

**Loop with Counter:**

```birch
.global{
    num i = 0;
    num total = 0;
}

.calculateSum{
    while i < 100 : addNumber;
}

.addNumber{
    total = total + i;
    i = i + 1;
}

```

**Conditional Chains:**

```birch
.processScore{
    if score >= 90 : gradeA | checkB;
}

.checkB{
    if score >= 80 : gradeB | checkC;
}

.checkC{
    if score >= 70 : gradeC | gradeF;
}

```

## Functions

Functions are the primary code organization mechanism in Birch:

### Function Definition

```birch
.functionName{
    instruction1;
    instruction2;
    instruction3;
}

```

### Function Calls

**Regular Function Calls:**

```birch
call : functionName;

```

**System Function Calls:**

```birch
syscall : systemFunctionName;

```

### Function Characteristics

**No Parameters or  Return Values** 
-   Functions cannot accept parameters or return directly
-   Use global variables for data sharing
-   Modify global variables before and after calling functions
-   Read global variables within functions


**Example Pattern:**

```birch
.global{
    num inputA = 0;
    num inputB = 0;
    num result = 0;
}

.main{
    inputA = 10;
    inputB = 20;
    call : addNumbers;
    sys-console = result;  # result now contains 30 #
    syscall : write;
}

.addNumbers{
    result = inputA + inputB;
}

```

### Special Functions

**main Function:**

-   Required in every program
-   Program execution starts here
-   Called automatically after global initialization

**global Function:**

-   Optional but recommended
-   Executed before main
-   Used for global variable initialization
-   Used for library imports
-   **Is not** added to the function list, it cannot be manually called

## System Functions

Birch provides built-in system functions (syscalls) for common operations:

### Console I/O

**write** - Output to console:

```birch
sys-console = "Hello, World!";
syscall : write;

```

**read** - Read user input:

```birch
syscall : read;
# User input is stored in sys-console #

```

### Type Conversion

**toNum** - Convert string to number:

```birch
STR-i = "42.5";
syscall : toNum;
num myNumber = NUM-i;  # myNumber gets 42.5 #

```

**toString** - Convert number to string:

```birch
NUM-i = 42.5;
syscall : toString;
str myString = STR-i;  # myString gets "42.5" #

```

### Utility Functions

**random** - Generate random numbers:

```birch
NUM-i = 0;      # Minimum value #
NUM-x = 100;    # Maximum value #
syscall : random;
num randomValue = NUM-i;  # Random number between 0 and 100 #

```

**describe** - Display help information:

```birch
syscall : describe;  # Shows built-in help and documentation#

```

### Custom Runtime Array Creation

**arr** - Create arrays (runtimemethod):

```birch
STR-i = "num";        # Array type #
STR-x = "myArray";    # Array name #
NUM-i = 5;            # Array size #
syscall : arr;        # Creates myArray_0 through myArray_4 #

```

### System Function Patterns

**Input/Output Cycle:**

```birch
.getNumber{
    sys-console = "Enter a number: ";
    syscall : write;
    syscall : read;
    STR-i = sys-console;
    syscall : toNum;
    num userNumber = NUM-i;
}

```

**Number Processing:**

```birch
.displayScore{
    NUM-i = playerScore;
    syscall : toString;
    sys-console = "Your score: " + STR-i;
    syscall : write;
}

```

## System Variables

Birch automatically creates reserved variables for system function communication:

### Console Variable

**sys-console** - Console I/O buffer:

-   Used by `read` and `write` syscalls
-   Store output text before writing
-   Contains input text after reading

### String Scratch Variables

**STR-i, STR-x, STR-c, STR-l** - String scratch space:

-   Used as input/output for string-related syscalls
-   `STR-i` commonly used for input values
-   `STR-x` often used for secondary parameters
-   `STR-c` and `STR-l` for additional string operations

### Number Scratch Variables

**NUM-i, NUM-x, NUM-c, NUM-l** - Number scratch space:

-   Used as input/output for number-related syscalls
-   `NUM-i` commonly used for input values
-   `NUM-x` often used for ranges or secondary parameters
-   `NUM-c` and `NUM-l` for additional numeric operations


**Special Note** : The sufix for the variables are inspired by roman numerals
| suffix | number |
|--|--|
| i | 0001 |
| x | 0010 |
| l | 0100 |
| m | 1000 |

### Usage Patterns

**Type Conversion Pattern:**

```birch
# String to Number #
STR-i = stringValue;
syscall : toNum;
num result = NUM-i;

# Number to String #
NUM-i = numberValue;
syscall : toString;
str result = STR-i;

```

**Random Number Pattern:**

```birch
NUM-i = minValue;
NUM-x = maxValue;
syscall : random;
num randomResult = NUM-i;

```

### Important Warnings

**Do Not Modify Directly:**

-   These variables are managed and used by the system
-   Unexpected modifications may cause syscalls to behave incorrectly
-   Always use your own global variables for application data

**Recommended Practice:**

```birch
# AVOID THIS
NUM-i = 42;  # Direct modification #

# PREFER THIS
.global{
    num myNumber = 42;
}
# Use myNumber instead of NUM-i #

```

## Arrays

Birch supports arrays as collections of numbers or strings with indexed access:

### Array Declaration

**Direct Declaration (Recommended):**

```birch
num myNumbers [5];        # Creates myNumbers_0 through myNumbers_4 #
str myTexts [3];          # Creates myTexts_0 through myTexts_2 #

```

**Runtime Syscall Method: (Redundant)**

```birch
STR-i = "num";           # Set array type #
STR-x = "myNumbers";     # Set array name #
NUM-i = 5;               # Set array size #
syscall : arr;           # Create the array #

```

UPDATE : Arrays can be created with numbers, varaibles and pointers. They are not restricted to numbers only anymore. The syscall : arr is redundant.

### Array Access

Arrays use underscore notation for element access:

```birch
num scores [3];          # Create array
scores_0 = 85;           # First element (index 0)
scores_1 = 92;           # Second element (index 1)
scores_2 = 78;           # Third element (index 2)

num firstScore = scores_0;   # Read first element

```

### Array Characteristics

**Indexing:**

-   Indices start at 0
-   Elements named: `arrayName_0`, `arrayName_1`, `arrayName_2`, etc.
-   No bounds checking at runtime
-   Array must be declared with >=1 values

**Type Consistency:**

-   All elements in an array have the same type
-   Cannot mix numbers and strings in one array

### Dynamic Array Access

Use pointers for runtime array indexing:

```birch
num data [10];
num index = 3;

# Convert index to string
NUM-i = index;
syscall : toString;
str arrayRef = "data_" + STR-i;  # Creates "data_3" #

# Access array element dynamically
num value = $arrayRef;           # Read data_3 #
$arrayRef = 999;                 # Write to data_3 #

```

### Array Patterns

**Array Initialization:**

```birch
.global{
    num temperatures [7];  # Week's temperatures #
}

.initializeWeek{
    temperatures_0 = 20.5;  # Monday #
    temperatures_1 = 22.0;  # Tuesday #
    temperatures_2 = 19.5;  # Wednesday #
    temperatures_3 = 21.0;  # Thursday #
    temperatures_4 = 23.5;  # Friday #
    temperatures_5 = 25.0;  # Saturday #
    temperatures_6 = 24.0;  # Sunday #
}

```

**Array Processing with Loops:**

```birch
.global{
    num values [5];
    num index = 0;
    num sum = 0;
}

.calculateSum{
    while index < 5 : addElement;
}

.addElement{
    NUM-i = index;
    syscall : toString;
    str elementName = "values_" + STR-i;
    sum = sum + $elementName;
    index = index + 1;
}

```

**String Array Usage:**

```birch
str names [4];
names_0 = "Alice";
names_1 = "Bob";
names_2 = "Carol";
names_3 = "Dave";

str selectedName = names_2;  # Gets "Carol" #

```

### Array Memory Management

**Element Deletion:**

```birch
num myArray [5];
myArray_2 = 100;

# Delete specific element #
del myArray_2;

# Delete via pointer #
str elementRef = "myArray_3"; 
del $elementRef;

```

**Accessing Deleted Elements:**

-   Attempting to access deleted array elements causes runtime errors
-   No automatic reindexing occurs
-   Gaps in arrays are preserved

## Project Management

Birch supports multi-file projects through its import system:

### Project Structure Requirements

**Entry Point:**

-   Projects must have `Index.br` as the main file
-   `Index.br` serves as the program entry point
-   All other files are treated as libraries

**File Organization:**

```
project/
├── Index.br          # Main entry point #
├── utilities.br      # Utility functions #
├── gameEngine.br     # Game-specific code #
├── dataStructures.br # Data handling #
└── constants.br      # Global constants #

```

### Import System

**Import Syntax:**

```birch
import : libraryName;     # Imports libraryName.br #

```

**Import Rules:**

-   Only works in project mode (not single files)
-   Imported files must be in the same directory as `Index.br`
-   Import statements should be placed in the `global` function
-   Runtime displays messages if library files are not found

### Import Examples

**Basic Import Structure:**

```birch
# Index.br
.global{
    import : utilities;
    import : gameLogic;
    import : constants;
    
    # Global variables accessible to all files #
    num gameState = 0;
    str playerName = "";
}

.main{
    call : initializeGame;    # From gameLogic.br #
    call : startMainLoop;     # From gameLogic.br #
}

```

**Library File Structure:**

```birch
# utilities.br - No main function can be written #
#                global functions in libs are allowed #
.displayMessage{
    syscall : write;
}

.getUserInput{
    syscall : read;
}

.convertToNumber{
    STR-i = sys-console;
    syscall : toNum;
}

```

### Single File vs Project Mode

**Single File Programs:**

```birch
# standalone.br - Can be run directly #
.main{
    sys-console = "Hello from single file!";
    syscall : write;
}

```

**Project Programs:**

```birch
# Index.br - Must be part of a project #
.global{
    import : helpers;
}

.main{
    call : helperFunction;  # From helpers.br #
}

```

## Memory Management

Birch provides explicit memory management through the `del` statement:

### Variable Deletion

**Basic Deletion:**

```birch
num temporaryVar = 42;
del temporaryVar;         # Remove from memory #
# temporaryVar no longer exists #

```

**Pointer-Based Deletion:**

```birch
str target = "myVariable";
num myVariable = 100;
del $target;              # Deletes myVariable #
# myVariable no longer exists, target still contains "myVariable" #

```

### Deletion Characteristics

**Immediate Effect:**

-   Memory is freed immediately upon deletion
-   Variable becomes inaccessible after deletion
-   Attempting to use deleted variables causes runtime errors

**Scope Respect:**

-   Works with both local and global variables
-   Local variables automatically deleted when function exits
-   Global variables persist until explicitly deleted

### Array Element Deletion

**Individual Element Deletion:**

```birch
num myArray [5];
myArray_2 = 100;
del myArray_2;            # Delete specific element #

# myArray_0, myArray_1, myArray_3, myArray_4 still exist #
# myArray_2 no longer exists #

```

**Dynamic Element Deletion:**

```birch
num data [10];
num index = 3;
NUM-i = index;
syscall : toString;
str elementName = "data_" + STR-i;
del $elementName;         # Deletes data_3 #

```

### Memory Management Patterns

**Temporary Variable Cleanup:**

```birch
.processData{
    num tempResult = calculation1 + calculation2;
    num finalResult = tempResult * modifier;
    
    del tempResult;       # Clean up temporary variable #
    
    # Continue with finalResult #
}

```

**Resource Management:**

```birch
.global{
    str inputBuffer = "";
    num processingFlags [10];
}

.cleanup{
    del inputBuffer;
    del processingFlags_0;
    del processingFlags_1;
    del processingFlags_2;
    # ... delete other array elements as needed #
}

```

**Conditional Cleanup:**

```birch
.processConditionally{
    if needsTemporaryStorage == 1 : createTemporary | pass;
    
    # ... processing logic ... #
    
    if needsTemporaryStorage == 1 : cleanupTemporary | pass;
}

.createTemporary{
    num tempStorage = 0;
}

.cleanupTemporary{
    del tempStorage;
}

```

### Memory Management Best Practices

**When to Delete Variables:**

-   Temporary variables after use
-   Large data structures when processing is complete
-   Variables that are no longer needed in the program flow

**When NOT to Delete Variables:**

-   Variables needed later in the program
-   Global variables used across multiple functions
-   Variables that might be accessed by other functions

**Error Prevention:**

```birch
# AVOID - Using deleted variables #
num myVar = 42;
del myVar;
num result = myVar;       # Runtime error! #

# CORRECT - Check before deletion
num myVar = 42;
num result = myVar;       # Use first #
del myVar;                # Delete after use #

```

## Variable Naming Rules

Birch has specific requirements for variable names:

### Allowed Characters

**Valid Characters:**

-   Letters: `a-z`, `A-Z`
-   Underscores: `_`
-   Case sensitivity: `myVar` and `MyVar` are different variables

**Valid Examples:**

```birch
num playerScore = 0;
str firstName = "";
num temp_value = 0;
str USER_INPUT = "";
num calculateResult = 0;

```

### Forbidden Characters

**Prohibited Characters:**

-   Dots (`.`): `my.var` 
-   Commas (`,`): `my,var` 
-   Spaces: `my var` 
-   Question marks (`?`): `my?var` 
-   Dollar signs (`$`): `my$var`   (reserved for pointers)
-   Numbers: `var123`, `123var`, `var1var` 

**Invalid Examples:**

```birch
num player.score = 0;     # Error: contains dot #
str first,name = "";      # Error: contains comma #
num temp value = 0;       # Error: contains space #
str user?input = "";      # Error: contains question mark #
num my$variable = 0;      # Error: contains dollar sign #
num score2 = 0;          # Error: contains number #

```

### Naming Best Practices

**Descriptive Names:**

```birch
# GOOD #
num playerHealth = 100;
str currentLevel = "beginner";
num remainingLives = 3;

# POOR #
num x = 100;
str s = "beginner";
num n = 3;

```

**Consistent Conventions:**

```birch
# Choose one style and stick with it
num playerScore = 0;      # camelCase #
num player_score = 0;     # snake_case #
num PLAYER_SCORE = 0;     # UPPER_CASE #

```

**Array Naming:**

```birch
# Remember: arrays create indexed elements
num playerScores [5];     # Creates playerScores_0, playerScores_1, etc. #
str levelNames [10];      # Creates levelNames_0, levelNames_1, etc. #

```

**Pointer-Related Naming:**

```birch
# Pointer variables should indicate their purpose #
str targetVariableName = "playerScore";
str currentArrayElement = "scores_3";
str selectedVariable = "healthPoints";

# Avoid confusing pointer names #
str ptr = "someVar";      # Unclear purpose
str ref = "data";         # Too generic

```

### Case Sensitivity Examples

```birch
# These are all different variables #
num score = 100;
num Score = 200;
num SCORE = 300;
num ScOrE = 400;

# Function names are also case sensitive #
.processData{ }           # Different from #
.ProcessData{ }           # Different from #
.PROCESSDATA{ }

```

## Common Patterns

Birch programming follows several established patterns for common tasks:

### Input/Output Patterns

**Basic Output:**

```birch
.displayMessage{
    sys-console = "Your message here";
    syscall : write;
}

```

**Basic Input:**

```birch
.getUserInput{
    sys-console = "Enter your choice: ";
    syscall : write;
    syscall : read;
    # User input now in sys-console #
}

```

**Input with Validation:**

```birch
.global{
    num userNumber = 0;
    str inputValid = "";
}

.getValidNumber{
    sys-console = "Enter a number: ";
    syscall : write;
    syscall : read;
    
    STR-i = sys-console;
    syscall : toNum;
    userNumber = NUM-i;
    
    if userNumber > 0 : validInput | invalidInput;
}

.validInput{
    inputValid = "yes";
}

.invalidInput{
    sys-console = "Invalid input. Please try again.";
    syscall : write;
    call : getValidNumber;
}

```

### Type Conversion Patterns

**String to Number:**

```birch
.convertStringToNumber{
    STR-i = sys-console;     # Put string in STR-i
    syscall : toNum;         # Convert to number in NUM-i
    num myVariable = NUM-i;  # Store the result
}

```

**Number to String:**

```birch
.convertNumberToString{
    NUM-i = myNumber;        # Put number in NUM-i
    syscall : toString;      # Convert to string in STR-i
    sys-console = STR-i;     # Use the string
}

```

**Combined Conversion for Display:**

```birch
.displayScore{
    NUM-i = playerScore;
    syscall : toString;
    sys-console = "Score: " + STR-i + " points";
    syscall : write;
}

```

### Loop Patterns

**Counter-Based Loop:**

```birch
.global{
    num counter = 0;
    num limit = 10;
}

.countingLoop{
    while counter < limit : processIteration;
}

.processIteration{
    # Do work here #
    sys-console = "Iteration: ";
    NUM-i = counter;
    syscall : toString;
    sys-console = sys-console + STR-i;
    syscall : write;
    
    counter = counter + 1;
}

```

**Array Processing Loop:**

```birch
.global{
    num data [5];
    num index = 0;
    num arraySize = 5;
}

.processArray{
    while index < arraySize : processElement;
}

.processElement{
    NUM-i = index;
    syscall : toString;
    str elementName = "data_" + STR-i;
    
    # Process element via pointer
    num currentValue = $elementName;
    $elementName = currentValue * 2;  # Double the value #
    
    index = index + 1;
}

```

### Conditional Patterns

**Simple Branching:**

```birch
.checkCondition{
    if value > threshold : handleHigh | handleLow;
}

.handleHigh{
    sys-console = "Value is high";
    syscall : write;
}

.handleLow{
    sys-console = "Value is low";
    syscall : write;
}

```

**Multi-Level Conditions:**

```birch
.gradeStudent{
    if score >= 90 : gradeA | checkLowerGrades;
}

.checkLowerGrades{
    if score >= 80 : gradeB | checkEvenLower;
}

.checkEvenLower{
    if score >= 70 : gradeC | gradeF;
}

```

**State Machine Pattern:**

```birch
.global{
    num currentState = 1;
}

.processState{
    if currentState == 1 : state1Handler | checkState2;
}

.checkState2{
    if currentState == 2 : state2Handler | checkState3;
}

.checkState3{
    if currentState == 3 : state3Handler | errorState;
}

```


### Dynamic Programming Patterns

**Variable Selection:**

```birch
.global{
    str selectedVar = "option1";
    num option1 = 10;
    num option2 = 20;
    num option3 = 30;
}

.processSelection{
    num currentValue = $selectedVar;
    $selectedVar = currentValue * 2;
    
    sys-console = "Updated value: ";
    NUM-i = $selectedVar;
    syscall : toString;
    sys-console = sys-console + STR-i;
    syscall : write;
}

```

**Dynamic Array Access:**

```birch
.global{
    num gameData [10];
    num playerIndex = 0;
}

.updatePlayerData{
    NUM-i = playerIndex;
    syscall : toString;
    str playerSlot = "gameData_" + STR-i;
    
    $playerSlot = $playerSlot + scoreIncrement;
}

```


## Program Execution Flow

Understanding Birch's execution model is crucial for effective programming:

### Execution Phases

**1. Parsing and Registration:**

-   All functions from source file(s) are parsed
-   Function definitions are registered in memory
-   Global functions are called
-   Import resolution occurs for project files

**2. System Initialization:**

-   System variables are created (sys-console, STR-i, NUM-i, etc.)
-   System functions are registered
-   Runtime environment is prepared

**3. Main Function Execution:**

-   The `main` function begins execution
-   This is the program's primary entry point
-   All user-defined logic flows from here

**4. Function Call Chain:**

-   Functions call other functions using `call : functionName`
-   System functions are called using `syscall : functionName`
-   Control returns to calling function after completion

**5. Program Termination:**

-   Program ends when `main` function completes
-   All variables are automatically cleaned up
-   System resources are released

### Execution Flow Diagram

```
Program Start
     ↓
Parse All Functions
     ↓
Initialize System Variables
     ↓
Execute .global{} (if present)
     ↓
Execute .main{}
     ↓
[Function Calls and Control Flow]
     ↓
Program End

```

### Variable Lifecycle

**Global Variables:**

-   Created during global function execution
-   Persist throughout entire program execution
-   Accessible from all functions
-   Destroyed only when program ends or explicitly deleted
-   System Variables are global

**Local Variables:**

-   Created when function begins execution
-   Exist only within the declaring function
-   Automatically destroyed when function exits
-   Cannot be accessed from other functions

### Error Handling During Execution

**Runtime Errors:**

-   Invalid variable access (using deleted or non-existent variables)
-   Type mismatches in operations
-   System function failures
-   Import resolution failures

**Error Recovery:**

-   Runtime displays error message with specific instruction
-   Program execution stops at the error point
-   No automatic error recovery mechanism
-   Debugging requires examining the failing instruction

### Common Mistakes and Solutions

**While Loop with Local Variables:**

```birch
# WRONG - Will cause runtime error #
.badLoop{
    num counter = 0;  # Local variable #
    while counter < 10 : increment;  # Error! #
}

# CORRECT - Use global variables
.global{
    num counter = 0;  # Global variable #
}

.goodLoop{
    while counter < 10 : increment;  # Works! #
}

```

**Pointer to Non-Existent Variable:**

```birch
# WRONG - Runtime error #
str pointer = "nonExistentVar";
num value = $pointer;  # Error! #

# CORRECT - Ensure target exists #
num targetVar = 42;
str pointer = "targetVar";
num value = $pointer;  # Works! #

```

**Using Deleted Variables:**

```birch
# WRONG - Runtime error #
num myVar = 42;
del myVar;
num result = myVar;  # Error! #

# CORRECT - Use before deleting #
num myVar = 42;
num result = myVar;  # Use first #
del myVar;           # Delete after use #

```

# Conclusion
Hello! My name is Dhruv Bhojaraj. I decided to write birch because the logic for this project just would not leave my head. This project is the successor to my previous attempt at writing a programming language (AKA : PINESCRIPT ).

I had a lot of fun making this project, implementing a lot of features that i expected to have. Although using this language may be a bit tedious, i designed it the way i wanted to, within the feel of control and designed syntax.

I doubt this project will get any more updates and work on it, but please fee free to drop buy your feedback if you have the time!
