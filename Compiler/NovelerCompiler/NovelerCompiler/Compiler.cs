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

            Console.WriteLine($"Namespaces ({mainCompileUnit.NameSpaces.Count}):");
            foreach (var ns in mainCompileUnit.NameSpaces)
            {
                Console.WriteLine(ns);
            }

            Console.WriteLine($"Threads ({mainCompileUnit.Threads.Count}):");
            foreach (var thread in mainCompileUnit.Threads)
            {
                Console.WriteLine(thread);
            }

            Dictionary<string, NameSpace> namespaceDeclarations = new();

            // TODO: fix up type references to appropriate types in appropriate namespaces

            Dictionary<string, NamespaceDefinition> namespaceDefinitions = new();

            var globalNamespaceDefinition = new NamespaceDefinition("__global__");

            globalNamespaceDefinition.TypeDefinitions.Add(PredefinedTypeDefinitions.Int8.Name, PredefinedTypeDefinitions.Int8);
            globalNamespaceDefinition.TypeDefinitions.Add(PredefinedTypeDefinitions.Int16.Name, PredefinedTypeDefinitions.Int16);
            globalNamespaceDefinition.TypeDefinitions.Add(PredefinedTypeDefinitions.Int32.Name, PredefinedTypeDefinitions.Int32);
            globalNamespaceDefinition.TypeDefinitions.Add(PredefinedTypeDefinitions.Int64.Name, PredefinedTypeDefinitions.Int64);

            globalNamespaceDefinition.TypeDefinitions.Add(PredefinedTypeDefinitions.UInt8.Name, PredefinedTypeDefinitions.UInt8);
            globalNamespaceDefinition.TypeDefinitions.Add(PredefinedTypeDefinitions.UInt16.Name, PredefinedTypeDefinitions.UInt16);
            globalNamespaceDefinition.TypeDefinitions.Add(PredefinedTypeDefinitions.UInt32.Name, PredefinedTypeDefinitions.UInt32);
            globalNamespaceDefinition.TypeDefinitions.Add(PredefinedTypeDefinitions.UInt64.Name, PredefinedTypeDefinitions.UInt64);

            globalNamespaceDefinition.TypeDefinitions.Add(PredefinedTypeDefinitions.Float32.Name, PredefinedTypeDefinitions.Float32);
            globalNamespaceDefinition.TypeDefinitions.Add(PredefinedTypeDefinitions.Float64.Name, PredefinedTypeDefinitions.Float64);

            namespaceDefinitions.Add(globalNamespaceDefinition.Name, globalNamespaceDefinition);

            // TODO: define all namespaces before here

            Stack<TypeDefinition> unfinishedTypeDependencyStack = new();

            // TODO: complete type definitions
            foreach (var namespaceDefinition in namespaceDefinitions.Values)
            {
                foreach (TypeDefinition typeDefinition in namespaceDefinition.TypeDefinitions.Values)
                {
                    CompleteTypeDefinition(typeDefinition);

                    if (unfinishedTypeDependencyStack.Count != 0)
                        throw new Exception("expected stack to be empty");
                }
            }

            void CompleteTypeDefinition(TypeDefinition typeDefinition)
            {
                if (typeDefinition.IsFullyDefined)
                    return;

                if (unfinishedTypeDependencyStack.Contains(typeDefinition))
                {
                    throw new Exception("Possible circular dependency.");
                }

                unfinishedTypeDependencyStack.Push(typeDefinition);

                for (int i = 0; i < typeDefinition.TypeFieldDefinitions.Length; i++)
                {
                    var typeField = typeDefinition.TypeFieldDefinitions[i];

                    // TODO: have type inclusion ambiguity solved before here
                    var referenceTypeName = typeField.FieldType.Name;
                    var referenceNamespaceName = "__global__"; // TODO: this as well

                    if (!namespaceDefinitions.TryGetValue(referenceNamespaceName, out var referenceNamespace))
                        throw new Exception("Namespace not found.");

                    if (!referenceNamespace.TypeDefinitions.TryGetValue(referenceTypeName, out var referenceTypeDefinition))
                        throw new Exception("Type not found.");

                    CompleteTypeDefinition(referenceTypeDefinition);

                    // dependency type should be complete now.

                    // TODO: is custom offsets something desired?
                    // TODO: does this work? make better :^)
                    typeDefinition.TypeFieldDefinitions[i] = typeField with
                    {
                        OffsetInBytes = typeDefinition.SizeInBytes
                    };
                    typeDefinition.SizeInBytes += referenceTypeDefinition.SizeInBytes;

                }

                typeDefinition.IsFullyDefined = true;
                if (unfinishedTypeDependencyStack.Pop() != typeDefinition)
                    throw new Exception("Popped type definition did not match popped type.");
            }

            // TODO: syntax tree formation

            // TODO: syntax tree to opcodes

            // TODO: actually return compiled result
            return CompileResult.FromSuccess(Array.Empty<byte>());
        }
    }

    internal sealed record NamespaceDefinition(string Name)
    {
        public Dictionary<string, TypeDefinition> TypeDefinitions = new();
    }
}
