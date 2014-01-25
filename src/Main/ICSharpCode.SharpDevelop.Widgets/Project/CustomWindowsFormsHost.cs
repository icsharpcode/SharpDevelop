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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace ICSharpCode.SharpDevelop.Widgets
{
	using TextBlock = System.Windows.Controls.TextBlock;
	
	/// <summary>
	/// A custom Windows Forms Host implementation.
	/// Hopefully fixes SD-1842 - ArgumentException in SetActiveControlInternal (WindowsFormsHost.RestoreFocusedChild)
	/// </summary>
	public class CustomWindowsFormsHost : HwndHost, IKeyboardInputSink
	{
		// Interactions of the MS WinFormsHost:
		//   IME
		//   Font sync
		//   Property sync
		//   Background sync (rendering bitmaps!)
		//   Access keys
		//   Tab Navigation
		//   Save/Restore focus for app switch
		//   Size feedback (WinForms control tells WPF desired size)
		//   Focus enter/leave - validation events
		//   ...
		
		// We don't need most of that.
		
		// Bugs in our implementation:
		//  - Slight background color mismatch in project options
		
		static class Win32
		{
			[DllImport("user32.dll")]
			public static extern IntPtr GetFocus();
			
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool IsChild(IntPtr hWndParent, IntPtr hwnd);
			
			[DllImport("user32.dll")]
			internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
			
			[DllImport("user32.dll")]
			internal static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);
		}
		
		#region Remember/RestoreFocusedChild
		sealed class HostNativeWindow : NativeWindow
		{
			readonly CustomWindowsFormsHost host;
			
			public HostNativeWindow(CustomWindowsFormsHost host)
			{
				this.host = host;
			}
			
			protected override void WndProc(ref Message m)
			{
				const int WM_ACTIVATEAPP = 0x1C;
				if (m.Msg == WM_ACTIVATEAPP) {
					if (m.WParam == IntPtr.Zero) {
						// The window is being deactivated:
						// If a WinForms control within this host has focus, remember it.
						IntPtr focus = Win32.GetFocus();
						if (focus == host.Handle || Win32.IsChild(host.Handle, focus)) {
							host.RememberActiveControl();
							host.Log("Window deactivated; RememberActiveControl(): " + host.savedActiveControl);
						} else {
							host.Log("Window deactivated; but focus not within WinForms");
						}
					} else {
						// The window is being activated.
						host.Log("Window activated");
						host.Dispatcher.BeginInvoke(
							DispatcherPriority.Normal,
							new Action(host.RestoreActiveControl));
					}
				}
				base.WndProc(ref m);
			}
			
			protected override void OnThreadException(Exception e)
			{
				System.Windows.Forms.Application.OnThreadException(e);
			}
		}
		
		HostNativeWindow hostNativeWindow;
		Control savedActiveControl;
		
		void RememberActiveControl()
		{
			savedActiveControl = container.ActiveControl;
		}
		
		void RestoreActiveControl()
		{
			if (savedActiveControl != null) {
				Log("RestoreActiveControl(): " + savedActiveControl);
				savedActiveControl.Focus();
				savedActiveControl = null;
			}
		}
		#endregion
		
		#region Container
		sealed class HostedControlContainer : ContainerControl
		{
			Control child;
			
			protected override void OnHandleCreated(EventArgs e)
			{
				base.OnHandleCreated(e);
				const int WM_UPDATEUISTATE = 0x0128;
				const int UISF_HIDEACCEL = 2;
				const int UISF_HIDEFOCUS = 1;
				const int UIS_SET = 1;
				Win32.SendMessage(this.Handle, WM_UPDATEUISTATE, new IntPtr(UISF_HIDEACCEL | UISF_HIDEFOCUS | (UIS_SET << 16)), IntPtr.Zero);
			}
			
			public Control Child {
				get { return child; }
				set {
					if (child != null) {
						this.Controls.Remove(child);
					}
					child = value;
					if (value != null) {
						value.Dock = DockStyle.Fill;
						this.Controls.Add(value);
					}
				}
			}
		}
		#endregion
		
		readonly HostedControlContainer container;
		
		#region Constructors
		/// <summary>
		/// Creates a new CustomWindowsFormsHost instance.
		/// </summary>
		public CustomWindowsFormsHost()
		{
			this.container = new HostedControlContainer();
			Init();
		}
		
		/// <summary>
		/// Creates a new CustomWindowsFormsHost instance that allows hosting controls
		/// from the specified AppDomain.
		/// </summary>
		public CustomWindowsFormsHost(AppDomain childDomain)
		{
			Type type = typeof(HostedControlContainer);
			this.container = (HostedControlContainer)childDomain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
			Init();
		}
		
		void Init()
		{
			this.EnableFontInheritance = true;
			this.Loaded += OnLoaded;
			this.Unloaded += OnUnloaded;
			Log("Instance created");
		}
		
		void OnLoaded(object sender, RoutedEventArgs e)
		{
			Log("OnLoaded()");
			SetFont();
			if (hwndParent.Handle != IntPtr.Zero && hostNativeWindow != null) {
				if (hostNativeWindow.Handle == IntPtr.Zero)
					hostNativeWindow.AssignHandle(hwndParent.Handle);
			}
		}
		
		void OnUnloaded(object sender, RoutedEventArgs e)
		{
			Log("OnUnloaded()");
			if (hostNativeWindow != null) {
				savedActiveControl = null;
				hostNativeWindow.ReleaseHandle();
			}
		}
		#endregion
		
		#region Font Synchronization
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == TextBlock.FontFamilyProperty || e.Property == TextBlock.FontSizeProperty) {
				SetFont();
			}
		}
		
		public bool EnableFontInheritance { get; set; }
		
		void SetFont()
		{
			if (!EnableFontInheritance)
				return;
			string fontFamily = TextBlock.GetFontFamily(this).Source;
			float fontSize = (float)(TextBlock.GetFontSize(this) * (72.0 / 96.0));
			container.Font = new System.Drawing.Font(fontFamily, fontSize, System.Drawing.FontStyle.Regular);
		}
		#endregion
		
		public Control Child {
			get { return container.Child; }
			set { container.Child = value; }
		}
		
		protected override Size MeasureOverride(Size constraint)
		{
			return new Size(0, 0);
		}
		
		protected override Size ArrangeOverride(Size finalSize)
		{
			container.Size = new System.Drawing.Size((int)finalSize.Width, (int)finalSize.Height);
			return finalSize;
		}
		
		HandleRef hwndParent;
		
		protected override HandleRef BuildWindowCore(HandleRef hwndParent)
		{
			Log("BuildWindowCore");
			if (hostNativeWindow != null) {
				hostNativeWindow.ReleaseHandle();
			} else {
				hostNativeWindow = new CustomWindowsFormsHost.HostNativeWindow(this);
			}
			this.hwndParent = hwndParent;
			hostNativeWindow.AssignHandle(hwndParent.Handle);
			
			IntPtr childHandle = container.Handle;
			Win32.SetParent(childHandle, hwndParent.Handle);
			return new HandleRef(container, childHandle);
		}
		
		protected override void DestroyWindowCore(HandleRef hwnd)
		{
			Log("DestroyWindowCore");
			hostNativeWindow.ReleaseHandle();
			savedActiveControl = null;
			hwndParent = default(HandleRef);
		}
		
		protected override void Dispose(bool disposing)
		{
			Log("Dispose (disposing="+disposing+")");
			base.Dispose(disposing);
			if (disposing) {
				container.Dispose();
			}
		}
		
		protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch (msg) {
				case 0x0007: // WM_SETFOCUS
				case 0x0021: // WM_MOUSEACTIVATE
					// Give the WindowsFormsHost logical focus:
					DependencyObject focusScope = this;
					while (focusScope != null && !FocusManager.GetIsFocusScope(focusScope)) {
						focusScope = VisualTreeHelper.GetParent(focusScope);
					}
					if (focusScope != null) {
						FocusManager.SetFocusedElement(focusScope, this);
					}
					break;
			}
			return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
		}
		
		#if DEBUG
		static int hostCount;
		int instanceID = System.Threading.Interlocked.Increment(ref hostCount);
		#endif
		
		[Conditional("DEBUG")]
		void Log(string text)
		{
			#if DEBUG
			Debug.WriteLine("CustomWindowsFormsHost #{0}: {1}", instanceID, text);
			#endif
		}
	}
}
