// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Description of PythonFormVisitor.
	/// </summary>
	public class PythonFormVisitor
	{
		public PythonFormVisitor()
		{
		}
		
		/// <summary>
		/// Creates a form from python code.
		/// </summary>
		public Form CreateForm(string pythonCode, IComponentCreator componentCreator)
		{
			Form form = (Form)componentCreator.CreateComponent(typeof(Form), "MainForm");
			form.Name = "MainForm";
			return form;
		}		
	}
}
