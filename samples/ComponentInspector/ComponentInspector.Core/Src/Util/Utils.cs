// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

using NoGoop.Win32;

namespace NoGoop.Util
{
	internal interface IHasControl
	{
		Control GetControl();
	}
	
	internal class Utils
	{
		internal const int SPLITTER_SIZE = 3;
		internal const int BUTTON_HEIGHT = 23;
		internal const int BUTTON_WIDTH_PAD = 15;
			/***
			Object ie = AppDomain.CreateInstance(ie);
			**/
		internal const bool             SHOW_WINDOW = true;
		
		protected static void InvokeWithSHDocVw(String uriStr)
		{
			Object ie;
			Assembly shdocvw = null;
			foreach (Assembly assy in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (assy.GetName().Name.Equals("SHDocVw"))
				{
					shdocvw = assy;
					break;
				}
			}
			if (shdocvw == null)
				throw new Exception("SHDocVw assembly not found");
			Type ieType = 
				shdocvw.GetType("SHDocVw.InternetExplorerClass", true);
			ConstructorInfo ieCons = ieType.GetConstructor(new Type[0]);
			if (ieCons == null)
			{
				throw new Exception
					("InternetExplorerClass constructor not found");
			}
			ie = ieCons.Invoke(null);
			if (ie == null)
			{
				throw new Exception
					("Unable to create SHDocVw/InternetExplorer");
			}
			MethodInfo mi = ieType.GetMethod("Navigate");
			mi.Invoke(ie, new Object[] { uriStr, null, null, null, null });
			PropertyInfo visProp = ieType.GetProperty("Visible");
			visProp.SetValue(ie, true, null);
			PropertyInfo hwndProp = ieType.GetProperty("HWND");
			int hwnd = (int)hwndProp.GetValue(ie, null);
			Windows.SetForegroundWindow(hwnd);
		}

		protected static void InvokeWithProcessStart(String uriStr)
		{
			ProcessStartInfo psi = new ProcessStartInfo();
			/* Simply using Process.Start(e.Link.LinkData.ToString());
			   does not work on all pc's! */
			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				/* Start is an internal command in WinNT/2000/XP */
				psi.FileName = "cmd.exe";
				psi.Arguments = "/c start " + uriStr;
			}
			else
			{
				/* Start is an external command in Win9x */
				psi.FileName = "start.exe";
				psi.Arguments = uriStr;
			}
			/* Hide the command prompt window */
			psi.WindowStyle = ProcessWindowStyle.Hidden;
			//Console.WriteLine("psi.FileName: " + psi.FileName);
			//Console.WriteLine("psi.Arguments: " + psi.Arguments);
			Process.Start(psi);
		}
		
		internal static void InvokeBrowser(String url)
		{
			InvokeBrowser(url, SHOW_WINDOW);
		}
		
		internal static void InvokeBrowser(Uri url)
		{
			InvokeBrowser(url, SHOW_WINDOW);
		}
		
		internal static void InvokeBrowser(String uri, bool showWindow)
		{
			string uriInput = uri;
			if (!uri.StartsWith("http:"))
				uriInput = "http://" + uri;
			InvokeBrowser(new Uri(uriInput), showWindow);
		}
		
		internal static void InvokeBrowser(Uri uri, bool showWindow)
		{
			try {
				TraceUtil.WriteLineInfo(null, "Invoke browser: " + uri);
				String uriStr = uri.AbsoluteUri;
				try {
					// First try this.
					InvokeWithProcessStart(uriStr);
				} catch (Exception ex) {
					TraceUtil.WriteLineWarning(null,
											 "Invoke browser Process.Start [" 
											 + uri + "]: " + ex);
					// If Process.Start did not work, then that means
					// SHDocVw is loaded, try invoking with that using
					// reflection.  If MSFT ever fixes that bug, then this
					// code can be removed
					InvokeWithSHDocVw(uriStr);
				}
			} catch (Exception ex) {
				TraceUtil.WriteLineError(null,
										 "Invoke browser [" 
										 + uri + "]: " + ex);
			}
		}
		
