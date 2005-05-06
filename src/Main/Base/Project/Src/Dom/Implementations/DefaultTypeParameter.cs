// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.Reflection;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Type parameter of a generic class/method.
	/// </summary>
	public class DefaultTypeParameter : ITypeParameter
	{
		string name;
		
		public string Name {
			get {
				return name;
			}
		}
		
		public DefaultTypeParameter(string name)
		{
			this.name = name;
		}
		
		public DefaultTypeParameter(Type type)
		{
			this.name = type.Name;
		}
	}
}
