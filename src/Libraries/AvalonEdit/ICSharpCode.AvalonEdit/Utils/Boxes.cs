// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.AvalonEdit.Utils
{
	/// <summary>
	/// Reuse the same instances for boxed booleans.
	/// </summary>
	static class Boxes
	{
		public static readonly object True = true;
		public static readonly object False = false;
		
		public static object Box(bool value)
		{
			return value ? True : False;
		}
	}
}
