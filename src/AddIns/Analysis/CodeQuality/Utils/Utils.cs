// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
