namespace NovelerCompiler
{
    /// <summary>
    /// A wrapper over TextReader to have proper Peek() behavior.
    /// </summary>
    internal sealed class ReaderWrapper
    {
        readonly private TextReader _reader;
        readonly private int[] _buffer = new int[2];
        private int _bufferCount;

        public ReaderWrapper(TextReader reader)
        {
            _reader = reader;
        }

        public int Read()
        {
            if (_bufferCount == 0)
                return _reader.Read();

            _bufferCount--;
            int val = _buffer[0];
            _buffer[0] = _buffer[1];
            return val;
        }

        public int Peek()
        {
            if (_bufferCount >= 1)
                return _buffer[0];

            _bufferCount = 1;
            _buffer[0] = _reader.Read();
            return _buffer[0];
        }

        public int PeekSecond()
        {
            if (_bufferCount >= 2)
                return _buffer[1];

            if (_bufferCount == 1)
            {
                _buffer[1] = _reader.Read();
            }
            else
            {
                _buffer[0] = _reader.Read();
                _buffer[1] = _reader.Read();
            }

            _bufferCount = 2;
            return _buffer[1];
        }

        public char ReadChar() =>
            (char)Read();

        public char PeekChar() =>
            (char)Peek();

        public char PeekSecondChar() =>
            (char)PeekSecond();
    }
}
