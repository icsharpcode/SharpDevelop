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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.FormsDesigner.Services;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.FormsDesigner
{
	public class XmlDesignerNameCreationService : INameCreationService
	{
		IDesignerHost host;
		
		public XmlDesignerNameCreationService(IDesignerHost host)
		{
			this.host = host;
		}
		
		public string CreateName(Type dataType)
		{
			return CreateName(host.Container, dataType);
		}
		
		public string CreateName(IContainer container, Type dataType)
		{
			string name = Char.ToLower(dataType.Name[0]) + dataType.Name.Substring(1);
			int number = 1;
			if (container != null) {
				while (container.Components[name + number.ToString()] != null) {
					++number;
				}
			}
			return name + number.ToString();
		}
		
		public bool IsValidName(string name)
		{
			if (name == null || name.Length == 0 || !(Char.IsLetter(name[0]) || name[0] == '_')) {
				return false;
			}
			
			foreach (char ch in name) {
				if (!Char.IsLetterOrDigit(ch) && ch != '_') {
					return false;
				}
			}
			
			return true;
		}
		
		public void ValidateName(string name)
		{
			if (!IsValidName(name)) {
				throw new System.Exception("Invalid name " + name);
			}
		}
	}
}
