using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ICSharpCode.Profiler.Controls
{
	/// <summary>
	/// A progressbar and task description with transparent overlay.
	/// </summary>
	public class WaitBar : Grid
	{
		ProgressBar progressBar;
		StackPanel panel;
		TextBlock text;
		
		public WaitBar(string statusText)
		{
			this.HorizontalAlignment = HorizontalAlignment.Stretch;
			this.VerticalAlignment = VerticalAlignment.Stretch;
			this.Background = new SolidColorBrush(Color.FromArgb(227, 255, 255, 255));
			
			this.Children.Add(panel = new StackPanel());
			
			panel.Children.Add(progressBar = new ProgressBar());
			
			progressBar.Height = 16;
			progressBar.Width = 120;
			
			progressBar.IsIndeterminate = true;
			
			panel.Children.Add(text = new TextBlock());
			
			text.Inlines.Add(statusText);
			text.HorizontalAlignment = HorizontalAlignment.Center;
			text.VerticalAlignment = VerticalAlignment.Center;

			panel.HorizontalAlignment = HorizontalAlignment.Stretch;
			panel.VerticalAlignment = VerticalAlignment.Center;
		}
	}
}
