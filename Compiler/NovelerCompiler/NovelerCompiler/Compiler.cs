﻿using Antlr4.Runtime;
using Noveler.Compiler.Grammar;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

			// first pass: plunge imports and create symbol table
			NovelerFirstPassVisitor firstPassVisitor = new(fileInfo);
			using FirstPassResult firstPassResult = firstPassVisitor.VisitStory(context);

			// second pass:
			NovelerSecondPassVisitor secondPassVisitor = new(firstPassResult);
			SecondPassResult secondPassResult = secondPassVisitor.VisitStory(context);

			// TODO: transformations

			// TODO: actually return compiled result
			return CompileResult.FromSuccess(Array.Empty<byte>());
		}
	}
}
