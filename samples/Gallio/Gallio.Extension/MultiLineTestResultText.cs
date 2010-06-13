// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;

namespace Gallio.Extension
{
	public class MultiLineTestResultText
	{
		StringBuilder textBuilder = new StringBuilder();
		
		public MultiLineTestResultText(string text)
		{
			EncodeText(text);
		}
		
		public override string ToString()
		{
			return textBuilder.ToString();
		}
		
		/// <summary>
		/// Replaces the first character on each new line with a space.
		/// The first line does not have the extra space added.
		/// </summary>
		void EncodeText(string text)
		{
			if (String.IsNullOrEmpty(text)) {
				return;
			}
			
			text = text.TrimEnd(Environment.NewLine.ToCharArray());
			
			foreach (char ch in text) {
				switch (ch) {
					case '\n':
						textBuilder.Append("\r\n ");
						break;
					case '\r':
						// Ignore.
						break;
					default:
						textBuilder.Append(ch);
						break;
				}
			}
		}
	}
}
