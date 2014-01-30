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
using System.Windows.Controls;

using ICSharpCode.Profiler.Controller.Data;

namespace ICSharpCode.Profiler.Controls
{
	/// <summary>
	/// Interaction logic for ExtendedTimeLineControl.xaml
	/// </summary>
	public partial class ExtendedTimeLineControl : UserControl
	{
		ProfilingDataProvider provider;
		
		public ExtendedTimeLineControl()
		{
			InitializeComponent();
			
			this.timeLine.RangeChanged += (sender, e) => { OnRangeChanged(e); };
		}
		
		public ProfilingDataProvider Provider
		{
			get { return provider; }
			set {
				provider = value;
				
				perfCounterList.ItemsSource = provider.GetPerformanceCounters();
				perfCounterList.SelectedIndex = 0;
				timeLine.Provider = provider;
				
				Update();
			}
		}
		
		public int SelectedStartIndex {
			get { return timeLine.SelectedStartIndex; }
			set { timeLine.SelectedStartIndex = value; }
		}
		
		public int SelectedEndIndex {
			get { return timeLine.SelectedEndIndex; }
			set { timeLine.SelectedEndIndex = value; }
		}
		
		public event EventHandler<RangeEventArgs> RangeChanged;

		protected virtual void OnRangeChanged(RangeEventArgs e)
		{
			if (RangeChanged != null)
				RangeChanged(this, e);
		}
		
		void Update()
		{
			timeLine.ValuesList.Clear();
			
			var selectedPerformanceCounter = perfCounterList.SelectedItem as PerformanceCounterDescriptor;
			
			if (selectedPerformanceCounter == null)
				return;
			
			List<TimeLineSegment> segments = new List<TimeLineSegment>();
			var values = provider.GetPerformanceCounterValues(perfCounterList.SelectedIndex);
			var markers = provider.DataSets.Select(item => item.IsFirst).ToArray();
			
			timeLine.MaxValue = selectedPerformanceCounter.MaxValue ?? (values.Any() ? values.Max() : 0);
			
			maxLabel.Text = (selectedPerformanceCounter.MaxValue ?? (values.Any() ? values.Max() : 0)).ToString("0");
			minLabel.Text = (selectedPerformanceCounter.MinValue ?? (values.Any() ? values.Min() : 0)).ToString("0");
			
			unitLabel.Text = timeLine.Unit = selectedPerformanceCounter.Unit;
			timeLine.Format = selectedPerformanceCounter.Format;
			
			for (int i = 0; i < values.Length; i++)
				segments.Add(new TimeLineSegment() { Value = values[i], DisplayMarker = markers[i], Events = provider.GetEventDataEntries(i) });
			
			timeLine.ValuesList.AddRange(segments);
		}
		
		void PerfCounterListSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Update();
		}
	}
}
