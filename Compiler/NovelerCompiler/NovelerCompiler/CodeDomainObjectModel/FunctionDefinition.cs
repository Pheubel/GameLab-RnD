using Microsoft.VisualBasic.FileIO;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Noveler.Compiler.CodeDomainObjectModel
{
    internal sealed record FunctionDefinition(
        string Name,
        NamespaceDefinition Namespace,
        TypeDefinition? ParentType,
        CompilationUnit? OriginalCompilationUnit,
        SymbolScope SymbolScope,
        FunctionDeclaration OriginalDeclaration) : IQualifyable
    {
        // TODO: figure this out properly, could use a list or custom type
        public SequenceLookUpTable<FunctionArgumentDefinition> FunctionArguments { get; } = new();
        public TypeDefinition? ReturnType { get; set; }

        [MemberNotNullWhen(true, nameof(ParentType))]
        public bool IsStructureMethod => ParentType != null;

        [MemberNotNullWhen(true, nameof(ReturnType))]
        public bool IsFullyDefined { get; set; }

        public string GetFullyQualifiedName()
        {
            if (!IsFullyDefined)
                throw new Exception("Type is not fully qualified.");

            StringBuilder sb = new();

            if (IsStructureMethod)
            {
                ParentType.GetFullyQualifiedName(sb);
            }
            else
            {
                Namespace.GetFullyQualifiedName(sb);
            }

            sb.Append("::");
            sb.Append(Name);
            sb.Append('(');

            foreach (var argument in FunctionArguments)
            {
                argument.ArgumentType.GetFullyQualifiedName(sb, sb.Length);
                sb.Append(',');
            }

            if (FunctionArguments.Count > 0)
                sb.Remove(sb.Length - 1, 1);

            sb.Append(')');
            sb.Append(':');
            ReturnType.GetFullyQualifiedName(sb, sb.Length);

            return sb.ToString();

            // should look like Foo.Bar.Fizz::Buzz(Ping,Pong):Unit
        }

        public void GetFullyQualifiedName(StringBuilder stringBuilder)
        {
            if (!IsFullyDefined)
                throw new Exception("Type is not fully qualified.");

            if (IsStructureMethod)
            {
                ParentType.GetFullyQualifiedName(stringBuilder, stringBuilder.Length);
            }
            else
            {
                Namespace.GetFullyQualifiedName(stringBuilder, stringBuilder.Length);
            }

            stringBuilder.Append("::");
            stringBuilder.Append(Name);
            stringBuilder.Append('(');

            foreach (var argument in FunctionArguments)
            {
                argument.ArgumentType.GetFullyQualifiedName(stringBuilder, stringBuilder.Length);
                stringBuilder.Append(',');
            }

            if (FunctionArguments.Count > 0)
                stringBuilder.Remove(stringBuilder.Length - 1, 1);

            stringBuilder.Append(')');
            stringBuilder.Append(':');
            ReturnType.GetFullyQualifiedName(stringBuilder, stringBuilder.Length);
        }

        public void GetFullyQualifiedName(StringBuilder stringBuilder, int index)
        {
            if (!IsFullyDefined)
                throw new Exception("Type is not fully qualified.");

            if (IsStructureMethod)
            {
                ParentType.GetFullyQualifiedName(stringBuilder, index);
            }
            else
            {
                Namespace.GetFullyQualifiedName(stringBuilder, index);
            }

            stringBuilder.Append("::");
            stringBuilder.Append(Name);
            stringBuilder.Append('(');

            foreach (var argument in FunctionArguments)
            {
                argument.ArgumentType.GetFullyQualifiedName(stringBuilder, stringBuilder.Length);
                stringBuilder.Append(',');
            }

            if (FunctionArguments.Count > 0)
                stringBuilder.Remove(stringBuilder.Length - 1, 1);

            stringBuilder.Append(')');
            stringBuilder.Append(':');
            ReturnType.GetFullyQualifiedName(stringBuilder, stringBuilder.Length);
        }

        public string GetFunctionSignature()
        {
            if (!IsFullyDefined)
                throw new Exception("Type is not fully qualified.");

            StringBuilder sb = new();
            sb.Append(Name);
            sb.Append('(');

            foreach (var argument in FunctionArguments)
            {
                argument.ArgumentType.GetFullyQualifiedName(sb, sb.Length);
                sb.Append(',');
            }

            if (FunctionArguments.Count > 0)
                sb.Remove(sb.Length - 1, 1);

            sb.Append(')');
            sb.Append(':');
            ReturnType.GetFullyQualifiedName(sb, sb.Length);

            return sb.ToString();
        }

        public string GetFunctionSignatureWithoutReturnType()
        {
            if (!IsFullyDefined)
                throw new Exception("Type is not fully qualified.");

            StringBuilder sb = new();
            sb.Append(Name);
            sb.Append('(');

            foreach (var argument in FunctionArguments)
            {
                argument.ArgumentType.GetFullyQualifiedName(sb, sb.Length);
                sb.Append(',');
            }

            if (FunctionArguments.Count > 0)
                sb.Remove(sb.Length - 1, 1);

            sb.Append(')');

            return sb.ToString();
        }
    }
}