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
	/// Mocks the AddAttributeDialog so we can test the
	/// XmlTreeViewContainerControl class when it displays
	/// the AddAttributeDialog.
	/// </summary>
	public class MockAddAttributeDialog : IAddAttributeDialog
	{
		DialogResult dialogResult = DialogResult.OK;
		List<string> attributeNames = new List<string>();
		
		/// <summary>
		/// Specifies the attribute names to return from the 
		/// IAddAttributeDialog.AttributeNames property.
		/// </summary>
		public void SetAttributeNamesToReturn(string[] names)
		{
			attributeNames.Clear();
			foreach (string name in names) {
				attributeNames.Add(name);
			}
		}
		
		/// <summary>
		/// Specifies the dialog result to return from the
		/// IAddAttributeDialog.ShowDialog method.
		/// </summary>
		public void SetDialogResult(DialogResult result)
		{
			dialogResult = result;
		}
		
		#region IAddAttributeDialog implementation
		
		public string[] AttributeNames {
			get {
				return attributeNames.ToArray();
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
