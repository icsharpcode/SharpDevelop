// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Allows animated collapsing of the content of this panel.
	/// </summary>
	public class CollapsiblePanel : ContentControl
	{
		static CollapsiblePanel()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(CollapsiblePanel),
			                                         new FrameworkPropertyMetadata(typeof(CollapsiblePanel)));
		}
		
		public static readonly DependencyProperty IsCollapsedProperty = DependencyProperty.Register(
			"IsCollapsed", typeof(bool), typeof(CollapsiblePanel),
			new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsCollapsedChanged)));
		
		public bool IsCollapsed {
			get { return (bool)GetValue(IsCollapsedProperty); }
			set { SetValue(IsCollapsedProperty, value); }
		}
		
		public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(
			"Duration", typeof(TimeSpan), typeof(CollapsiblePanel),
			new UIPropertyMetadata(TimeSpan.FromMilliseconds(250)));
		
		/// <summary>
		/// The duration in milliseconds of the animation.
		/// </summary>
		public TimeSpan Duration {
			get { return (TimeSpan)GetValue(DurationProperty); }
			set { SetValue(DurationProperty, value); }
		}
		
		protected internal static readonly DependencyProperty AnimationProgressProperty = DependencyProperty.Register(
			"AnimationProgress", typeof(double), typeof(CollapsiblePanel),
			new FrameworkPropertyMetadata(1.0));
		
		/// <summary>
		/// Value between 0 and 1 specifying how far the animation currently is.
		/// </summary>
		protected internal double AnimationProgress {
			get { return (double)GetValue(AnimationProgressProperty); }
			set { SetValue(AnimationProgressProperty, value); }
		}
		
		static void OnIsCollapsedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((CollapsiblePanel)d).SetupAnimation((bool)e.NewValue);
		}
		
		void SetupAnimation(bool isCollapsed)
		{
			if (this.IsLoaded) {
				// If the animation is already running, calculate remaining portion of the time
				double currentProgress = AnimationProgress;
				if (!isCollapsed) {
					currentProgress = 1.0 - currentProgress;
				}
				
				DoubleAnimation animation = new DoubleAnimation();
				animation.To = isCollapsed ? 0.0 : 1.0;
				animation.Duration = TimeSpan.FromSeconds(Duration.TotalSeconds * currentProgress);
				animation.FillBehavior = FillBehavior.HoldEnd;
				
				this.BeginAnimation(AnimationProgressProperty, animation);
			} else {
				this.AnimationProgress = isCollapsed ? 0.0 : 1.0;
			}
		}
	}
	
	sealed class CollapsiblePanelProgressToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is double)
				return (double)value > 0 ? Visibility.Visible : Visibility.Collapsed;
			else
				return Visibility.Visible;
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
