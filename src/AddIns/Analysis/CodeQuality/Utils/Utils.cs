// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using ICSharpCode.CodeQuality.Engine.Dom;
using Mono.Cecil;

namespace ICSharpCode.CodeQuality
{
	/// <summary>
	/// Description of Utils.
	/// </summary>
	public static class Utils
	{
		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);
		
		public static string GetInfoText(this NodeBase left, NodeBase top)
		{
			int item1 = left.GetUses(top);
			int item2 = top.GetUses(left);
			
			string text = GetText(item1, item2);
			
			return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", left.Name, text, top.Name);
		}
		
		static string GetText(int item1, int item2)
		{
			if (item1 == -1 && item2 == -1)
				return "is the same as";
			if (item1 > 0 && item2 > 0)
				return "uses and is used by";
			if (item1 > 0)
				return "uses";
			if (item2 > 0)
				return "is used by";
			return "is not related to";
		}
	}
}
