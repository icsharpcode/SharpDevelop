// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Documents;
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
		
		static readonly DependencyColorizer colorizer = new DependencyColorizer();
		
		public static object GetInfoText(this NodeBase left, NodeBase top)
		{
			int item1 = left.GetUses(top);
			int item2 = top.GetUses(left);
			var uses = Tuple.Create(item1, item2);
			
			string text = colorizer.GetDescription(uses);
			
			return new TextBlock {
				Inlines = {
					left.Name,
					new Bold {
						Inlines = {
							" " + text + " "
						},
						Foreground = colorizer.GetColorBrush(uses)
					},
					top.Name
				}
			};
		}
	}
}
