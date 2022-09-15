I have a surface level understanding of how compilers work. For my own scripting language I will have to make make my own in order to turn a script into instructions.

I will be following: https://compilers.iecc.com/crenshaw/

I begin with a basic program that can read a file, apply a transformation and write the output by reading arguments from a command line.

![SimpleStringTransformation.png](./Resources/SimpleStringTransformation.png)

Now that I can emit output to a file I can start with the process of translating input to a set of instructions as output, these instructions will be a set of codes that my runtime consumes to change the current state.

For now I have defined a set of instructions used for basic math operations