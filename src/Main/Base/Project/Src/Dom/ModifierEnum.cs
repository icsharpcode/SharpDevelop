// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	[Flags]
	public enum ModifierEnum // must be the same values as NRefactories' ModifierEnum
	{
		None       = 0,
		
		// Access
		Private   = 0x0001,
		Internal  = 0x0002, // == Friend
		Protected = 0x0004,
		Public    = 0x0008,
		Dim	      = 0x0010,	// VB.NET SPECIFIC
		
		// Scope
		Abstract  = 0x0010,  // == 	MustOverride/MustInherit
		Virtual   = 0x0020,
		Sealed    = 0x0040,
		Static    = 0x0080,
		Override  = 0x0100,
		Readonly  = 0x0200,
		Const	  = 0x0400,
		New       = 0x0800,  // == Shadows
		Partial   = 0x1000,
		
		// Special
		Extern     = 0x2000,
		Volatile   = 0x4000,
		Unsafe     = 0x8000,
		Overloads  = 0x10000, // VB specific
		WithEvents = 0x20000, // VB specific
		Default    = 0x40000, // VB specific
		Narrowing  = 0x80000, // VB specific
		Widening  = 0x100000, // VB specific
		Synthetic = 0x200000,
		
		ProtectedAndInternal = Internal | Protected,
		VisibilityMask = Private | Internal | Protected | Public,
	}
}
