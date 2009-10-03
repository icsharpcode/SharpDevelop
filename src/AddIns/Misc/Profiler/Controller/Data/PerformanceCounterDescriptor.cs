// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ICSharpCode.Profiler.Controller.Data
{
	public class PerformanceCounterDescriptor
	{
		public string Category { get; private set; }
		public string Name { get; private set; }
		public string Instance { get; private set; }
		public string Computer { get; private set; }
		public IList<float> Values { get; private set; }
		
		public float? MinValue { get; private set; }
		public float? MaxValue { get; private set; }
		public string Unit { get; private set; }
		
		float defaultValue;
		PerformanceCounter counter;
		
		public PerformanceCounterDescriptor(string category, string name, string instance, string computer,
		                                    float defaultValue, float? minValue, float? maxValue, string unit)
		{
			Category = category;
			Name = name;
			Instance = instance;
			Computer = computer;
			Values = new List<float>();
			this.defaultValue = defaultValue;
			MinValue = minValue;
			MaxValue = maxValue;
			Unit = unit;
		}
		
		public PerformanceCounterDescriptor(string name, float? minValue, float? maxValue, string unit)
			: this(null, name, null, null, 0, minValue, maxValue, unit)
		{
		}
		
		public static string GetProcessInstanceName(int pid)
		{
			PerformanceCounterCategory cat = new PerformanceCounterCategory("Process");

			string[] instances = cat.GetInstanceNames();
			foreach (string instance in instances) {
				using (PerformanceCounter cnt = new PerformanceCounter("Process", "ID Process", instance, true)) {
					int val = (int)cnt.RawValue;
					if (val == pid)
						return instance;
				}
			}
			
			return null;
		}
		
		public void Reset()
		{
			this.Values.Clear();
		}
		
		public void Collect(string instanceName)
		{
			if (counter == null && Instance != null)
				counter = new PerformanceCounter(Category, Name, instanceName ?? Instance, Computer);
			
			try {
				this.Values.Add(counter.NextValue());
			} catch (Exception e) {
				Console.WriteLine(e.ToString());
				this.Values.Add(defaultValue);
			}
		}
		
		public override string ToString()
		{
			return Name;
		}
	}
}
