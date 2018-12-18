using System;
using System.Threading;
using Memory.clients;
using Memory.utils;

namespace Memory
{
	class Program
	{
		static MemoryManager memoryManager;

		static volatile bool running = true;

		static void Main(string[] args)
		{
			Console.WriteLine("Starting");
			Console.CancelKeyPress += QuitHandler;

			memoryManager = new MemoryManager();
			memoryManager.Start();

			while (running)
			{
				memoryManager.Service();
				Thread.Sleep(1);
			}

			Console.WriteLine("Bye");
		}

		static void QuitHandler(object sender, ConsoleCancelEventArgs e)
		{
			memoryManager.CleanUp();
			running = false;
		}
	}
}
