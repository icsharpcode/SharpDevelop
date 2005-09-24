// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
				return Clipboard.GetDataObject();
			}
		}
		
		public static void SetDataObject(object data)
		{
			int i = 0;
			while (true) {
				try {
					Clipboard.SetDataObject(data, true, 5, 50);
					return;
				} catch (ExternalException) {
					if (i++ > 5)
						throw;
				}
				System.Threading.Thread.Sleep(50);
				Application.DoEvents();
				System.Threading.Thread.Sleep(50);
			}
		}
	}
}
