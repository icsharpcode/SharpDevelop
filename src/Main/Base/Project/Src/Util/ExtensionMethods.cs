// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using WinForms = System.Windows.Forms;
using System.Windows.Documents;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Extension methods used in SharpDevelop.
	/// </summary>
	public static class ExtensionMethods
	{
		/// <summary>
		/// Runs an action for all elements in the input.
		/// </summary>
		public static void Foreach<T>(this IEnumerable<T> input, Action<T> action)
		{
			if (input == null)
				throw new ArgumentNullException("input");
			foreach (T element in input) {
				action(element);
			}
		}
		
		/// <summary>
		/// Adds all <paramref name="elements"/> to <paramref name="list"/>.
		/// </summary>
		public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> elements)
		{
			foreach (T o in elements)
				list.Add(o);
		}
		
		public static ReadOnlyCollection<T> AsReadOnly<T>(this T[] arr)
		{
			return Array.AsReadOnly(arr);
		}
		
		public static string Join(this IEnumerable<string> input, string separator)
		{
			return string.Join(separator, input.ToArray());
		}
		
		public static IEnumerable<WinForms.Control> GetRecursive(this WinForms.Control.ControlCollection collection)
		{
			foreach (WinForms.Control ctl in collection) {
				yield return ctl;
				foreach (WinForms.Control subCtl in ctl.Controls.GetRecursive()) {
					yield return subCtl;
				}
			}
		}
		
		/// <summary>
		/// Creates an array containing a part of the array (similar to string.Substring).
		/// </summary>
		public static T[] Splice<T>(this T[] array, int startIndex)
		{
			if (array == null)
				throw new ArgumentNullException("array");
			return Splice(array, startIndex, array.Length - startIndex);
		}
		
		/// <summary>
		/// Creates an array containing a part of the array (similar to string.Substring).
		/// </summary>
		public static T[] Splice<T>(this T[] array, int startIndex, int length)
		{
			if (array == null)
				throw new ArgumentNullException("array");
			if (startIndex < 0 || startIndex > array.Length)
				throw new ArgumentOutOfRangeException("startIndex", startIndex, "Value must be between 0 and " + array.Length);
			if (length < 0 || length > array.Length - startIndex)
				throw new ArgumentOutOfRangeException("length", length, "Value must be between 0 and " + (array.Length - startIndex));
			T[] result = new T[length];
			Array.Copy(array, startIndex, result, 0, length);
			return result;
		}
		
		/// <summary>
		/// Gets the IWin32Window associated with a WPF window.
		/// </summary>
		public static WinForms.IWin32Window GetWin32Window(this System.Windows.Window window)
		{
			var wnd = System.Windows.PresentationSource.FromVisual(window) as System.Windows.Interop.IWin32Window;
			if (wnd != null)
				return new Win32WindowAdapter(wnd);
			else
				return null;
		}
		
		sealed class Win32WindowAdapter : WinForms.IWin32Window
		{
			System.Windows.Interop.IWin32Window window;
			
			public Win32WindowAdapter(System.Windows.Interop.IWin32Window window)
			{
				this.window = window;
			}
			
			public IntPtr Handle {
				get { return window.Handle; }
			}
		}
		
		/// <summary>
		/// Sets the Content property of the specified ControlControl to the specified content.
		/// If the content is a Windows-Forms control, it is wrapped in a WindowsFormsHost.
		/// If the content control already contains a WindowsFormsHost with that content,
		/// the old WindowsFormsHost is kept.
		/// When a WindowsFormsHost is replaced with another content, the host is disposed (but the control
		/// inside the host isn't)
		/// </summary>
		public static void SetContent(this ContentControl contentControl, object content)
		{
			if (contentControl == null)
				throw new ArgumentNullException("contentControl");
			var host = contentControl.Content as SDWindowsFormsHost;
			if (host != null) {
				if (host.Child == content)
					return;
				host.Dispose();
			}
			if (content is WinForms.Control) {
				contentControl.Content = new SDWindowsFormsHost((WinForms.Control)content);
			} else if (content is string) {
				contentControl.Content = new TextBlock(new Run(content.ToString())) { TextWrapping = TextWrapping.Wrap };
			} else {
				contentControl.Content = content;
			}
		}
		
		class SDWindowsFormsHost : WinForms.Integration.WindowsFormsHost
		{
			public SDWindowsFormsHost(WinForms.Control child)
			{
				this.Child = child;
				child.Disposed += child_Disposed;
			}

			void child_Disposed(object sender, EventArgs e)
			{
				Dispose();
			}
			
			protected override void Dispose(bool disposing)
			{
				if (disposing && Child != null) {
					Child.Disposed -= child_Disposed;
					// prevent child from being disposed
					Child = null;
				}
				base.Dispose(disposing);
			}
		}
	}
}
