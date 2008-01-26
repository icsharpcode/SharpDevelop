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

	public interface IParameter : IFreezable, IComparable
	{
		string Name {
			get;
		}

		IReturnType ReturnType {
			get;
			set;
		}

		IList<IAttribute> Attributes {
			get;
		}

		ParameterModifiers Modifiers {
			get;
		}
		
		DomRegion Region {
			get;
		}
		
		string Documentation {
			get;
		}

		bool IsOut {
			get;
		}

		bool IsRef {
			get;
		}

		bool IsParams {
			get;
		}
		
		bool IsOptional {
			get;
		}
	}
}
