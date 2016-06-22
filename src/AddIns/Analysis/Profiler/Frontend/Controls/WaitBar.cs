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
using System.Windows;
using System.Windows.Controls;
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
