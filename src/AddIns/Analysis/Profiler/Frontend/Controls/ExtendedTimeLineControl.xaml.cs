// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
