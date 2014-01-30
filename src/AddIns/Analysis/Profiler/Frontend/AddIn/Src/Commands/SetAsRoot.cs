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

using ICSharpCode.Core;
using ICSharpCode.Profiler.Controller.Data;
using ICSharpCode.Profiler.Controller.Queries;

namespace ICSharpCode.Profiler.AddIn.Commands
{
	/// <summary>
	/// Description of SetAsRoot
	/// </summary>
	public class SetAsRoot : ProfilerMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			var items = GetSelectedItems().Where(item => item != null && item.Node != null).Select(i => i.Node);
			
			if (items.FirstOrDefault() != null) {
				List<string> parts = new List<string>();
				
				int? nameId = items.First().NameMapping.Id; // use nullable int to represent non assigned nameId
				
				foreach (CallTreeNode node in items) {
					if (nameId != null && nameId != node.NameMapping.Id)
						nameId = null;
					foreach (var path in node.GetPath())
						parts.Add("GetNodeByPath(" + string.Join(",", path.Select(i => i.ToString()).ToArray()) + ")");
				}
				
				string header = string.Format(StringParser.Parse("${res:AddIns.Profiler.Commands.SetAsRoot.TabTitle}:"), items.First().Name);
				if (nameId == null)
					header = StringParser.Parse("${res:AddIns.Profiler.Commands.SetAsRoot.TabTitle}:");
				
				Parent.CreateTab(header, "Merge(" + string.Join(",", parts.ToArray()) + ")");
			}	
		}
	}
}
