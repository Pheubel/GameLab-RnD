using Noveler;
using System.CommandLine;
using System.Runtime.InteropServices;

public class Program
{
    const string NovelExtension = ".nvl";
    const string CompiledNovelExtension = ".cnvl";

    static async Task<int> Main(string[] args)
    {
        var fileOption = new Option<FileInfo?>(
            name: "--input",
            description: "The file to read and display on the console.",
            parseArgument: result =>
            {
                string? filePath = result.Tokens.Single().Value;

                if (!File.Exists(filePath))
                {
                    result.ErrorMessage = "Error: File does not exist.";
                    return null;
                }

                var fileInfo = new FileInfo(filePath);

                if (fileInfo.Extension != NovelExtension)
                    Console.WriteLine($"Warning: Input file is not a \"{NovelExtension}\" file.");

                return fileInfo;
            });

        var outputOption = new Option<FileInfo?>(
            name: "--output",
            description: "The file to output the compiled script to.",
            parseArgument: result =>
            {
                string? filePath = result.Tokens.Single().Value;

                var fileInfo = new FileInfo(filePath);

                if (fileInfo.Exists)
                {
                    Console.WriteLine("Warning: output file has been overwritten.");
                }

                return fileInfo;
            });

        fileOption.Arity = new ArgumentArity(1, 1);
        outputOption.Arity = new ArgumentArity(1, 1);

        fileOption.IsRequired = true;
        outputOption.IsRequired = true;

        fileOption.AddAlias("-i");
        outputOption.AddAlias("-o");

        var rootCommand = new RootCommand("Compiles Noveler scripts into easy to interpret instructions for the Noveler runtime.");
        rootCommand.AddOption(fileOption);
        rootCommand.AddOption(outputOption);

        rootCommand.SetHandler((inputFile, outputFile) =>
        {
            if (inputFile == null)
            {
                Console.WriteLine("Missing input file. See --help");
                return Task.FromResult(ReturnCode.MissingInput);
            }

            if (outputFile == null)
            {
                Console.WriteLine("Missing output destination. See --help");
                return Task.FromResult(ReturnCode.MissingInput);
            }

            var scriptString = File.ReadAllText(inputFile.FullName);

            var resultCode = Compiler.Compile(scriptString, out string compileResult);

            if (resultCode != Compiler.Result.Succes)
                return Task.FromResult(ReturnCode.CompilerError);

            string destination = outputFile.Extension == CompiledNovelExtension ?
                outputFile.FullName :
                outputFile.FullName + CompiledNovelExtension;

            try
            {
                using var output = File.OpenWrite(destination);

                output.Write(MemoryMarshal.AsBytes(compileResult.AsSpan()));
            }
            catch (Exception)
            {
                Console.WriteLine($"Unable to write the compile output to \"{destination}\".");
                return Task.FromResult(ReturnCode.OutputFailure);
            }

            return Task.FromResult(resultCode);
        },
            fileOption, outputOption);

        return await rootCommand.InvokeAsync(args);
    }

    public enum ReturnCode
    {
        Succes,
        MissingInput,
        OutputFailure,
        CompilerError,
    }
}
