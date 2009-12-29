// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Text;

namespace RubyBinding.Tests.Utils
{
	/// <summary>
	/// Helper class that stores the parameters passed when the IComponentCreator.CreateInstance method is called.
	/// </summary>
	public class CreatedInstance
	{
		public CreatedInstance(Type type, ICollection arguments, string name, bool addToContainer)
		{
			InstanceType = type;
			Arguments = arguments;
			Name = name;
			AddToContainer = addToContainer;
		}
		
		public Type InstanceType { get; set; }
		public ICollection Arguments { get; set; }
		public string Name { get; set; }
		public bool AddToContainer { get; set; }
		public object Object { get; set; }
		
		public override bool Equals(object obj)
		{
			CreatedInstance rhs = obj as CreatedInstance;
			if (rhs != null) {
				return ToString() == rhs.ToString();
			}
			return base.Equals(obj);
		}
		
		public override int GetHashCode()
		{
			return InstanceType.GetHashCode();
		}
		
		public override string ToString()
		{
			return "CreatedInstance [Type=" + InstanceType + ", Name=" + Name + ArgsToString() + "]";
		}
		
		string ArgsToString()
		{
			StringBuilder s = new StringBuilder();
			s.Append("[Args=");
			
			bool firstArg = true;
			foreach (object o in Arguments) {
				if (!firstArg) {
					s.Append(", ");
				}
				s.Append(o);
				firstArg = false;
			}
			s.Append("]");
			return s.ToString();
		}
	}
}
