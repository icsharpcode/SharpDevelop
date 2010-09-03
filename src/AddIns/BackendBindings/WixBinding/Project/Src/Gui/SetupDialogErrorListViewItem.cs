// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
