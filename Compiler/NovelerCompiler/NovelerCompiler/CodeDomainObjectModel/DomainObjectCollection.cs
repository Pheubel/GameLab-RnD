using System.Collections;

namespace Noveler.Compiler.CodeDomainObjectModel
{
	internal abstract record DomainObjectCollection<T> : DomainObject, IList<T>
		where T : DomainObject
	{
		private readonly List<T> _values = new();

		public sealed override IReadOnlyList<DomainObject> GetChildren() => _values;

		public T this[int index] { get => _values[index]; set => _values[index] = value; }

		public int Count => _values.Count;

		public bool IsReadOnly => false;

		public void Add(T item)
		{
			_values.Add(item);
		}

		public void AddRange(IEnumerable<T> items)
		{
			_values.AddRange(items);
		}

		public void Clear()
		{
			_values.Clear();
		}

		public bool Contains(T item)
		{
			return _values.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_values.CopyTo(array, arrayIndex);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _values.GetEnumerator();
		}

		public int IndexOf(T item)
		{
			return _values.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			_values.Insert(index, item);
		}

		public bool Remove(T item)
		{
			return _values.Remove(item);
		}

		public void RemoveAt(int index)
		{
			_values.RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _values.GetEnumerator();
		}
	}
}
