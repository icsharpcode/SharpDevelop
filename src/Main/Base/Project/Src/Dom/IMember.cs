// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IMember : IDecoration, ICloneable
	{
		string FullyQualifiedName {
			get;
		}
		
		DomRegion Region {
			get;
		}
		
		string Name {
			get;
		}
		
		string Namespace {
			get;
		}
		
		string DotNetName {
			get;
		}
		
		IReturnType ReturnType {
			get;
			set;
		}
	}
}
