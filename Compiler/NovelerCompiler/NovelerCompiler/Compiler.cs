using Antlr4.Runtime;
using Noveler.Compiler.Grammar;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noveler.Compiler.CodeDomainObjectModel;
using System.Runtime.InteropServices;
using Noveler.Compiler.CodeDomainObjectModel.Statements;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Build.Utilities;

namespace Noveler.Compiler
{
    internal class DefinitionCollection
    {
        public Dictionary<string, NamespaceDefinition> NamespaceDefinitions = new();
    }

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

            // TODO: fix up type references to appropriate types in appropriate namespaces

            DefinitionCollection definitionCollection = new();

            var globalNamespaceDefinition = new NamespaceDefinition("__global__", null);

            AddBuiltInDefinitions(globalNamespaceDefinition);

            definitionCollection.NamespaceDefinitions.Add(globalNamespaceDefinition.Name, globalNamespaceDefinition);

            List<TypeDefinition> foundTypes = new(32);
            List<FunctionDefinition> foundFunctions = new(32);

            foreach (var unit in compilationUnits.Values)
            {
                foreach (NameSpace @namespace in unit.NameSpaces)
                {
                    // forgive the null entry, as it will be assigned after it's existance has been checked.   
                    ref NamespaceDefinition namespaceDefinitionEntry = ref CollectionsMarshal.GetValueRefOrAddDefault(definitionCollection.NamespaceDefinitions, @namespace.Name, out bool namespaceExists)!;

                    // if the namespace is new, create a new namespace instance
                    if (!namespaceExists)
                    {
                        // TODO: handle parent namespace definition for new instances (currently using placeholder)
                        namespaceDefinitionEntry = new NamespaceDefinition(@namespace.Name, globalNamespaceDefinition);
                    }

                    foreach (var typeDeclaration in @namespace.Types)
                    {
                        if (typeDeclaration.IsGenericType)
                        {
                            // TODO: come up with a plan for handling generic types.
                            // possible what to do: gather all generic types and references to types, then form new ones and add the generated ones.
                            // also do the same for fuctions.
                            continue;
                        }

                        ref TypeDefinition? typeDefinitionEntry = ref CollectionsMarshal.GetValueRefOrAddDefault(namespaceDefinitionEntry.TypeDefinitions, typeDeclaration.Name, out bool typeExists);

                        if (typeExists)
                        {
                            throw new Exception($"Type \"{typeDeclaration.Name}\" has already been declared inside this namespace.");
                        }

                        typeDefinitionEntry = new TypeDefinition(typeDeclaration.Name, namespaceDefinitionEntry, unit, typeDeclaration.SymbolScope, typeDeclaration);

                        if (!@namespace.SymbolScope.TryInsert(typeDefinitionEntry.Name, new TypeSymbolInfo(typeDefinitionEntry.Name, typeDefinitionEntry)))
                        {
                            // TODO: symbol already exists in the current scope, handle this
                            throw new Exception($"Symbol \"{typeDefinitionEntry.Name}\" already exists in this scope.");
                        }

                        foundTypes.Add(typeDefinitionEntry);

                        //TODO: make sure this is finished

                        foreach (var typeFunctionDeclaration in typeDeclaration.TypeMemberFunctions)
                        {
                            var typeFunctionDefinition = new FunctionDefinition(
                                Name: typeFunctionDeclaration.FunctionDeclaration.Name,
                                Namespace: namespaceDefinitionEntry,
                                ParentType: typeDefinitionEntry,
                                OriginalCompilationUnit: unit,
                                SymbolScope: typeFunctionDeclaration.FunctionDeclaration.SymbolScope,
                                OriginalDeclaration: typeFunctionDeclaration.FunctionDeclaration
                                );

                            if (!typeDefinitionEntry.SymbolScope!.TryInsert(typeFunctionDefinition.Name, new FunctionSymbolInfo(typeFunctionDefinition.Name, typeFunctionDefinition)))
                            {
                                // TODO: symbol already exists in the current scope, handle this
                                throw new Exception($"Symbol \"{typeFunctionDefinition.Name}\" already exists in this scope.");
                            }

                            foundFunctions.Add(typeFunctionDefinition);
                            // delay adding the function declaration to the type's function dictionary
                        }

                        foreach (var constructor in typeDeclaration.TypeConstructors)
                        {
                            // TODO: handle this.
                        }
                    }

                    // TODO: do function declarations
                    foreach (var functionDeclaration in @namespace.Functions)
                    {
                        var functionDefinition = new FunctionDefinition(
                             Name: functionDeclaration.Name,
                                Namespace: namespaceDefinitionEntry!,
                                ParentType: null,
                                OriginalCompilationUnit: unit,
                                SymbolScope: functionDeclaration.SymbolScope,
                                OriginalDeclaration: functionDeclaration
                            );

                        if (!@namespace.SymbolScope.TryInsert(functionDefinition.Name, new FunctionSymbolInfo(functionDeclaration.Name, functionDefinition)))
                        {
                            // TODO: symbol already exists in the current scope, handle this
                            throw new Exception($"Symbol \"{functionDefinition.Name}\" already exists in this scope.");
                        }

                        foundFunctions.Add(functionDefinition);
                    }
                }
            }

