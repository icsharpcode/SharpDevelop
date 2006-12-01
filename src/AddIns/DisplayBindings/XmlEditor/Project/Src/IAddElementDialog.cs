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
	/// Interface for the AddElementDialog.
	/// </summary>
	public interface IAddElementDialog : IDisposable
	{
		/// <summary>
		/// The element names that should be added. These are the
		/// element names that the user selected in the dialog when
		/// it was closed.
		/// </summary>
		string[] ElementNames {get;}
		
		/// <summary>
		/// Shows the dialog.
		/// </summary>
		DialogResult ShowDialog();
	}
}
