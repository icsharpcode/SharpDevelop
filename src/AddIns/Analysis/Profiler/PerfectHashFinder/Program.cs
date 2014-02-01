// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;

namespace PerfectHashFinder
{
	class Program
	{
		public static void Main(string[] args)
		{
			List<int> allocationSizes = new List<int>();
			
			Console.WriteLine("FunctionInfo, 32-bit:");
			allocationSizes.Clear();
			for (int i = 1; i < 27; i++) {
				allocationSizes.Add(4 * (1<<i) + 24);
			}
			Console.WriteLine(FindPerfectHash(allocationSizes.ToArray(), 32));
			
			Console.WriteLine("FunctionInfo, 64-bit:");
			allocationSizes.Clear();
			for (int i = 1; i < 27; i++) {
				allocationSizes.Add(8 * (1<<i) + 24);
			}
			Console.WriteLine(FindPerfectHash(allocationSizes.ToArray(), 32));
			
			Console.WriteLine("Stack, 32-bit:");
			allocationSizes.Clear();
			allocationSizes.Add(3 * 4 + 4); // ThreadLocalData size
			for (int i = 64; i <= 1024 * 1024; i *= 2) {
				allocationSizes.Add(12 * i);
			}
			Console.WriteLine(FindPerfectHash(allocationSizes, 16));
			
			Console.WriteLine("Stack, 64-bit:");
			allocationSizes.Clear();
			allocationSizes.Add(3 * 8 + 4); // ThreadLocalData size
			for (int i = 64; i <= 1024 * 1024; i *= 2) {
				allocationSizes.Add(16 * i);
			}
			Console.WriteLine(FindPerfectHash(allocationSizes, 16));
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
		
		// Tries to find a perfect hash function of form 'f(x) = (a * x) % hashTableSize'.
		static string FindPerfectHash(IEnumerable<int> allocationSizes, int hashTableSize)
		{
			var allocationSizesArray = allocationSizes.Distinct().ToArray();
			if (allocationSizesArray.Length > hashTableSize)
				return "cannot find perfect hash if table is smaller than number of allocation sizes";
			int bitmask = hashTableSize - 1;
			if ((bitmask & hashTableSize) != 0)
				return "Hashtable size " + hashTableSize + " not supported (table sizes must be power of 2)";
			bool[] table = new bool[hashTableSize];
			unchecked {
				uint a = 0;
				for (uint run = 0; run < uint.MaxValue; run++) {
					if ((run & 0x7fffff) == 0x7fffff) {
						Console.Write((100.0 * run / uint.MaxValue).ToString("f1") + "%");
						Console.CursorLeft = 0;
					}
					a += 16247; // do not search linearly, but 'jump around'
					// this tends to find a result much faster because there usually are many possible
					// solutions that are close together.
					// Incrementing by any value that does not contain the factor 2 means we still
					// search all possible integers.
					for (int i = 0; i < table.Length; i++) {
						table[i] = false;
					}
					foreach (int x in allocationSizesArray) {
						int y = (int)((a * (long)x) >> 32) & bitmask;
						if (table[y])
							goto retry;
						else
							table[y] = true;
					}
					Console.WriteLine("100%  ");
					return "f(x) = ((" + a + "L * x) >> 32) % " + hashTableSize + Environment.NewLine
						+ "public:" + Environment.NewLine
						+ "  static const int FreeListSize = " + hashTableSize + ";" + Environment.NewLine
						+ "  static const size_t PossibleAllocationSizes[" + allocationSizesArray.Length + "];" + Environment.NewLine
						+ "  " + Environment.NewLine
						+ "  static inline UINT_PTR allocMappingFunc(size_t bytes)" + Environment.NewLine
						+ "  {" + Environment.NewLine
						+ "    return (UINT_PTR)(((bytes * " + a + "ULL) >> 32) & " + bitmask + ");" + Environment.NewLine
						+ "  }" + Environment.NewLine
						+ "::PossibleAllocationSizes[" + allocationSizesArray.Length + "] = {"
						+ string.Join(", ", allocationSizesArray.Select(s=>s.ToString()).ToArray()) + "};" + Environment.NewLine;
					retry:;
				}
			}
			Console.WriteLine("100%  ");
			return "no hash function of form 'f(x) = ((a * x) >> 32) % " + hashTableSize + "' found";
		}
	}
}