            // TODO: define all namespaces before here

            // TODO: maybe have all generic types and functions ready before here?

            Stack<TypeDefinition> unfinishedTypeDependencyStack = new();

            // completes the prepared types that are user made
            foreach (TypeDefinition typeDefinition in foundTypes)
            {
                CompleteTypeDefinition(typeDefinition);

                if (unfinishedTypeDependencyStack.Count != 0)
                    throw new Exception("expected stack to be empty");
            }

            // completes the functions that are user made
            foreach (FunctionDefinition functionDefinition in foundFunctions)
            {
                CompleteFunctionDefinition(functionDefinition);
            }

            TypeDefinition GetReferencedType(TypeReference typeReference)
            {
                // TODO: fix this up to support namespaces (outside the global namespace), add namespaces to look through as argument? 

                var referenceTypeName = typeReference.Name;
                var referenceNamespaceName = "__global__";

                if (!definitionCollection.NamespaceDefinitions.TryGetValue(referenceNamespaceName, out var referenceNamespace))
                    throw new Exception("Namespace not found.");

                if (!referenceNamespace.TypeDefinitions.TryGetValue(referenceTypeName, out var referenceTypeDefinition))
                    throw new Exception("Type not found.");

                // TODO: also make sure that exactly 1 definition has been found, otherwise report error.

                return referenceTypeDefinition;
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

                var highestTypeAllignment = NaturalAlignment.Byte;

                // original declaration should not be null for non built in types
                foreach (var fieldDeclaration in typeDefinition.OriginalDeclaration!.TypeFieldMembers)
                {
                    var referencedTypeDefinition = GetReferencedType(fieldDeclaration.FieldDeclarationStatement.TypeReference);

                    CompleteTypeDefinition(referencedTypeDefinition);

                    if ((int)highestTypeAllignment < (int)referencedTypeDefinition.Alignment)
                        highestTypeAllignment = referencedTypeDefinition.Alignment;

                    int alignmentValue = 1 << (int)referencedTypeDefinition.Alignment;
                    int mask = alignmentValue - 1;
                    // int spill = typeDefinition.SizeInBytes & mask;
                    // int padding = (alignmentValue - spill) % alignmentValue;

                    int padding  = (~typeDefinition.SizeInBytes + 1) & mask;

                    int offset = typeDefinition.SizeInBytes + padding;

                    var typeFieldDefinition = new TypeFieldDefinition(
                            FieldName: fieldDeclaration.FieldDeclarationStatement.VariableName,
                            FieldType: referencedTypeDefinition,
                            OffsetInBytes: offset,
                            InitializationExpression: fieldDeclaration.FieldDeclarationStatement.HasInitializationExpression
                                ? ((VariableDeclarationAssignmentStatement)fieldDeclaration.FieldDeclarationStatement).InitializationExpression
                                : null
                            );

                    // TODO: is custom offsets something desired? if so, all fields need to have an offset set and needs different branch
                    // TODO: does this work? make better :^)
                    if (!typeDefinition.TypeFieldDefinitions.TryAdd(
                        key: fieldDeclaration.FieldDeclarationStatement.VariableName,
                        item: typeFieldDefinition,
                        index: out _))
                    {
                        throw new Exception("field already exists.");
                    }

                    if (!typeDefinition.SymbolScope!.TryInsert(typeFieldDefinition.FieldName, new TypeFieldSymbolInfo(typeFieldDefinition.FieldName, typeFieldDefinition)))
                    {
                        // TODO: symbol already exists in the current scope, handle this
                        throw new Exception($"Symbol \"{typeFieldDefinition.FieldName}\" already exists in this scope.");
                    }

                    typeDefinition.SizeInBytes = offset + referencedTypeDefinition.SizeInBytes;
                }

                // do padding at the end of the structure
                var typeAlignmentValue = 1 << (int)highestTypeAllignment;
                var typeMask = typeAlignmentValue - 1;
                // int typeSpill = typeDefinition.SizeInBytes & typeMask;
                // int typePadding = (typeAlignmentValue - typeSpill) % typeAlignmentValue;

                int typePadding = (~typeDefinition.SizeInBytes + 1) & typeMask;

                typeDefinition.SizeInBytes += typePadding;

                typeDefinition.Alignment = highestTypeAllignment;
                typeDefinition.IsFullyDefined = true;
                if (unfinishedTypeDependencyStack.Pop() != typeDefinition)
                    throw new Exception("Popped type definition did not match popped type.");
            }

