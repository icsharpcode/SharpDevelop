// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
