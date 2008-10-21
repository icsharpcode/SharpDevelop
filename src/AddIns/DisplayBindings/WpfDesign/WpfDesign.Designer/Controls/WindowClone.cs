// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3210 $</version>
// </file>

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// A custom control that imitates the properties of <see cref="Window"/>, but is not a top-level control.
	/// </summary>
	[ReplacerFor(typeof(Window))]
	public class WindowClone : ContentControl
	{
		static WindowClone()
		{
			//This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
			//This style is defined in themes\generic.xaml
			DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowClone), new FrameworkPropertyMetadata(typeof(WindowClone)));
			
			Control.IsTabStopProperty.OverrideMetadata(typeof(WindowClone), new FrameworkPropertyMetadata(SharedInstances.BoxedFalse));
			KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(WindowClone), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
			KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(WindowClone), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
			KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof(WindowClone), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
			FocusManager.IsFocusScopeProperty.OverrideMetadata(typeof(WindowClone), new FrameworkPropertyMetadata(SharedInstances.BoxedTrue));
		}
		
		/// <summary>
		/// This property has no effect. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		public bool AllowsTransparency {
			get { return (bool)GetValue(Window.AllowsTransparencyProperty); }
			set { SetValue(Window.AllowsTransparencyProperty, SharedInstances.Box(value)); }
		}
		
		/// <summary>
		/// This property has no effect. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), TypeConverter(typeof(DialogResultConverter))]
		public bool? DialogResult {
			get {
				return null;
			}
			set { }
		}
		
		/// <summary>
		/// Specifies the icon to use.
		/// </summary>
		public ImageSource Icon {
			get { return (ImageSource)GetValue(Window.IconProperty); }
			set { SetValue(Window.IconProperty, value); }
		}
		
		/// <summary>
		/// This property has no effect. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		[TypeConverter(typeof(LengthConverter))]
		public double Left {
			get { return (double)GetValue(Window.LeftProperty); }
			set { SetValue(Window.LeftProperty, value); }
		}
		
		Window owner;
		
		/// <summary>
		/// This property has no effect. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		public Window Owner {
			get { return owner; }
			set { owner = value; }
		}
		
		/// <summary>
		/// Gets or sets the resize mode.
		/// </summary>
		public ResizeMode ResizeMode {
			get { return (ResizeMode)GetValue(Window.ResizeModeProperty); }
			set { SetValue(Window.ResizeModeProperty, value); }
		}

		public static readonly DependencyProperty ShowActivatedProperty =
			DependencyProperty.Register("ShowActivated", typeof(bool), typeof(WindowClone));

		public bool ShowActivated {
			get { return (bool)GetValue(ShowActivatedProperty); }
			set { SetValue(ShowActivatedProperty, value); }
		}
		
		/// <summary>
		/// This property has no effect. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		public bool ShowInTaskbar {
			get { return (bool)GetValue(Window.ShowInTaskbarProperty); }
			set { SetValue(Window.ShowInTaskbarProperty, SharedInstances.Box(value)); }
		}
		
		/// <summary>
		/// Gets or sets a value that specifies whether a window will automatically size itself to fit the size of its content.
		/// </summary>
		public SizeToContent SizeToContent {
			get { return (SizeToContent)GetValue(Window.SizeToContentProperty); }
			set { SetValue(Window.SizeToContentProperty, value); }
		}
		
		/// <summary>
		/// The title to display in the Window's title bar.
		/// </summary>
		public string Title {
			get { return (string)GetValue(Window.TitleProperty); }
			set { SetValue(Window.TitleProperty, value); }
		}
		
		/// <summary>
		/// This property has no effect. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		[TypeConverter(typeof(LengthConverter))]
		public double Top {
			get { return (double)GetValue(Window.TopProperty); }
			set { SetValue(Window.TopProperty, value); }
		}
		
		/// <summary>
		/// This property has no effect. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		public bool Topmost {
			get { return (bool)GetValue(Window.TopmostProperty); }
			set { SetValue(Window.TopmostProperty, SharedInstances.Box(value)); }
		}
		
		WindowStartupLocation windowStartupLocation;
		
		/// <summary>
		/// This property has no effect. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		public WindowStartupLocation WindowStartupLocation {
			get { return windowStartupLocation; }
			set { windowStartupLocation = value; }
		}
		
		/// <summary>
		/// This property has no effect. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		public WindowState WindowState {
			get { return (WindowState) GetValue(Window.WindowStateProperty); }
			set { SetValue(Window.WindowStateProperty, value); }
		}
		
		/// <summary>
		/// This property has no effect. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		public WindowStyle WindowStyle {
			get { return (WindowStyle)GetValue(Window.WindowStyleProperty); }
			set { SetValue(Window.WindowStyleProperty, value); }
		}
		
		#pragma warning disable 0067
		// disable "event is never used" warning
		
		/// <summary>
		/// This event is never raised. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		public event EventHandler Activated;
		
		/// <summary>
		/// This event is never raised. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		public event EventHandler Closed;
		
		/// <summary>
		/// This event is never raised. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		public event EventHandler Closing;
		
		/// <summary>
		/// This event is never raised. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		public event EventHandler ContentRendered;
		
		/// <summary>
		/// This event is never raised. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		public event EventHandler Deactivated;
		
		/// <summary>
		/// This event is never raised. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		public event EventHandler LocationChanged;
		
		/// <summary>
		/// This event is never raised. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		public event EventHandler SourceInitialized;
		
		/// <summary>
		/// This event is never raised. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		public event EventHandler StateChanged;
		#pragma warning restore
	}
}
