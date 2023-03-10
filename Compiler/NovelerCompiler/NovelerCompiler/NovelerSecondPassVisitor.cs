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
using Microsoft.CodeAnalysis;
using Noveler.Compiler.Grammar;
using OneOf;

namespace Noveler.Compiler
{
	internal sealed class VisitorContextState
	{
		public Dictionary<string, FunctionInfo> FunctionTable { get; } = new(128);
		public Dictionary<string, TypeInfo> TypeTable { get; } = new(128);
		public Dictionary<string, object> StubTable { get; } = new(128);

		public List<string> StringTable { get; } = new(128);
	}

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

	internal sealed record TypeInfo(string Name, string FullyQualifiedName, int SizeInBytes, Optional<FieldInfo[]> FieldInfo);

	internal sealed record FieldInfo(int Offset, TypeInfo TypeInfo);

	internal sealed record FunctionInfo(string Name, string FullyQualifiedName, TypeInfo ReturnType, ParameterInfo[] Parameters, FunctionSyntaxTree SyntaxTree);

	internal sealed record ParameterInfo(string Name, TypeInfo TypeInfo);

	internal static class PredefinedInfo
	{
		public static readonly TypeInfo Int8 = new TypeInfo("Int8", "Int8", 1, default);
		public static readonly TypeInfo Int16 = new TypeInfo("Int16", "Int16", 2, default);
		public static readonly TypeInfo Int32 = new TypeInfo("Int32", "Int32", 4, default);
		public static readonly TypeInfo Int64 = new TypeInfo("Int64", "Int64", 8, default);

		public static readonly TypeInfo UInt8 = new TypeInfo("UInt8", "UInt8", 1, default);
		public static readonly TypeInfo UInt16 = new TypeInfo("UInt16", "UInt16", 2, default);
		public static readonly TypeInfo UInt32 = new TypeInfo("UInt32", "UInt32", 4, default);
		public static readonly TypeInfo UInt64 = new TypeInfo("UInt64", "UInt64", 8, default);

		public static readonly TypeInfo Float32 = new TypeInfo("Float32", "Float32", 4, default);
		public static readonly TypeInfo Float64 = new TypeInfo("Float64", "Float64", 8, default);
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
