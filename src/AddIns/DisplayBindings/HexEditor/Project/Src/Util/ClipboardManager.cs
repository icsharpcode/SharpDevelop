// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 2984 $</version>
// </file>

using System;
using System.Windows.Forms;

namespace HexEditor.Util
{
	/// <summary>
	/// Manages the clipboard actions.
	/// </summary>
	public static class ClipboardManager
	{
 		/// <summary>
		/// Used to determine if text is in the clipboard or not.
		/// </summary>
		public static bool ContainsText {
			get {
				return Clipboard.ContainsText();
			}
		}
		
		/// <summary>
		/// Cleares the Clipboard.
		/// </summary>
		public static void Clear()
		{
			Clipboard.Clear();
		}
		
		/// <summary>
		/// Copies text into the clipboard.
		/// </summary>
		/// <param name="text">The text to be copied to the clipboard.</param>
		public static void Copy(string text)
		{
			Clipboard.SetText(text);
		}
		
		/// <summary>
		/// Pastes the text.
		/// </summary>
		/// <returns>the text in the clipboard.</returns>
		public static string Paste()
		{
			return Clipboard.GetText();
		}
	}
}
