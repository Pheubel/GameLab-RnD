using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Noveler.Compiler
{
    internal class SequenceLookUpTable<T> : IReadOnlyList<T>
    {
        private readonly Dictionary<string, T> _dictionary;
        private readonly List<T> _list;

        public SequenceLookUpTable() : this(4) { }

        public SequenceLookUpTable(int startSize)
        {
            _dictionary = new Dictionary<string, T>(startSize);
            _list = new List<T>(startSize);
        }

        public int Count => _list.Count;

        public int Add(string key, T item)
        {
            if (!TryAdd(key, item, out int index))
                throw new ArgumentException($"Item with key \"{key}\" already exists.");

            return index;
        }

        public bool TryAdd(string key, T item, out int index)
        {
            if (_dictionary.TryAdd(key, item))
            {
                index = _list.Count;
                _list.Add(item);
                return true;
            }

            index = -1;
            return false;
        }

        public T Get(string key)
        {
            if (!_dictionary.TryGetValue(key, out T? item))
                throw new ArgumentException($"Item with key \"{key}\" not found.");

            return item;
        }

        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public bool TryGet(string key, [MaybeNullWhen(false)] out T item)
        {
            return _dictionary.TryGetValue(key, out item);
        }

        public T Get(int index)
        {
            if ((uint)index >= (uint)_list.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            return _list[index];
        }

        public bool TryGet(int index, [MaybeNullWhen(false)] out T item)
        {
            if ((uint)index >= (uint)_list.Count)
            {
                item = default;
                return false;
            }

            item = _list[index];
            return true;
        }

        public int GetIndex(string key)
        {
            if (!_dictionary.TryGetValue(key, out var item))
                return -1;

            return _list.IndexOf(item);
        }

        public T this[string key]
        {
            get => Get(key);
        }

        public T this[int index]
        {
            [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
            get => Get(index);
        }

        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}