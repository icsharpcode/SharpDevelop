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
		public string Format { get; private set; }
		
		float defaultValue;
		PerformanceCounter counter;
		
		public PerformanceCounterDescriptor(string category, string name, string instance, string computer,
		                                    float defaultValue, float? minValue, float? maxValue, string unit, string format)
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
			Format = format;
		}
		
		public PerformanceCounterDescriptor(string name, float? minValue, float? maxValue, string unit, string format)
			: this(null, name, null, null, 0, minValue, maxValue, unit, format)
		{
		}
		
		public static string GetProcessInstanceName(int pid)
		{
			PerformanceCounterCategory cat = new PerformanceCounterCategory("Process");

			string[] instances = cat.GetInstanceNames();
			foreach (string instance in instances) {
				using (PerformanceCounter procIdCounter = new PerformanceCounter("Process", "ID Process", instance, true)) {
					int val = (int)procIdCounter.RawValue;
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
				#if DEBUG
				Console.WriteLine(e.ToString());
				#endif
				this.Values.Add(defaultValue);
			}
		}
		
		public override string ToString()
		{
			return Name;
		}
	}
	
	public enum EventType : int
	{
		Exception = 0,
		Console = 1,
		WindowsForms = 2,
		WindowsPresentationFoundation = 3
	}
	
	public class EventDataEntry
	{
		public int DataSetId { get; set; }
		public EventType Type { get; set; }
		public int NameId { get; set; }
		public string Data { get; set; }
	}
}
