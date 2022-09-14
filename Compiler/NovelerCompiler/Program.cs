using Noveler;
using System.CommandLine;
using System.Runtime.InteropServices;

public class Program
{
    static async Task<int> Main(string[] args)
    {
        var fileOption = new Option<FileInfo?>(
            name: "--file",
            description: "The file to read and display on the console.",
            parseArgument: result =>
            {
                if (result.Tokens.Count == 0)
                {
                    result.ErrorMessage = "";
                    return null;
                }

                string? filePath = result.Tokens.Single().Value;

                if (!File.Exists(filePath))
                {
                    result.ErrorMessage = "File does not exist";
                    return null;
                }

                return new FileInfo(filePath);
            });

        var outputOption = new Option<FileInfo?>(
            name: "--output",
            description: "The file ot output the compiled script to. "
            );

        var rootCommand = new RootCommand("Compiles Noveler scripts into easy to interpret instructions forthe Noveler runtime.");
        rootCommand.AddOption(fileOption);
        rootCommand.AddOption(outputOption);

        rootCommand.SetHandler((inputFile, outputFile) =>
        {
            var scriptString = File.ReadAllText(inputFile!.FullName);

            var resultCode = Compiler.Compile(scriptString, out string compileResult);

            if (resultCode != Compiler.Result.Succes)
                return Task.FromResult(resultCode);

            string destination = outputFile!.FullName;

            using (var output = File.OpenWrite(destination))
            {
                output.Write(MemoryMarshal.AsBytes(compileResult.AsSpan()));
            }

            return Task.FromResult(resultCode);
        },
            fileOption, outputOption);

        return await rootCommand.InvokeAsync(args);
    }
}

namespace Noveler
{
    public class Compiler
    {
        public static Result Compile(string script, out string result)
        {
            // as example reverse the input
            result = script.Reverse().ToString()!;

            return Result.Succes;
        }

        /// <summary>
        /// A list of possible result codes from compiling a script.
        /// </summary>
        public enum Result
        {
            Succes
        }
    }
}