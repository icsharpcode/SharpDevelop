// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IAttribute : IComparable
	{
		AttributeTarget AttributeTarget {
			get;
		}
		
		string Name {
			get;
		}
		
		List<AttributeArgument> PositionalArguments {
			get;
		}
		
		SortedList<string, AttributeArgument> NamedArguments {
			get;
		}
	}
	
	public enum AttributeTarget
	{
		None,
		Assembly,
		Field,
		Event,
		Method,
		Module,
		Param,
		Property,
		Return,
		Type
	}
	
	public struct AttributeArgument
	{
		public readonly IReturnType Type;
		public readonly object Value;
		
		public AttributeArgument(IReturnType type, object value)
		{
			this.Type = type;
			this.Value = value;
		}
	}
}
