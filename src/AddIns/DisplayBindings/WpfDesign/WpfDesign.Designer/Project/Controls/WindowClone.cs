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
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;

using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// A custom control that imitates the properties of <see cref="Window"/>, but is not a top-level control.
	/// </summary>
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
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
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
		
		/// <summary>
		/// This property has no effect. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		public bool ShowActivated {
			get { return (bool)GetValue(Window.ShowActivatedProperty); }
			set { SetValue(Window.ShowActivatedProperty, value); }
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
		/// This property has no effect. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		public TaskbarItemInfo TaskbarItemInfo {
			get { return (TaskbarItemInfo)GetValue(Window.TaskbarItemInfoProperty); }
			set { SetValue(Window.TaskbarItemInfoProperty, value); }
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
		
		/// <summary>
		/// This event is never raised. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
		public event EventHandler Activated { add {} remove {} }
		
		/// <summary>
		/// This event is never raised. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
		public event EventHandler Closed { add {} remove {} }
		
		/// <summary>
		/// This event is never raised. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
		public event EventHandler Closing { add {} remove {} }
		
		/// <summary>
		/// This event is never raised. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
		public event EventHandler ContentRendered { add {} remove {} }
		
		/// <summary>
		/// This event is never raised. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
		public event EventHandler Deactivated { add {} remove {} }
		
		/// <summary>
		/// This event is never raised. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
		public event EventHandler LocationChanged { add {} remove {} }
		
		/// <summary>
		/// This event is never raised. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
		public event EventHandler SourceInitialized { add {} remove {} }
		
		/// <summary>
		/// This event is never raised. (for compatibility with <see cref="Window"/> only).
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
		public event EventHandler StateChanged { add {} remove {} }
	}
	
	/// <summary>
	/// A <see cref="CustomInstanceFactory"/> for <see cref="Window"/>
	/// (and derived classes, unless they specify their own <see cref="CustomInstanceFactory"/>).
	/// </summary>
	[ExtensionFor(typeof(Window))]
	public class WindowCloneExtension : CustomInstanceFactory
	{
		/// <summary>
		/// Used to create instances of <see cref="WindowClone"/>.
		/// </summary>
		public override object CreateInstance(Type type, params object[] arguments)
		{
			Debug.Assert(arguments.Length == 0);
			return new WindowClone();
		}
	}
}
