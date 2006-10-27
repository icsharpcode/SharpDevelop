// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IField : IMember
	{
		/// <summary>Gets if this field is a local variable that has been converted into a field.</summary>
		bool IsLocalVariable { get; }
		
		/// <summary>Gets if this field is a parameter that has been converted into a field.</summary>
		bool IsParameter { get; }
	}
}
