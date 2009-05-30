// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2229 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.XmlEditor;

namespace XmlEditor.Tests.Utils
{
	/// <summary>
	/// Mocks the AddXmlNodeDialog so we can test the
	/// XmlTreeViewContainerControl class when it displays
	/// the AddElementDialog or the AddAttributeDialog.
	/// </summary>
	public class MockAddXmlNodeDialog : IAddXmlNodeDialog
	{
		DialogResult dialogResult = DialogResult.OK;
		List<string> names = new List<string>();
		
		/// <summary>
		/// Specifies the names to return from the 
		/// IAddXmlNodeDialog.GetNames method.
		/// </summary>
		public void SetNamesToReturn(string[] names)
		{
			this.names.Clear();
			foreach (string name in names) {
				this.names.Add(name);
			}
		}
		
		/// <summary>
		/// Specifies the dialog result to return from the
		/// IAddXmlNodeDialog.ShowDialog method.
		/// </summary>
		public void SetDialogResult(DialogResult result)
		{
			dialogResult = result;
		}
		
		#region IAddXmlNodeDialog implementation
		
		public string[] GetNames()
		{
			return names.ToArray();
		}
		
		public DialogResult ShowDialog()
		{
			return dialogResult;
		}
		
		public void Dispose()
		{
		}
		
		#endregion
	}
}