		// Makes a transparent, read only text box used to describe
		// something
		internal static RichTextBox MakeDescText(String text, Control con)
		{
			RichTextBox tb = new RichTextBox();
			tb.TabStop = false;
			tb.Text = text;
			tb.BorderStyle = BorderStyle.None;
			tb.Multiline = true;
			tb.AutoSize = true;
			tb.ReadOnly = true;
			tb.WordWrap = true;
			tb.HideSelection = true;
			tb.BackColor = con.BackColor;
			// Get number of actual lines the text box needs 
			// given the current width
			int lines = (1 + tb.GetLineFromCharIndex(tb.TextLength));
			tb.Height = tb.PreferredHeight * lines;
			return tb;
		}
		
		// Makes a button with the specified text
		internal static Button MakeButton(String text)
		{
			Button b = new Button();
			Graphics g = b.CreateGraphics();
			SizeF size = g.MeasureString(text, b.Font);
			b.Height = BUTTON_HEIGHT;
			int proposedWidth = (int)(size.Width + BUTTON_WIDTH_PAD);
			if (b.Width < proposedWidth)
				b.Width = proposedWidth;
			b.Text = text;
			g.Dispose(); 
			return b;
		}
		
		internal static int SetMaxWidth(Control con, String text, int maxWidth)
		{
			Graphics g = con.CreateGraphics();
			int width = (int)g.MeasureString(text, con.Font).Width;
			if (width > maxWidth)   
				maxWidth = width;
			g.Dispose();
			return maxWidth;
		}
		
		internal static String MakeGuidStr(Guid guid)
		{
			return "{" + guid + "}";
		}
		
		// Adds the specified list of controls to the specified
		// control in reverse order so that the layout works
		// correctly.
		internal static void AddControls(Control control, ArrayList controls)
		{
			AddControls(control, controls, 0);
		}
		
		internal static void AddControls(Control control, ArrayList controls, int tabOffset)
		{
			// This is so stupid
			controls.Reverse();
			int len = controls.Count;
			Control[] c = new Control[len];
			Array.Copy(controls.ToArray(), c, len);
			control.Controls.AddRange(c);
			for (int i = 0; i < len; i++)
				control.Controls[i].TabIndex = len - i + tabOffset;
		}
		
		// Adds the specified list control containers to specified
		// control in reverse order so that the layout works
		// correctly.
		internal static void AddHasControls(Control control, ArrayList hasControls)
		{
			AddHasControls(control, hasControls, 0);
		}
		
		internal static void AddHasControls(Control control, ArrayList hasControls, int tabOffset)
		{
			
			// This is so stupid
			hasControls.Reverse();
			int len = hasControls.Count;
			Control[] c = new Control[len];
			for (int i = 0; i < len; i++) {
				c[i] = ((IHasControl)hasControls[i]).GetControl();
				c[i].TabIndex = len - i + tabOffset;
			}
			control.Controls.AddRange(c);
		}
		
		// Works around a problem that some objects (notably Cursor)
		// throw in their Equals method
		internal static bool ContainsHack(ArrayList a, Object obj)
		{
			return FindHack(a, obj) != null;
		}
		
		internal static Object FindHack(ArrayList a, Object obj)
		{
			foreach (Object arrayObj in a) {
				try {
					if (arrayObj.Equals(obj))
						return arrayObj;
				} catch  {
					continue;
				}
			}
			return null;
		}
		
		internal static bool IsArrayListEqual(ArrayList al1, ArrayList al2)
		{
			if (al1 == null && al2 == null)
				return true;
			if (al1 == null || al2 == null ||
				al1.Count != al2.Count)
				return false;
			bool isEqual = true;
			for (int i = 0; i < al2.Count; i++) {
				if (al2[i] != al1[i]) {
					isEqual = false;
					break;
				}
			}
			return isEqual;
		}
	}
}
