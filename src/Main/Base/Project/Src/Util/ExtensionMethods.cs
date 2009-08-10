// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core.Presentation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using WinForms = System.Windows.Forms;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Extension methods used in SharpDevelop.
	/// </summary>
	public static class ExtensionMethods
	{
		/// <summary>
		/// Raises the event.
		/// Does nothing if eventHandler is null.
		/// Because the event handler is passed as parameter, it is only fetched from the event field one time.
		/// This makes
		/// <code>MyEvent.RaiseEvent(x,y);</code>
		/// thread-safe
		/// whereas
		/// <code>if (MyEvent != null) MyEvent(x,y);</code>
		/// would not be safe.
		/// </summary>
		/// <remarks>Using this method is only thread-safe under the Microsoft .NET memory model,
		/// not under the less strict memory model in the CLI specification.</remarks>
		public static void RaiseEvent(this EventHandler eventHandler, object sender, EventArgs e)
		{
			if (eventHandler != null) {
				eventHandler(sender, e);
			}
		}
		
		/// <summary>
		/// Raises the event.
		/// Does nothing if eventHandler is null.
		/// Because the event handler is passed as parameter, it is only fetched from the event field one time.
		/// This makes
		/// <code>MyEvent.RaiseEvent(x,y);</code>
		/// thread-safe
		/// whereas
		/// <code>if (MyEvent != null) MyEvent(x,y);</code>
		/// would not be safe.
		/// </summary>
		public static void RaiseEvent<T>(this EventHandler<T> eventHandler, object sender, T e) where T : EventArgs
		{
			if (eventHandler != null) {
				eventHandler(sender, e);
			}
		}
		
		/// <summary>
		/// Runs an action for all elements in the input.
		/// </summary>
		public static void ForEach<T>(this IEnumerable<T> input, Action<T> action)
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
			readonly System.Windows.Interop.IWin32Window window;
			
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
			SetContent(contentControl, content, null);
		}
		
		public static void SetContent(this ContentControl contentControl, object content, object serviceObject)
		{
			if (contentControl == null)
				throw new ArgumentNullException("contentControl");
			// serviceObject = object implementing the old clipboard/undo interfaces
			// to allow WinForms AddIns to handle WPF commands
			
			var host = contentControl.Content as SDWindowsFormsHost;
			if (host != null) {
				if (host.Child == content) {
					host.ServiceObject = serviceObject;
					return;
				}
				host.Dispose();
			}
			if (content is WinForms.Control) {
				contentControl.Content = new SDWindowsFormsHost {
					Child = (WinForms.Control)content,
					ServiceObject = serviceObject,
					DisposeChild = false
				};
			} else if (content is string) {
				contentControl.Content = new TextBlock {
					Text = content.ToString(),
					TextWrapping = TextWrapping.Wrap
				};
			} else {
				contentControl.Content = content;
			}
		}
		
		#region System.Drawing <-> WPF conversions
		public static System.Drawing.Point ToSystemDrawing(this Point p)
		{
			return new System.Drawing.Point((int)p.X, (int)p.Y);
		}
		
		public static System.Drawing.Size ToSystemDrawing(this Size s)
		{
			return new System.Drawing.Size((int)s.Width, (int)s.Height);
		}
		
		public static System.Drawing.Rectangle ToSystemDrawing(this Rect r)
		{
			return new System.Drawing.Rectangle(r.TopLeft.ToSystemDrawing(), r.Size.ToSystemDrawing());
		}
		
		public static System.Drawing.Color ToSystemDrawing(this System.Windows.Media.Color c)
		{
			return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
		}
		
		public static Point ToWpf(this System.Drawing.Point p)
		{
			return new Point(p.X, p.Y);
		}
		
		public static Size ToWpf(this System.Drawing.Size s)
		{
			return new Size(s.Width, s.Height);
		}
		
		public static Rect ToWpf(this System.Drawing.Rectangle rect)
		{
			return new Rect(rect.Location.ToWpf(), rect.Size.ToWpf());
		}
		
		public static System.Windows.Media.Color ToWpf(this System.Drawing.Color c)
		{
			return System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);
		}
		#endregion
		
		public static string Replace(this string original, string pattern, string replacement, StringComparison comparisonType)
		{
			if (original == null)
				throw new ArgumentNullException("original");
			if (pattern == null)
				throw new ArgumentNullException("pattern");
			if (pattern.Length == 0)
				throw new ArgumentException("String cannot be of zero length.", "pattern");
			if (comparisonType != StringComparison.Ordinal && comparisonType != StringComparison.OrdinalIgnoreCase)
				throw new NotSupportedException("Currently only ordinal comparisons are implemented.");
			
			StringBuilder result = new StringBuilder(original.Length);
			int currentPos = 0;
			int nextMatch = original.IndexOf(pattern, comparisonType);
			while (nextMatch >= 0) {
				result.Append(original, currentPos, nextMatch - currentPos);
				// The following line restricts this method to ordinal comparisons:
				// for non-ordinal comparisons, the match length might be different than the pattern length.
				currentPos = nextMatch + pattern.Length;
				result.Append(replacement);
				
				nextMatch = original.IndexOf(pattern, currentPos, comparisonType);
			}
			
			result.Append(original, currentPos, original.Length - currentPos);
			return result.ToString();
		}
		
		public static byte[] GetBytesWithPreamble(this Encoding encoding, string text)
		{
			byte[] encodedText = encoding.GetBytes(text);
			byte[] bom = encoding.GetPreamble();
			if (bom != null && bom.Length > 0) {
				byte[] result = new byte[bom.Length + encodedText.Length];
				bom.CopyTo(result, 0);
				encodedText.CopyTo(result, bom.Length);
				return result;
			} else {
				return encodedText;
			}
		}
		
		/// <summary>
		/// Creates a new image for the image source.
		/// </summary>
		public static Image CreateImage(this IImage image)
		{
			if (image == null)
				throw new ArgumentNullException("image");
			return new Image { Source = image.ImageSource };
		}
		
		/// <summary>
		/// Creates a new image for the image source.
		/// </summary>
		public static UIElement CreatePixelSnappedImage(this IImage image)
		{
			return new PixelSnapper(CreateImage(image));
		}
		
		/// <summary>
		/// Translates a WinForms menu to WPF.
		/// </summary>
		public static ICollection TranslateToWpf(this ToolStripItem[] items)
		{
			return items.OfType<ToolStripMenuItem>().Select(item => TranslateMenuItemToWpf(item)).ToList();
		}
		
		static System.Windows.Controls.MenuItem TranslateMenuItemToWpf(ToolStripMenuItem item)
		{
			var r = new System.Windows.Controls.MenuItem();
			r.Header = MenuService.ConvertLabel(item.Text);
			if (item.ImageIndex >= 0)
				r.Icon = ClassBrowserIconService.GetImageByIndex(item.ImageIndex).CreateImage();
			if (item.DropDownItems.Count > 0) {
				foreach (ToolStripMenuItem subItem in item.DropDownItems) {
					r.Items.Add(TranslateMenuItemToWpf(subItem));
				}
			} else {
				r.Click += delegate { item.PerformClick(); };
			}
			return r;
		}
	}
}
