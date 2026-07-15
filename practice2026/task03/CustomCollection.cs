using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace task03
{
    public class CustomCollection<T> : IEnumerable<T>
    {
        private readonly List<T> _items = new();

        public void Add(T item) => _items.Add(item);

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerable<T> GetReverseEnumerator()
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                yield return _items[i];
            }
        }

        public static IEnumerable<int> GenerateSequence(int start, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Количество элементов не может быть отрицательным.");
            }

            for (int i = 0; i < count; i++)
            {
                yield return start + i;
            }
        }

        public IEnumerable<T> FilterAndSort(Func<T, bool> predicate, Func<T, IComparable> keySelector)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate), "Предикат фильтрации не может быть null.");
            }

            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector), "Селектор ключа сортировки не может быть null.");
            }

            return _items.Where(predicate).OrderBy(keySelector);
        }
    }
}
