// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.XmlEditor;

namespace XmlEditor.Tests.Utils
{
	/// <summary>
	/// Mocks the AddElementDialog so we can test the
	/// XmlTreeViewContainerControl class when it displays
	/// the AddElementDialog.
	/// </summary>
	public class MockAddElementDialog : IAddElementDialog
	{
		DialogResult dialogResult = DialogResult.OK;
		List<string> elementNames = new List<string>();
		
		/// <summary>
		/// Specifies the element names to return from the 
		/// IAddElementDialog.ElementNames property.
		/// </summary>
		public void SetElementNamesToReturn(string[] names)
		{
			elementNames.Clear();
			foreach (string name in names) {
				elementNames.Add(name);
			}
		}
		
		/// <summary>
		/// Specifies the dialog result to return from the
		/// IAddElementDialog.ShowDialog method.
		/// </summary>
		public void SetDialogResult(DialogResult result)
		{
			dialogResult = result;
		}
		
		#region IAddElementDialog implementation
		
		public string[] ElementNames {
			get {
				return elementNames.ToArray();
			}
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
