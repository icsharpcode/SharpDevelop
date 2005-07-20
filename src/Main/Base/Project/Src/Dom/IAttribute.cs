// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;

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
		
		ArrayList PositionalArguments { // [expression]
			get;
		}
		
		SortedList NamedArguments { // string/expression
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
}
