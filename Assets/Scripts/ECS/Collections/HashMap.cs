using System.Collections.Generic;

namespace ECS.Collections
{
	public class HashMap<T> where T : struct
	{
		readonly int m_BucketSize;
		readonly List<Element[]> m_Buckets;

		public HashMap(int bucketSize)
		{
			m_BucketSize = bucketSize;
			m_Buckets = new List<Element[]>();
		}

		public void Set(int key, T value)
		{
			var bucketIndex = key / m_BucketSize;

			if (bucketIndex >= m_Buckets.Count)
				while (m_Buckets.Count <= bucketIndex)
					m_Buckets.Add(default);

			if (m_Buckets[bucketIndex] == null)
				m_Buckets[bucketIndex] = new Element[m_BucketSize];

			var elementIndex = key - bucketIndex * m_BucketSize;

			var element = m_Buckets[bucketIndex][elementIndex];
			element.Value = value;
			element.IsValid = true;
			m_Buckets[bucketIndex][elementIndex] = element;
		}

		public bool Contains(int key)
		{
			var bucketIndex = key / m_BucketSize;
			return m_Buckets[bucketIndex][key - bucketIndex * m_BucketSize].IsValid;
		}

		public bool TryGetValue(int key, out T value)
		{
			var bucketIndex = key / m_BucketSize;
			var element = m_Buckets[bucketIndex][key - bucketIndex * m_BucketSize];
			value = element.Value;
			return element.IsValid;
		}

		public void Remove(int key)
		{
			var bucketIndex = key / m_BucketSize;
			var elementIndex = key - bucketIndex * m_BucketSize;

			var element = m_Buckets[bucketIndex][elementIndex];
			element.IsValid = false;
			m_Buckets[bucketIndex][elementIndex] = element;
		}

		struct Element
		{
			public bool IsValid;
			public T Value;
		}
	}
}
