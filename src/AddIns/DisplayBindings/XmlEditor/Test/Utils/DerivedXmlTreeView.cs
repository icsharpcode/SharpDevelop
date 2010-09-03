// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
