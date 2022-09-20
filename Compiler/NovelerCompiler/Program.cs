using Noveler.Compiler;
using System.CommandLine;
using System.Runtime.InteropServices;
using System.Text;

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

            IReadOnlyList<CompilerMessage> messages;
            List<byte> compileResult;
            bool compileIsSuccesful;
            using (var textReader = new StringReader(scriptString))
            {
                compileIsSuccesful = Compiler.Compile(textReader, out compileResult, out messages);
            }

            if (messages.Count != 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var message in messages)
                {
                    sb.AppendLine($"Code: {message.Code:d} ({message.Code})\t{message.Code.GetMessageTypeFromCode()}: {message.Content}\tSource: \"{message.Source}\"");
                }

                Console.WriteLine(sb.ToString());
            }

            if (compileIsSuccesful)
                return Task.FromResult(ReturnCode.CompilerError);

            string destination = outputFile.Extension == CompiledNovelExtension ?
                outputFile.FullName :
                outputFile.FullName + CompiledNovelExtension;

            try
            {
                using var output = File.OpenWrite(destination);

                var rs = CollectionsMarshal.AsSpan(compileResult);
                output.Write(rs);
            }
            catch (Exception)
            {
                Console.WriteLine($"Unable to write the compile output to \"{destination}\".");
                return Task.FromResult(ReturnCode.OutputFailure);
            }

            return Task.FromResult(ReturnCode.Succes);
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
