// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Benchmark
{
	class Program
	{
		static bool automated;
		static StreamWriter automationOutput;
		
		public static void Main(string[] args)
		{
			automated = args.Length == 1 && args[0] == "/automated";
			if (automated) {
				automationOutput = new StreamWriter("benchmark.out");
			}
			
			Stopwatch w = new Stopwatch();
			w.Start();
			
			Test(SwapAndVirtualCalls);
			Test(BadRecursion);
			Test(QuickSort);
			Test(FloatCalculation);
			
			w.Stop();
			WriteLine("Total tests time: " + FormatTime(totalTime.TotalMilliseconds));
			WriteLine("Total time: " + FormatTime(w.Elapsed.TotalMilliseconds));
			
			if (automated) {
				automationOutput.Close();
			} else {
				Console.Write("Press any key to continue . . . ");
				Console.ReadKey(true);
			}
		}
		
		static void WriteLine(string text)
		{
			Console.WriteLine(text);
			if (automated)
				automationOutput.WriteLine(text);
		}
		
		static TimeSpan totalTime = TimeSpan.Zero;
		
		static void Test(Action action)
		{
			Stopwatch w = new Stopwatch();
			w.Start();
			action();
			w.Stop();
			WriteLine(action.Method.Name + ": " + FormatTime(w.Elapsed.TotalMilliseconds));
			totalTime += w.Elapsed;
		}
		
		static string FormatTime(double milliseconds)
		{
			return milliseconds.ToString("f6", System.Globalization.NumberFormatInfo.InvariantInfo) + "ms";
		}
		
		#region SwapAndVirtualCalls
		static void SwapAndVirtualCalls()
		{
			BaseClass a = new DerivedClass1();
			BaseClass b = new DerivedClass2();
			for (int i = 0; i < 50000; i++) {
				a.Test();
				Swap(ref a, ref b);
			}
		}
		
		[MethodImpl(MethodImplOptions.NoInlining)]
		static void Swap<T>(ref T a, ref T b)
		{
			T t = a;
			a = b;
			b = t;
		}
		
		public abstract class BaseClass
		{
			public abstract void Test();
		}
		
		public class DerivedClass1 : BaseClass
		{
			public override void Test()
			{
			}
		}
		
		public class DerivedClass2 : BaseClass
		{
			public override void Test()
			{
			}
		}
		#endregion
		
		#region BadRecursion
		static void BadRecursion()
		{
			BadRecursion_A(17);
		}
		
		[MethodImpl(MethodImplOptions.NoInlining)]
		static void BadRecursion_A(int level)
		{
			if (level == 0) return;
			BadRecursion_A(level - 1);
			BadRecursion_B(level - 1);
		}
		
		[MethodImpl(MethodImplOptions.NoInlining)]
		static void BadRecursion_B(int level)
		{
			if (level == 0) return;
			BadRecursion_A(level - 1);
			BadRecursion_B(level - 1);
		}
		#endregion
		
		#region QuickSort
		static void QuickSort()
		{
			int[] data = new int[100000];
			for (int i = 0; i < data.Length; i++) {
				data[i] = unchecked( i * 165185 );
			}
			new QuickSortHelper(data).Sort();
		}
		
		public class QuickSortHelper
		{
			int[] array;
			
			public QuickSortHelper(int[] array)
			{
				this.array = array;
			}
			
			public void Sort()
			{
				Sort(0, this.array.Length - 1);
			}
			
			int Partition(int left, int right)
			{
				int value = this.array[right];
				int i = left, j = right - 1;
				
				do
				{
					while (i < right && this.array[i] <= value)
						i++;
					
					while (j > left && this.array[j] >= value)
						j--;
					if (i < j) {
						int help = this.array[i];
						this.array[i] = this.array[j];
						this.array[j] = help;
					}
				} while (i < j);
				
				if (this.array[i] > value) {
					int help = this.array[right];
					this.array[right] = this.array[i];
					this.array[i] = help;
				}
				
				return i;
			}
			
			[MethodImpl(MethodImplOptions.NoInlining)]
			void Sort(int left, int right)
			{
				if (left < right) {
					int i = Partition(left, right);
					Sort(left, i - 1);
					Sort(i + 1, right);
				}
			}
		}
		#endregion
		
		#region FloatCalculation
		[MethodImpl(MethodImplOptions.NoInlining)]
		static void FloatCalculation()
		{
			double c = 0;
			double sum = 0;
			for (int i = 0; i < 10000; i++) {
				sum += GetNextDouble(ref c);
			}
			if (sum != 50005000)
				throw new Exception("Calculation failed!");
		}
		
		
		[MethodImpl(MethodImplOptions.NoInlining)]
		static double GetNextDouble(ref double counter)
		{
			return ++counter;
		}
		#endregion
	}
}