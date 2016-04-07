using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TD
{
    public class Slice<T, TList> : IList<T> where TList : IList<T>
    {
        public TList List { get; private set; }
        public int StartIndex { get; private set; }
        public int Count { get; private set; }
        public bool IsReadOnly => false;

        public T this[int index]
        {
            get
            {
                return List[index + StartIndex];
            }

            set
            {
                List[index + StartIndex] = value;
            }
        }

        public IEnumerator<T> GetEnumerator() => List.Skip(StartIndex).Take(Count).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int IndexOf(T item)
        {
            foreach (var i in Enumerable.Range(0, Count))
            {
                if (this[i].Equals(item)) return i;
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            throw new InvalidOperationException($"Cannot add to or remove from this list.");
        }

        public void RemoveAt(int index)
        {
            throw new InvalidOperationException($"Cannot add to or remove from this list.");
        }

        public void Add(T item)
        {
            throw new InvalidOperationException($"Cannot add to or remove from this list.");
        }

        public void Clear()
        {
            throw new InvalidOperationException($"Cannot add to or remove from this list.");
        }

        public bool Contains(T item)
        {
            return this.Any(thisItem => thisItem.Equals(item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            var max = Math.Min(array.Length - arrayIndex, Count);

            foreach (var i in Enumerable.Range(0, max))
            {
                array[arrayIndex + i] = this[i];
            }
        }

        public bool Remove(T item)
        {
            throw new InvalidOperationException($"Cannot add to or remove from this list.");
        }

        public Slice(TList list, int startIndex, int count)
        {
            List = list;
            StartIndex = startIndex;
            Count = count;
        }

        public Slice(TList list, int startIndex) : this(list, startIndex, list.Count - startIndex)
        {

        }

        public Slice(Slice<T, TList> slice, int startIndex, int count) : 
            this(slice.List, slice.StartIndex + startIndex, count)
        {

        }

        public Slice(Slice<T, TList> slice, int startIndex) : this(slice.List, slice.StartIndex + startIndex)
        {

        }
    }

    public class ArraySlice<T> : Slice<T, T[]>
    {
        public ArraySlice(T[] array, int startIndex, int count) : base(array, startIndex, count) { }

        public ArraySlice(T[] array, int startIndex) : base(array, startIndex) { }
    }
}
