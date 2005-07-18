// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Reflection;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IMethodOrIndexer : IMember
	{
		IRegion BodyRegion {
			get;
		}
		
		List<IParameter> Parameters {
			get;
		}
	}
	
	public interface IMethod : IMethodOrIndexer
	{
		List<ITypeParameter> TypeParameters {
			get;
		}
		
		bool IsConstructor {
			get;
		}
	}
}
