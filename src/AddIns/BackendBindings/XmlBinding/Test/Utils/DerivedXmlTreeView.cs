// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2164 $</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.XmlEditor;

namespace XmlEditor.Tests.Utils
{
	/// <summary>
	/// Test utility class that has the XmlTreeViewControl as a base class
	/// and gives access to protected methods.
	/// </summary>
	public class DerivedXmlTreeViewControl : XmlTreeViewControl
	{
		public DerivedXmlTreeViewControl()
		{
		}
		
		public void CallMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
		}
		
		public void CallProcessCmdKey(Keys keys)
		{
			Message msg = new Message();
			base.ProcessCmdKey(ref msg, keys);
		}
	}
}
