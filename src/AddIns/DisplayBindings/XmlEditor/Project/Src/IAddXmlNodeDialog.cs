// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
