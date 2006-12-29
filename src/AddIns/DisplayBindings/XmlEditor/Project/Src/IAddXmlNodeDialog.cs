// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>


using System;
using System.Windows.Forms;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Interface for the AddXmlNodeDialog.
	/// </summary>
	public interface IAddXmlNodeDialog : IDisposable
	{
		/// <summary>
		/// The names that should be added. These are the
		/// names that the user selected in the dialog when
		/// it was closed.
		/// </summary>
		string[] GetNames();
		
		/// <summary>
		/// Shows the dialog.
		/// </summary>
		DialogResult ShowDialog();
	}
}
