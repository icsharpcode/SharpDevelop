// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Resources;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class ViewGPLDialog : BaseSharpDevelopForm 
	{
		public ViewGPLDialog()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.ViewGPLDialog.xfrm"));
			LoadGPL();
		}
		
		void LoadGPL()
		{
			string filename = FileUtility.SharpDevelopRootPath + 
			                  Path.DirectorySeparatorChar + "doc" +
			                  Path.DirectorySeparatorChar + "license.txt";
			if (FileUtility.TestFileExists(filename)) {
				RichTextBox licenseRichTextBox = (RichTextBox)ControlDictionary["licenseRichTextBox"];
				licenseRichTextBox.LoadFile(filename, RichTextBoxStreamType.PlainText);
			}
		}
	}
}
