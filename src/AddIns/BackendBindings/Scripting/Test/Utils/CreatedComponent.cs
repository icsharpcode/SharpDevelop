// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;

namespace ICSharpCode.Scripting.Tests.Utils
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
		
		public CreatedComponent(string typeName, string name)
		{
			TypeName = typeName;
			Name = name;
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
