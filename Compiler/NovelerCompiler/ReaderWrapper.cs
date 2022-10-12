namespace NovelerCompiler
{
    /// <summary>
    /// A wrapper over TextReader to have proper Peek() behavior.
    /// </summary>
    internal class ReaderWrapper
    {
        readonly TextReader _reader;
        int? _buffer;

        public ReaderWrapper(TextReader reader)
        {
            _reader = reader;
        }

        public int Read()
        {
            if (_buffer == null)
                return _reader.Read();

            int val = _buffer.Value;
            _buffer = null;
            return val;
        }

        public int Peek()
        {
            if (_buffer != null)
                return _buffer.Value;

            _buffer = _reader.Read();
            return _buffer.Value;
        }

        public char ReadChar() => 
            (char)Read();

        public char PeekChar() =>
            (char)Peek();
    }
}
