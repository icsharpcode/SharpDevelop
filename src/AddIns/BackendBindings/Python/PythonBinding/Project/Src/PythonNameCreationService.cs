// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using ICSharpCode.FormsDesigner;

namespace ICSharpCode.PythonBinding
{
	public class PythonNameCreationService : INameCreationService
	{
		IDesignerHost host;
		XmlDesignerLoader.NameCreationService nameCreationService;
		
		public PythonNameCreationService(IDesignerHost host)
		{
			this.host = host;
			nameCreationService = new XmlDesignerLoader.NameCreationService(host);
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
