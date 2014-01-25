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
using System.Linq;
using System.Text;
using System.Windows;

using ICSharpCode.Profiler.Controls;

namespace ICSharpCode.Profiler.AddIn.Commands
{
	/// <summary>
	/// Description of CopySelectedData
	/// </summary>
	public class CopySelectedData : ProfilerMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			var list = GetSelectedItems();
			
			if (list.FirstOrDefault() != null) {
				StringBuilder builder = new StringBuilder();
				
				foreach (CallTreeNodeViewModel node in list) {
					if (node != null)
						builder.AppendLine(
							new string('\t', node.Level) +
							node.Name + "\t" +
							node.CallCount + "\t" +
							node.TimeSpent + "\t" +
							node.TimeSpentSelf + "\t" +
							node.TimeSpentPerCall + "\t" +
							node.TimeSpentSelfPerCall + "\t" +
							node.TimePercentageOfParentAsText
						);
				}
				
				Clipboard.SetText(builder.ToString());
			}
		}
	}
}
