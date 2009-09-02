// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Profiler.Controller.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using ICSharpCode.Profiler.Controller;

namespace BenchmarkRunner
{
	class Program
	{
		const int TestRunCount = 5;
		
		public static void Main(string[] args)
		{
			ProcessStartInfo benchmarkStartInfo = new ProcessStartInfo("Benchmark.exe", "/automated");
			benchmarkStartInfo.UseShellExecute = false;
			
			ProcessStartInfo benchmark32StartInfo = new ProcessStartInfo("Benchmark32.exe", "/automated");
			benchmark32StartInfo.UseShellExecute = false;
			
			List<TestSeries> testSeries = new List<TestSeries>();
			if (File.Exists(benchmark32StartInfo.FileName)) {
				testSeries.Add(new TestSeries("Without profiler (32-bit)", () => WithoutProfiler(benchmark32StartInfo)));
				testSeries.Add(new TestSeries("With profiler (32-bit)", () => WithProfiler(benchmark32StartInfo)));
			} else {
				Console.WriteLine("32-bit tests not executed: could not find Benchmark32.exe");
			}
			
			if (IntPtr.Size == 8) {
				// in 64-bit mode, also test 64-bit profiler
				if (File.Exists(benchmarkStartInfo.FileName)) {
					testSeries.Add(new TestSeries("Without profiler (64-bit)", () => WithoutProfiler(benchmarkStartInfo)));
					testSeries.Add(new TestSeries("With profiler (64-bit)", () => WithProfiler(benchmarkStartInfo)));
				} else {
					Console.WriteLine("64-bit tests not executed: could not find Benchmark.exe");
				}
			}
			
			for (int i = 0; i < TestRunCount; i++) {
				foreach (TestSeries ts in testSeries) {
					ts.ExecuteRun();
				}
			}
			ResultSet[] results = testSeries.Select(ts=>ts.GetResult()).ToArray();
			
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine();
			
			DisplayResults(results);
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
		
		class ProfilingResults
		{
			public string DatabaseSize;
		}
		
		static ProfilingResults WithoutProfiler(ProcessStartInfo startInfo)
		{
			Process p = Process.Start(startInfo);
			p.WaitForExit();
			return new ProfilingResults {
				DatabaseSize = FormatBytes(0)
			};
		}
		
		static ProfilingResults WithProfiler(ProcessStartInfo startInfo)
		{
			string fileName = Path.Combine(Path.GetDirectoryName(typeof(Profiler).Assembly.Location), "output.sdps");
			if (File.Exists(fileName))
				File.Delete(fileName);
			
			using (var profiler = new Profiler(
				startInfo, new ProfilingDataSQLiteWriter(fileName, false, null), new ProfilerOptions()
			)) {
				using (ManualResetEvent mre = new ManualResetEvent(false)) {
					profiler.SessionEnded += delegate {
						mre.Set();
					};
					profiler.Start();
					mre.WaitOne();
				}
			}
			return new ProfilingResults {
				DatabaseSize = FormatBytes(new FileInfo(fileName).Length)
			};
		}
		
		class TestSeries
		{
			string name;
			Func<ProfilingResults> runTestSet;
			List<List<Measurement>> measurements = new List<List<Measurement>>();
			
			public TestSeries(string name, Func<ProfilingResults> runTestSet)
			{
				this.name = name;
				this.runTestSet = runTestSet;
			}
			
			public void ExecuteRun()
			{
				Thread.Sleep(100);
				Console.WriteLine("Testset '" + name + "', run " + (measurements.Count+1));
				Stopwatch watch = new Stopwatch();
				watch.Start();
				ProfilingResults results = runTestSet();
				watch.Stop();
				List<string> content = new List<string>();
				content.Add("Total external time: " + FormatTime(watch.Elapsed.TotalMilliseconds));
				foreach (var field in typeof(ProfilingResults).GetFields()) {
					string value = field.GetValue(results) as string;
					if (value != null)
						content.Add(field.Name + ": " + value);
				}
				content.ForEach(Console.WriteLine);
				measurements.Add(File.ReadAllLines("benchmark.out").Concat(content).Select(line=>Measurement.Parse(line)).ToList());
			}
			
			public ResultSet GetResult()
			{
				Measurement[][] mSwapped = new Measurement[measurements[0].Count][];
				for (int j = 0; j < mSwapped.Length; j++) {
					mSwapped[j] = new Measurement[measurements.Count];
					for (int i = 0; i < measurements.Count; i++) {
						mSwapped[j][i] = measurements[i][j];
					}
				}
				
				return new ResultSet {
					Name = name,
					Results = mSwapped.Select(m=>new Result(SelectMiddle(m))).ToList()
				};
			}
		}
		
		static string FormatTime(double milliseconds)
		{
			return milliseconds.ToString("f6", System.Globalization.NumberFormatInfo.InvariantInfo) + "ms";
		}
		
		static string FormatBytes(long bytes)
		{
			return (bytes / 1048576.0).ToString("f4", System.Globalization.NumberFormatInfo.InvariantInfo) + "MB";
		}
		
		static Measurement[] SelectMiddle(Measurement[] m)
		{
			return m.OrderBy(x=>x.Result).Skip(m.Length / 10).Take(7 * m.Length / 10).ToArray();
		}
		
		static void DisplayResults(ResultSet[] resultSets)
		{
			int maxNameLength = 2 + resultSets.Max(s => s.Name.Length);
			for (int i = 0; i < resultSets[0].Results.Count; i++) {
				Console.WriteLine();
				Console.WriteLine(resultSets[0].Results[i].Name);
				var averageStrings = resultSets.Select(s => s.Results[i].Average.ToString("f4")).ToArray();
				var sdStrings = resultSets.Select(s => s.Results[i].StandardDeviation.ToString("f4")).ToArray();
				int maxAverageLength = averageStrings.Max(s => s.Length);
				int maxSDLength = sdStrings.Max(s => s.Length);
				
				var bestResult = resultSets.Min(s => s.Results[i].Average);
				var factorStrings = resultSets.Select(s => (s.Results[i].Average / bestResult).ToString("f2")).ToArray();
				int maxFactorLength = factorStrings.Max(s => s.Length);
				
				for (int j = 0; j < resultSets.Length; j++) {
					Console.WriteLine("  " + (resultSets[j].Name + ":").PadRight(maxNameLength)
					                  + averageStrings[j].PadLeft(maxAverageLength) + resultSets[0].Results[i].Unit
					                  + " +- " + sdStrings[j].PadLeft(maxSDLength) + resultSets[0].Results[i].Unit
					                  + " (" + factorStrings[j].PadLeft(maxFactorLength) + "x)");
				}
			}
		}
		
		class Measurement
		{
			public string Name;
			public double Result;
			public string Unit;
			
			public static Measurement Parse(string text)
			{
				Match m = Regex.Match(text, @"^([\w\d\s]+):\s+([\d.]+)(ms|MB)");
				if (m.Success) {
					return new Measurement {
						Name = m.Groups[1].Value,
						Result = double.Parse(m.Groups[2].Value, System.Globalization.NumberFormatInfo.InvariantInfo),
						Unit = m.Groups[3].Value
					};
				} else {
					throw new Exception("Unknown output line " + text);
				}
			}
		}
		
		class Result
		{
			public string Name;
			public double Average;
			public double StandardDeviation;
			public string Unit;
			
			public Result(Measurement[] measurements)
			{
				Name = measurements[0].Name;
				Unit = measurements[0].Unit;
				double total = measurements.Sum(m=>m.Result);
				Average = total / measurements.Length;
				
				double variance = measurements.Sum(m => {
				                                   	double dist = m.Result - Average;
				                                   	return dist * dist;
				                                   });
				variance /= (measurements.Length - 1);
				StandardDeviation = Math.Sqrt(variance);
			}
		}
		
		class ResultSet
		{
			public string Name;
			public List<Result> Results;
		}
	}
}