// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Base class for XmlTextTreeNodes and XmlCommentTreeNodes
	/// </summary>
	public abstract class XmlCharacterDataTreeNode : ExtTreeNode
	{
		XmlCharacterData characterData;
		
		protected XmlCharacterDataTreeNode(XmlCharacterData characterData)
		{
			this.characterData = characterData;
		}
		
		/// <summary>
		/// Updates the display text based on changes in the
		/// XmlCharacterData's InnerText associated with this node.
		/// </summary>
		public void Update()
		{
			Text = GetDisplayText(characterData.InnerText);
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
						return string.Concat(line, "...");
					}
				}
			}
			return string.Empty;
		}
	}
}
