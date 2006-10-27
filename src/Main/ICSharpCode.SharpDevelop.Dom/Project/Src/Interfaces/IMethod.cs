// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IMethodOrProperty : IMember
	{
		DomRegion BodyRegion {
			get;
		}
		
		IList<IParameter> Parameters {
			get;
		}
		
		bool IsExtensionMethod {
			get;
		}
	}
	
	public interface IMethod : IMethodOrProperty
	{
		IList<ITypeParameter> TypeParameters {
			get;
		}
		
		bool IsConstructor {
			get;
		}
	}
}
