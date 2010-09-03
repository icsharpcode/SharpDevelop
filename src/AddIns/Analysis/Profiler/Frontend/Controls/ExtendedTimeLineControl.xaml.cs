// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
			get { return this.provider; }
			set {
				this.provider = value;
				
				this.perfCounterList.ItemsSource = this.provider.GetPerformanceCounters();
				this.perfCounterList.SelectedIndex = 0;
				this.timeLine.Provider = provider;
				
				Update();
			}
		}
		
		public int SelectedStartIndex {
			get { return this.timeLine.SelectedStartIndex; }
			set { this.timeLine.SelectedStartIndex = value; }
		}
		
		public int SelectedEndIndex {
			get { return this.timeLine.SelectedEndIndex; }
			set { this.timeLine.SelectedEndIndex = value; }
		}
		
		public event EventHandler<RangeEventArgs> RangeChanged;

		protected virtual void OnRangeChanged(RangeEventArgs e)
		{
			if (RangeChanged != null)
				RangeChanged(this, e);
		}
		
		void Update()
		{
			this.timeLine.ValuesList.Clear();
			
			var selectedPerformanceCounter = this.perfCounterList.SelectedItem as PerformanceCounterDescriptor;
			
			if (selectedPerformanceCounter == null)
				return;
			
			List<TimeLineSegment> segments = new List<TimeLineSegment>();
			var values = this.provider.GetPerformanceCounterValues(this.perfCounterList.SelectedIndex);
			var markers = this.provider.DataSets.Select(item => item.IsFirst).ToArray();
			
			this.timeLine.MaxValue = selectedPerformanceCounter.MaxValue ?? (values.Any() ? values.Max() : 0);
			
			this.maxLabel.Text = (selectedPerformanceCounter.MaxValue ?? (values.Any() ? values.Max() : 0)).ToString("0");
			this.minLabel.Text = (selectedPerformanceCounter.MinValue ?? (values.Any() ? values.Min() : 0)).ToString("0");
			
			this.unitLabel.Text = this.timeLine.Unit = selectedPerformanceCounter.Unit;
			this.timeLine.Format = selectedPerformanceCounter.Format;
			
			for (int i = 0; i < values.Length; i++)
				segments.Add(new TimeLineSegment() { Value = values[i], DisplayMarker = markers[i], Events = provider.GetEventDataEntries(i) });
			
			this.timeLine.ValuesList.AddRange(segments);
		}
		
		void PerfCounterListSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Update();
		}
	}
}
