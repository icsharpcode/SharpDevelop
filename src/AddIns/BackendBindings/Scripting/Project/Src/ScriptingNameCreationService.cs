// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using ICSharpCode.FormsDesigner;

namespace ICSharpCode.Scripting
{
	public class ScriptingNameCreationService : INameCreationService
	{
		IDesignerHost host;
		XmlDesignerNameCreationService nameCreationService;
		
		public ScriptingNameCreationService(IDesignerHost host)
		{
			this.host = host;
			nameCreationService = new XmlDesignerNameCreationService(host);
		}
		
		public string CreateName(IContainer container, Type dataType)
		{
			return nameCreationService.CreateName(container, dataType);
		}
		
		public bool IsValidName(string name)
		{
			if (!nameCreationService.IsValidName(name)) {
				return false;
			}
			
			if (host.Container != null) {
				return host.Container.Components[name] == null;
			}
			return true;
		}
		
		public void ValidateName(string name)
		{
			if (!IsValidName(name)) {
				throw new Exception("Invalid name " + name);
			}
		}
	}
}
