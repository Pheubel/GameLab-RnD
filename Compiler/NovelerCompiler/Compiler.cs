using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Globalization;
using NovelerCompiler;
using System.CommandLine.Parsing;

namespace Noveler.Compiler
{
    public class Compiler
    {
        // TODO use TextReader to use streams instead in the future?
        public static bool Compile(TextReader script, out List<byte> result, out IReadOnlyList<CompilerMessage> messages)
        {
            var reader = new ReaderWrapper(script);

            var outMessages = new List<CompilerMessage>();
            messages = outMessages;

            Dictionary<string, SymbolTableEntry> variableTable = new Dictionary<string, SymbolTableEntry>();
            ReadingContext context = new ReadingContext(variableTable, 1, 1, null, 0, outMessages);

            var tokenStream = Lexer.Lex(reader, ref context);
            //var tree = SyntaxAnalyzer.Analyze()

            //result = EmitTree(tree, outMessages);

            result = new List<byte>();

            return true;
        }


        private static List<byte> EmitTree(SyntaxTree tree, List<CompilerMessage> outMessages)
        {
            List<byte> result;

            if (!tree.IsValid())
            {
                return new List<byte>(0);
            }

            result = new List<byte>(2048);

            EmitCode(tree.Root);

            return result;
        }

        private static byte EmitCode(TreeNode node)
        {
            switch (node.Token.Type)
            {
                case TokenType.InvalidToken:
                    break;
                case TokenType.IntLiteral:
                    Console.WriteLine($"LoadConst32 R0 {node.Token.ValueString:X}");
                    break;
                case TokenType.LongLiteral:
                    Console.WriteLine($"LoadConst64 R0 {node.Token.ValueString:X}");
                    break;
                case TokenType.FloatLiteral:
                    break;
                case TokenType.DoubleLiteral:
                    break;

                case TokenType.Add:
                    EmitCode(node.Children[0]);
                    Console.WriteLine("PUSH R1");
                    EmitCode(node.Children[1]);
                    Console.WriteLine("MOVE R2 R1");
                    Console.WriteLine("POP R1");
                    Console.WriteLine("ADD R1 R2");
                    break;

                case TokenType.Subtract:
                    EmitCode(node.Children[0]);
                    Console.WriteLine("PUSH R1");
                    EmitCode(node.Children[1]);
                    Console.WriteLine("MOVE R2 R1");
                    Console.WriteLine("POP R1");
                    Console.WriteLine("SUBTRACT R1 R2");
                    break;

                case TokenType.Multiply:
                    EmitCode(node.Children[0]);
                    Console.WriteLine("PUSH R1");
                    EmitCode(node.Children[2]);
                    Console.WriteLine("MULTIPLY R1 R2");
                    break;

                case TokenType.Divide:
                    EmitCode(node.Children[0]);
                    Console.WriteLine("PUSH R1");
                    EmitCode(node.Children[2]);
                    Console.WriteLine("DIVREM R1 R2");
                    break;

                case TokenType.Assign:
                    break;
                case TokenType.AddAssign:
                    break;
                case TokenType.SubtractAssign:
                    break;
                case TokenType.EqualsTo:
                    break;
                case TokenType.Increment:
                    break;
                case TokenType.Decrement:
                    break;
                case TokenType.FunctionName:
                    break;
                case TokenType.OpenFunction:
                    break;
                case TokenType.CloseFunction:
                    break;
                case TokenType.SemiColon:
                    break;
                case TokenType.ClosingCurlyBracket:
                    break;
                case TokenType.Identifier:
                    break;
                case TokenType.Root:
                    EmitCode(node.Children[0]);
                    break;
                case TokenType.EndOfLine:
                    break;
                case TokenType.EndOfFile:
                    break;
                case TokenType.Negate:
                    Console.WriteLine("NEGATE R1");
                    break;
                default:
                    break;
            }

            Console.WriteLine(node.Token.ValueType);

            return default;
        }
    }

    enum ReadState
    {
        Story,
        Code,
        EmbeddedCode
    }
}