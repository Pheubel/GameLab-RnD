using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Noveler.Compiler.Grammar;
using Noveler.Compiler.SymbolTypes;
using OneOf.Types;

namespace Noveler.Compiler
{
	internal sealed class SecondPassResult
	{

	}

	internal sealed class NovelerSecondPassVisitor : NovelerParserBaseVisitor<SecondPassResult?>
	{
		private FirstPassResult _firstPassResult;

		public NovelerSecondPassVisitor(FirstPassResult firstPassResult)
		{
			_firstPassResult = firstPassResult;
		}

		public override SecondPassResult VisitStory([Antlr4.Runtime.Misc.NotNull] NovelerParser.StoryContext context)
		{
			SecondPassResult result = new SecondPassResult();

			//TODO: visit several parts

			return result;
		}
	}

	// TODO: implement and move to separate files
	internal abstract class SyntaxTree
	{

	}

	internal sealed class StorySyntaxTree : SyntaxTree
	{

	}

	internal sealed class FunctionSyntaxTree : SyntaxTree
	{

	}
}
