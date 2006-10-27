// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
			: this(fileName, ex.LineNumber - 1, ex.LinePosition - 1)
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
