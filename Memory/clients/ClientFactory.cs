using System;
using System.Collections.Generic;
using Memory.utils;

namespace Memory.clients
{
	public static class ClientFactory<T> where T : class, IClient, IDataResource, new()
	{
		// ========= PUBLIC MEMBERS ====================================================================================

		public static int Total => _pool.TotalAllocated;

		// ========= PRIVATE MEMBERS ====================================================================================

		static readonly ResourcePool<T> _pool;

		static ClientFactory()
		{
			_pool = new ResourcePool<T>(30);
		}

		// ========= PUBLIC METHODS ====================================================================================

		/// <summary>
		/// Returns an instance
		/// </summary>
		/// <returns>The client.</returns>
		public static T BuildClient(string host, int port)
		{
			T client = _pool.AcquireResource();

			if (client != null)
			{
				client.Setup(host, port);
			}
			else
			{
				throw new Exception("Impossible to Acquire Resource");
			}

			return client;
		}

		public static void CleanUp()
		{
			_pool.CleanUp();
		}
	}
}