            void CompleteFunctionDefinition(FunctionDefinition functionDefinition)
            {
                foreach (var parameter in functionDefinition.OriginalDeclaration.Parameters)
                {
                    var functionArgumentType = GetReferencedType(parameter.ParameterType);
                    FunctionArgumentDefinition functionArgument = new(parameter.Name, functionArgumentType);

                    if (!functionDefinition.FunctionArguments.TryAdd(parameter.Name, functionArgument, out functionArgument.ArgumentIndex))
                        throw new Exception("Duplicate function argument name");

                    if (!functionDefinition.SymbolScope.TryInsert(functionArgument.Name, new FunctionArgumentSymbolInfo(parameter.Name, functionArgument)))
                    {
                        // TODO: symbol already exists in the current scope, handle this
                        throw new Exception($"Symbol \"{functionArgument.Name}\" already exists in this scope.");
                    }
                }

                functionDefinition.ReturnType = GetReferencedType(functionDefinition.OriginalDeclaration.ReturnType);

                functionDefinition.IsFullyDefined = true;
            }

            // TODO: create symbol table to look up variables for scopes

            // TODO: syntax tree formation

            // am i actually making a syntax tree? i already have a lot of info from the function declaration statements
            void CreateFold(FunctionDefinition functionDefinition)
            {
                var functionBodyScope = functionDefinition.SymbolScope.CreateChildScope();

                var statements = functionDefinition.OriginalDeclaration.FunctionBodyDeclaration.Statements;

                foreach (Statement statement in statements)
                {
                    statement.CreateSyntaxTreeNode(definitionCollection, functionBodyScope)
                }
            }

            // TODO: syntax tree to opcodes

            // TODO: actually return compiled result
            return CompileResult.FromSuccess(Array.Empty<byte>());
        }

        private static void AddBuiltInDefinitions(NamespaceDefinition globalNamespaceDefinition)
        {
            TypeDefinition Int8 = new TypeDefinition("Int8", globalNamespaceDefinition, 1, NaturalAlignment.Byte) { StructureType = StructureType.Int8 };
            TypeDefinition Int16 = new TypeDefinition("Int16", globalNamespaceDefinition, 2, NaturalAlignment.Short) { StructureType = StructureType.Int16 };
            TypeDefinition Int32 = new TypeDefinition("Int32", globalNamespaceDefinition, 4, NaturalAlignment.Int) { StructureType = StructureType.Int32 };
            TypeDefinition Int64 = new TypeDefinition("Int64", globalNamespaceDefinition, 8, NaturalAlignment.Long) { StructureType = StructureType.Int64 };

            TypeDefinition UInt8 = new TypeDefinition("UInt8", globalNamespaceDefinition, 1, NaturalAlignment.Byte) { StructureType = StructureType.UInt8 };
            TypeDefinition UInt16 = new TypeDefinition("UInt16", globalNamespaceDefinition, 2, NaturalAlignment.Short) { StructureType = StructureType.UInt8 };
            TypeDefinition UInt32 = new TypeDefinition("UInt32", globalNamespaceDefinition, 4, NaturalAlignment.Int) { StructureType = StructureType.UInt8 };
            TypeDefinition UInt64 = new TypeDefinition("UInt64", globalNamespaceDefinition, 8, NaturalAlignment.Long) { StructureType = StructureType.UInt8 };

            TypeDefinition Float32 = new TypeDefinition("Float32", globalNamespaceDefinition, 4, NaturalAlignment.Int) { StructureType = StructureType.Float32 };
            TypeDefinition Float64 = new TypeDefinition("Float64", globalNamespaceDefinition, 8, NaturalAlignment.Long) { StructureType = StructureType.Float64 };

            TypeDefinition Unit = new TypeDefinition("Unit", globalNamespaceDefinition, 0, NaturalAlignment.Byte) { StructureType = StructureType.Unit };

            globalNamespaceDefinition.TypeDefinitions.Add(Int8.Name, Int8);
            globalNamespaceDefinition.TypeDefinitions.Add(Int16.Name, Int16);
            globalNamespaceDefinition.TypeDefinitions.Add(Int32.Name, Int32);
            globalNamespaceDefinition.TypeDefinitions.Add(Int64.Name, Int64);

            globalNamespaceDefinition.TypeDefinitions.Add(UInt8.Name, UInt8);
            globalNamespaceDefinition.TypeDefinitions.Add(UInt16.Name, UInt16);
            globalNamespaceDefinition.TypeDefinitions.Add(UInt32.Name, UInt32);
            globalNamespaceDefinition.TypeDefinitions.Add(UInt64.Name, UInt64);

            globalNamespaceDefinition.TypeDefinitions.Add(Float32.Name, Float32);
            globalNamespaceDefinition.TypeDefinitions.Add(Float64.Name, Float64);

            globalNamespaceDefinition.TypeDefinitions.Add(Unit.Name, Unit);
        }
    }
}
