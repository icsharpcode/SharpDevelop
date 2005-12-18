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
	public interface IEvent : IMember
	{
		DomRegion BodyRegion {
			get;
		}
		
		IMethod AddMethod {
			get;
		}
		
		IMethod RemoveMethod {
			get;
		}
		
		IMethod RaiseMethod {
			get;
		}
	}
}
