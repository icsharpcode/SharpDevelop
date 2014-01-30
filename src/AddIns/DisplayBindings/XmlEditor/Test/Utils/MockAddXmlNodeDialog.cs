// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
