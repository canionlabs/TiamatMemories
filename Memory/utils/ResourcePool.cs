using System.Collections.Generic;

namespace Memory.utils
{
	public class ResourcePool<T> where T : class, IDataResource, new()
	{
		// ========= PUBLIC MEMBERS ====================================================================================

		public int TotalAllocated => _resources.Count;

		// ========= PRIVATE MEMBERS ===================================================================================

		readonly List<T> _resources;

		// ========= PUBLIC METHODS ====================================================================================

		public ResourcePool(int size = 50)
		{
			_resources = new List<T>(size);
		}

        public List<T> ListResources()
        {
            return _resources;
        }

        /// <summary>
        /// Acquires a resource.
        /// </summary>
        /// <returns>The resource.</returns>
        public T AcquireResource()
		{
			T resource = _resources.Find((item) =>
			{
				return item.IsAvailable;
			});

			if (resource == null)
			{
				resource = new T();
				_resources.Add(resource);
			}

			return resource;
		}

		/// <summary>
		/// Dispose and delete all resources
		/// </summary>
		public void CleanUp()
		{
			foreach (T resource in _resources)
			{
				resource.Dispose();
			}

			_resources.Clear();
		}
	}
}
