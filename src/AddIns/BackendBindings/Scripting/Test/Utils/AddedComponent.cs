// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;

namespace ICSharpCode.Scripting.Tests.Utils
{
	/// <summary>
	/// Component passed to the IComponentCreator.Add method.
	/// </summary>
	public class AddedComponent
	{
		public AddedComponent(IComponent component, string name)
		{
			Component = component;
			Name = name;
		}
		
		public string Name { get; set; }
		public IComponent Component { get; set; }
		
		public override bool Equals(object obj)
		{
			AddedComponent rhs = obj as AddedComponent;
			if (rhs != null) {
				return Name == rhs.Name && Component == rhs.Component;
			}
			return base.Equals(obj);
		}
		
		public override int GetHashCode()
		{
			return Component.GetHashCode() ^ Name.GetHashCode();
		}
		
		public override string ToString()
		{
			return "AddedComponent [Component=" + GetComponentTypeName() + "Name=" + Name + "]";
		}
		
		string GetComponentTypeName()
		{
			if (Component != null) {
				return Component.GetType().Name;
			}
			return "<null>";
		}
	}
}
