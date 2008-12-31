using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;

namespace SharpDevelop.XamlDesigner.Controls
{
	public class FilterDecorator : ContentControl
	{
		static FilterDecorator()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(FilterDecorator),
				new FrameworkPropertyMetadata(typeof(FilterDecorator)));
		}

		public FilterDecorator()
		{
			timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromMilliseconds(1000);
			timer.Tick += new EventHandler(timer_Tick);
		}

		DispatcherTimer timer;
		bool waitingForInput;

		public static readonly DependencyProperty FilterProperty =
			DependencyProperty.Register("Filter", typeof(string), typeof(FilterDecorator),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public string Filter
		{
			get { return (string)GetValue(FilterProperty); }
			set { SetValue(FilterProperty, value); }
		}

		static readonly DependencyPropertyKey IsFilteringPropertyKey =
			DependencyProperty.RegisterReadOnly("IsFiltering", typeof(bool), typeof(FilterDecorator), null);

		public static DependencyProperty IsFilteringProperty = IsFilteringPropertyKey.DependencyProperty;

		public bool IsFiltering
		{
			get { return (bool)GetValue(IsFilteringProperty); }
			private set { SetValue(IsFilteringPropertyKey, value); }
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			if (Keyboard.FocusedElement is TextBoxBase) return;
			Focus();
		}

		void timer_Tick(object sender, EventArgs e)
		{
			waitingForInput = false;
			timer.Stop();
		}

		protected override void OnPreviewTextInput(TextCompositionEventArgs e)
		{
			if (e.OriginalSource is TextBoxBase) return;

			if (e.Text != null && 
				e.Text.Length > 0 && 
				char.IsLetterOrDigit(e.Text[0])) {

				if (waitingForInput) {
					Filter = Filter + e.Text;
				}
				else {
					Filter = e.Text;
					waitingForInput = true;
				}
				IsFiltering = true;
				timer.Stop();
				timer.Start();
				e.Handled = true;
			}
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Escape && IsFiltering) {
				StopFiltering();
				e.Handled = true;
			}
		}

		void StopFiltering()
		{
			timer.Stop();
			Filter = null;
			IsFiltering = false;
			waitingForInput = false;
		}
	}
}
