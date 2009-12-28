// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;

namespace RubyBinding.Tests.Utils
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
