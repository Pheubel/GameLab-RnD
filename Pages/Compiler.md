## Compiler

I have a surface level understanding of how compilers work. For my own scripting language I will have to make make my own in order to turn a script into instructions.

As my main guideline I will be following: https://compilers.iecc.com/crenshaw/

### Getting started

I begin with a basic program that can read a file, apply a transformation and write the output by reading arguments from a command line.

![SimpleStringTransformation.png](./Resources/SimpleStringTransformation.png)

Now that I can emit output to a file I can start with the process of translating input to a set of instructions as output, these instructions will be a set of codes that my runtime consumes to change the current state.

### Simple Math

For now I have defined a set of instructions used for basic math operations. In order to generate those instructions I will have to strucure the code in the script to make it easy for my compiler to place the instructions sequentially. To do this, I make use of Abstract syntax tree. An abstract syntax tree is a tree with nodes containing the relevant tokens for instructions, like variables and instruction codes. By traversing the tree it is possible correctly formulate the instructions in the right order.

As example, the statement `a = 5 + 6` would be transformed into:
![SyntaxTreeOne](./Resources/SyntaxTreeOne.png)
This would create an add operation to add `5` and `6` together, after the result has been calculated it would store the result in variable `a`.

Smart compilers can analyze a syntax tree to come up with techniques to reduce the size of the generated code, like constant folding, a process in which the compiler checks if an equation can be skipped by computing the result and replacing the node with the result. Using the tree above it would look like:
![SyntaxTreeTwo](./Resources/SyntaxTreeTwo.png)
Now that the tree has been simplified, it allows the compiler to emit just the storing of `11` into the `a` variable.

For now I won't be using any tricks to generate optimized code, due to the early stage of development.

I am able to calculate the expression `1 + 2 - 3 + 4 + 5`, problems arrise when the formula has parts wrapped in parentheses. With the current way of handling tokens the generated tree will look the same as without. In order to branch the syntax tree I need to be able to make small trees that I can stich together later. To do this I make use of temporary nodes. With each open parentesis I create a temporary node and push my current node onto a stack, when the compiler gets to a closing parenthesis it pops the stack to stitch the tree. With the formula `((1 + 2) - (3 + 4)) + 5` the tree gets created as followed:
![SyntaxTreeStichAnimation]()
TODO: create animation

For factor evaluation, such as `*` and `/` the rules are slightly different, in a formula they take precedence over `+` and `-` and will have to occur earlier when traversing the syntax tree.

A syntax tree for the equation `5 + 6 * 7` would look like:
![SyntaxTreeMultiplication]()
TODO: create tree

### Logic

Inside of the language it is possible to include logic for branching. To handle this I need to choose a method of branching. After referencing Assembly instructions, a common way to handle branching is to test for `0` to decide if the code execution should jump over a block of code.

TODO: animation for jump logic

### Variables

Variables require special attention in order to use within the language. their value or type might not be immediately known when compiling, requiring an extra step in order to use right. In order to keep track of variables, I will be using a symbol table, with this I can keep track of which variables exist inside of the script. I will also be able to store additional data, like the type of the variable's value.

TODO: image/code of example table

Whenever a variable gets encountered, an entry will be stored in the table, or an existing record will have a reference to the location in the tree added. After the syntax tree has been generated, I will be able to replace the references with a properly structured node with correct typing.

### Introducing Story



## Runtime V1

## First Round of Testing