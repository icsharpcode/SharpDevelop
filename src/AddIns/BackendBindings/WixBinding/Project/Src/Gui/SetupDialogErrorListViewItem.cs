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
using System.Drawing;
using System.IO;
using System.Xml;

namespace ICSharpCode.WixBinding
{
	public class SetupDialogErrorListViewItem : SetupDialogListViewItem
	{
		int line;
		int column;
		
		public SetupDialogErrorListViewItem(string fileName, XmlException ex)
			: this(fileName, ex.LineNumber, ex.LinePosition)
		{
		}
		
		public SetupDialogErrorListViewItem(string fileName)
			: this(fileName, 0, 0)
		{
		}
		
		public SetupDialogErrorListViewItem(string fileName, int line, int column) : base(fileName, String.Empty)
		{
			Text = Path.GetFileName(fileName);
			this.line = line;
			this.column = column;
			ForeColor = Color.White;
			BackColor = Color.Red;
		}
		
		/// <summary>
		/// Gets the line position of the error.
		/// </summary>
		public int Line {
			get {
				return line;
			}
		}
		
		/// <summary>
		/// Gets the column position of the error.
		/// </summary>
		public int Column {
			get {
				return column;
			}
		}
	}
}
