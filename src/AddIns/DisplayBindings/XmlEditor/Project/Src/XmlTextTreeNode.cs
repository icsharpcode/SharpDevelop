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
	/// Represents an XmlText node in the tree.
	/// </summary>
	public class XmlTextTreeNode : XmlCharacterDataTreeNode
	{
		public const string XmlTextTreeNodeImageKey = "XmlTextTreeNodeImage";
		public const string XmlTextTreeNodeGhostImageKey = "XmlTextTreeNodeGhostImage";
		
		XmlText xmlText;
		
		public XmlTextTreeNode(XmlText xmlText)
			: base(xmlText)
		{
			this.xmlText = xmlText;
			ImageKey = XmlTextTreeNodeImageKey;
			SelectedImageKey = ImageKey;
			Update();
		}
		
		/// <summary>
		/// Gets the XmlText associated with this tree node.
		/// </summary>
		public XmlText XmlText {
			get {
				return xmlText;
			}
		}
		
		/// <summary>
		/// Gets or sets whether to show the ghost image which is 
		/// displayed when cutting the node.
		/// </summary>
		public bool ShowGhostImage {
			get {
				return ImageKey == XmlTextTreeNodeGhostImageKey;
			}
			set {
				if (value) {
					ImageKey = XmlTextTreeNodeGhostImageKey;
				} else {
					ImageKey = XmlTextTreeNodeImageKey;
				}
				SelectedImageKey = ImageKey;
			}
		}
	}
}
