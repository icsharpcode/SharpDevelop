// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace WixBinding.Tests.Utils
{
	/// <summary>
	/// Helper class that stores the component name and type.
	/// </summary>
	public class CreatedComponent 
	{
		string typeName = String.Empty;
		string name = String.Empty;
		
		public CreatedComponent(string typeName, string name)
		{
			this.typeName = typeName;
			this.name = name;
		}
		
		public string TypeName {
			get {
				return typeName;
			}
		}
		
		public string Name {
			get {
				return name;
			}
		}
	}
}
