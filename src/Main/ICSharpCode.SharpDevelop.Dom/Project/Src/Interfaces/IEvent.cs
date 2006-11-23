// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IEvent : IMember
	{
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
