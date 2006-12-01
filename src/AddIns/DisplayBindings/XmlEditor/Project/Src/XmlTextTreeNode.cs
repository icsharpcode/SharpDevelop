// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XmlEditor
{
	public class XmlTextTreeNode : ExtTreeNode
	{
		public const string XmlTextTreeNodeImageKey = "XmlTextTreeNodeImage";
		
		XmlText xmlText;
		
		public XmlTextTreeNode(XmlText xmlText)
		{
			this.xmlText = xmlText;
			ImageKey = XmlTextTreeNodeImageKey;
			SelectedImageKey = ImageKey;
			Update();
		}
		
		public XmlText XmlText {
			get {
				return xmlText;
			}
		}
		
		/// <summary>
		/// Updates the display text based on changes in the
		/// XmlText associated with this node.
		/// </summary>
		public void Update()
		{
			Text = GetDisplayText(xmlText.InnerText);
		}
		
		/// <summary>
		/// Gets the text to display for this tree node.
		/// </summary>
		/// <remarks>If the text is a single line then it is returned, but
		/// trimmed. If the text has multiple lines then the first line that
		/// is not empty is returned. This line may have "..." appended to indicate 
		/// there is more text for this node that is not being displayed. The
		/// "..." will be appended only if there are multiple lines containing 
		/// text.</remarks>
		static string GetDisplayText(string s)
		{
			string[] lines = s.Trim().Split('\n');
			for (int i = 0; i < lines.Length; ++i) {
				string line = lines[i].Trim();
				if (line.Length > 0) {
					if (lines.Length == 1) {
						return line;
					} else {
						return String.Concat(line, "...");
					}
				}
			}
			return String.Empty;
		}
	}
}
