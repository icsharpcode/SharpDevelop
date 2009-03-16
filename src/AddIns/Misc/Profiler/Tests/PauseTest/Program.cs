using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PauseTest
{
	class Program
	{
		static void Main(string[] args)
		{
			while (true)
			{
				Random r = new Random();

				Console.WriteLine("Running!");
				Thread.Sleep(100);

				for (int i = 0; i < r.Next(1, 200); i++)
				{
					Console.WriteLine("i: " + i);
				}

				if (Console.ReadKey().Key == ConsoleKey.Escape)
					return;
			}
		}
	}
}
