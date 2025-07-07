using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Common.MyCollections
{
    public class MyHashSet<T>
    {
        private readonly ConcurrentDictionary<T, byte> _dict = new();

        /// <summary>
        /// Thêm phần tử vào tập hợp. Trả về true nếu phần tử được thêm thành công (chưa tồn tại).
        /// </summary>
        public bool Add(T item) => _dict.TryAdd(item, 0);

        /// <summary>
        /// Xoá phần tử khỏi tập hợp. Trả về true nếu phần tử tồn tại và đã được xoá.
        /// </summary>
        public bool Remove(T item) => _dict.TryRemove(item, out _);

        /// <summary>
        /// Kiểm tra phần tử có tồn tại trong tập hợp không.
        /// </summary>
        public bool Contains(T item) => _dict.ContainsKey(item);

        /// <summary>
        /// Lấy toàn bộ phần tử dưới dạng IEnumerable<T>.
        /// </summary>
        public IEnumerable<T> Items => _dict.Keys;

        /// <summary>
        /// Đếm số phần tử trong tập hợp.
        /// </summary>
        public int Count => _dict.Count;

        /// <summary>
        /// Xoá toàn bộ tập hợp.
        /// </summary>
        public void Clear() => _dict.Clear();
    }
}
