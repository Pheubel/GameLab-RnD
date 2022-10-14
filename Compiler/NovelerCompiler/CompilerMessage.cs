namespace Noveler.Compiler
{
    public class CompilerMessage
    {
        public string Content;
        public MessageCode Code;
        public string Source;

        public CompilerMessage(string content, MessageCode code, string source)
        {
            Content = content;
            Code = code;
            Source = source;
        }

        internal CompilerMessage(string content, MessageCode code, ref ReadingContext context)
        {
            Content = content;
            Code = code;
            Source = $"{(context.CurrentFile is null ? string.Empty : context.CurrentFile + ": ")} Line: {context.LineNumber} Column: {context.CharacterOnLine}";
        }

        public enum MessageCode
        {
            // messages
            Succes,

            // warnings
            Foo,

            // errors
            InvalidToken,
            InvalidLiteral,
            ReservedKeyword,
            RedeclaredVariable,
            UndefinedVariable,
            MissingType
        }

        public enum MessageType
        {
            Unknown,
            Message,
            Warning,
            Error
        }
    }

    public static class CompilerMessageExtensions
    {
        public static CompilerMessage.MessageType GetMessageTypeFromCode(this CompilerMessage.MessageCode code)
        {
            const int FirstMessage = (int)CompilerMessage.MessageCode.Succes;
            const int LastMessage = (int)CompilerMessage.MessageCode.Succes;

            const int FirstWarning = (int)CompilerMessage.MessageCode.Foo;
            const int LastWarning = (int)CompilerMessage.MessageCode.Foo;

            const int FirstError = (int)CompilerMessage.MessageCode.InvalidToken;
            const int LastError = (int)CompilerMessage.MessageCode.InvalidToken;

            return (int)code switch
            {
                >= FirstMessage and <= LastMessage => CompilerMessage.MessageType.Message,
                >= FirstWarning and <= LastWarning => CompilerMessage.MessageType.Warning,
                >= FirstError and <= LastError => CompilerMessage.MessageType.Error,

                _ => CompilerMessage.MessageType.Unknown
            };
        }
    }
}