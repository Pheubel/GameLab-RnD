using Antlr4.Runtime;
using Noveler.Compiler.Grammar;
using OneOf;
using OneOf.Types;
using System.Runtime.InteropServices;

namespace Noveler.Compiler
{
	internal sealed class FirstPassResult : IDisposable
	{
		const int DefaultStartSizeImport = 16;
		const int DefaultStartSizeSymbleTable = 128;

		public Dictionary<string, SymbolInfo> SymbolTable { get; } = new(DefaultStartSizeSymbleTable);
		public IReadOnlyDictionary<string, NovelerParser.Imported_fileContext> VisitedFiles => _visitedFiles;

		private readonly Dictionary<string, NovelerParser.Imported_fileContext> _visitedFiles = new(DefaultStartSizeImport);
		private readonly List<FileStream> _openStreams = new(DefaultStartSizeImport);

		public OneOf<NovelerParser.Imported_fileContext, Error<int>> AddOrGetImport(FileInfo importFile, out bool alreadyExists)
		{
			// report errors when imported file can't be found
			if (!importFile.Exists)
			{
				// TODO: signal failure for importing better

				alreadyExists = false;
				return new Error<int>(1);
			}

			ref var importedFileContext = ref CollectionsMarshal.GetValueRefOrAddDefault(_visitedFiles, importFile.FullName, out alreadyExists);

			if (!alreadyExists)
			{
				FileStream importedFileStream;
				try
				{
					importedFileStream = importFile.OpenRead();
				}
				catch (UnauthorizedAccessException)
				{
					return new Error<int>(2);
				}
				catch (IOException)
				{
					return new Error<int>(3);
				}

				AntlrInputStream inputStream = new AntlrInputStream(importedFileStream);
				NovelerLexer novelerLexer = new NovelerLexer(inputStream);
				CommonTokenStream commonTokenStream = new CommonTokenStream(novelerLexer);
				NovelerParser novelerParser = new NovelerParser(commonTokenStream);
				importedFileContext = novelerParser.imported_file();

				_openStreams.Add(importedFileStream);
			}

			return importedFileContext!;
		}

		public void Dispose()
		{
			if (_openStreams.Count > 0)
			{
				for (int i = 0; i < _openStreams.Count; i++)
				{
					_openStreams[i].Close();
				}

				_openStreams.Clear();
			}

			GC.SuppressFinalize(this);
		}

		~FirstPassResult()
		{
			Dispose();
		}
	}
}
