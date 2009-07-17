// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version>$Revision: 4094 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace VBNetBinding
{
	public static class Extensions
	{
		public static T PeekOrDefault<T>(this Stack<T> stack)
		{
			if (stack.Count > 0)
				return stack.Peek();
			
			return default(T);
		}
		
		public static T PopOrDefault<T>(this Stack<T> stack)
		{
			if (stack.Count > 0)
				return stack.Pop();
			
			return default(T);
		}
	}
}
