// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	[Serializable]
	[Flags]
	public enum ParameterModifiers : byte
	{
		// Values must be the same as in NRefactory's ParamModifiers
		None = 0,
		In  = 1,
		Out = 2,
		Ref = 4,
		Params = 8,
		Optional = 16
	}
}
