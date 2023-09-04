using Antlr4.Runtime;
using Noveler.Compiler.Grammar;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noveler.Compiler.CodeDomainObjectModel;

namespace Noveler.Compiler
{
	public static class Compiler
	{
		public static CompileResult Compile(FileInfo fileInfo)
		{
			using var fs = fileInfo.OpenRead();
			return Compile(fs, fileInfo);
		}

		public static CompileResult Compile(Stream stream, Optional<FileInfo> fileInfo = default)
		{
			// Set up antlr
			AntlrInputStream inputStream = new AntlrInputStream(stream);
			NovelerLexer novelerLexer = new NovelerLexer(inputStream);
			CommonTokenStream commonTokenStream = new CommonTokenStream(novelerLexer);
			NovelerParser novelerParser = new NovelerParser(commonTokenStream);
			NovelerParser.StoryContext context = novelerParser.story();

			// gather all compile units
			NovelerVisitor visitor = new NovelerVisitor(fileInfo.HasValue ? fileInfo.Value.FullName : string.Empty);
			var compilationUnits = (Dictionary<string, CompilationUnit>)visitor.VisitStory(context);

			var mainCompileUnit = compilationUnits[visitor.SourcePath];

			Console.WriteLine("Namespaces:");
			foreach (var ns in mainCompileUnit.NameSpaces)
			{
				Console.WriteLine(ns);
			}

			Console.WriteLine("Threads:");
            foreach (var thread in mainCompileUnit.Threads)
            {
                Console.WriteLine(thread);
            }

            // TODO: complete type definitions

            // TODO: syntax tree formation

            // TODO: syntax tree to opcodes

            // TODO: actually return compiled result
            return CompileResult.FromSuccess(Array.Empty<byte>());
		}
	}
}
