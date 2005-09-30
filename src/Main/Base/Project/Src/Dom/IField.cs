// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;

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
