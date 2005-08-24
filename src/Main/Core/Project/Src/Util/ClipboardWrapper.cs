/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 24.08.2005
 * Time: 11:38
 */

using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Helper class to access the clipboard without worrying about ExternalExceptions
	/// </summary>
	public static class ClipboardWrapper
	{
		public static bool ContainsText {
			get {
				try {
					return Clipboard.ContainsText();
				} catch (ExternalException) {
					return false;
				}
			}
		}
		
		public static string GetText()
		{
			// retry 2 times should be enough for read access
			try {
				return Clipboard.GetText();
			} catch (ExternalException) {
				return Clipboard.GetText();
			}
		}
		
		public static void SetText(string text)
		{
			DataObject data = new DataObject();
			data.SetData(DataFormats.UnicodeText, true, text);
			SetDataObject(data);
		}
		
		public static IDataObject GetDataObject()
		{
			// retry 2 times should be enough for read access
			try {
				return Clipboard.GetDataObject();
			} catch (ExternalException) {
				return Clipboard.GetDataObject();
			}
		}
		
		public static void SetDataObject(object data)
		{
			Clipboard.SetDataObject(data, true, 50, 50);
		}
	}
}
