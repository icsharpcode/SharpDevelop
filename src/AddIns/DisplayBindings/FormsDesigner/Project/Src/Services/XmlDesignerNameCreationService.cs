// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
