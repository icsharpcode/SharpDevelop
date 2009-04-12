// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;

namespace PythonBinding.Tests.Utils
{
	/// <summary>
	/// Helper class that stores the component name and type.
	/// </summary>
	public class CreatedComponent 
	{		
		public CreatedComponent(string typeName, string name, IComponent component)
		{
			TypeName = typeName;
			Name = name;
			Component = component;
		}
		
		public string TypeName { get; set; }	
		public string Name { get; set; }
		public IComponent Component { get; set; }
		
		public override bool Equals(object obj)
		{
			CreatedComponent rhs = obj as CreatedComponent;
			if (rhs != null) {
				return TypeName == rhs.TypeName && Name == rhs.Name;
			}
			return base.Equals(obj);
		}
		
		public override int GetHashCode()
		{
			return TypeName.GetHashCode() ^ Name.GetHashCode();
		}
		
		public override string ToString()
		{
			return "CreatedComponent [Type=" + TypeName + ", Name=" + Name + "]";
		}
	}
}
