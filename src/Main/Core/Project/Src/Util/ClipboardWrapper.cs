// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
					LoggingService.Debug("ContainsText called");
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
				try {
					return Clipboard.GetDataObject();
				} catch (ExternalException) {
					return new DataObject();
				}
			}
		}
		
		public static void SetDataObject(object data)
		{
			try {
				Clipboard.SetDataObject(data, true, 10, 50);
			} catch (ExternalException) {
				Application.DoEvents();
				try {
					Clipboard.SetDataObject(data, true, 10, 50);
				} catch (ExternalException) { }
			}
		}
	}
}
