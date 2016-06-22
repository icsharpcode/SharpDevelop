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
