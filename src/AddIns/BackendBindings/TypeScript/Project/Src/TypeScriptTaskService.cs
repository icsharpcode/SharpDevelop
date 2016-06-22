// 
// TypeScriptTaskService.cs
// 
// Author:
//   Matt Ward <ward.matt@gmail.com>
// 
// Copyright (C) 2014 Matthew Ward
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.TypeScriptBinding.Hosting;

namespace ICSharpCode.TypeScriptBinding
{
	public class TypeScriptTaskService
	{
		public void Update(Diagnostic[] diagnostics, FileName fileName)
		{
			SD.MainThread.InvokeIfRequired(() => {
				ClearTasksForFileName(fileName);
				
				IViewContent view = FileService.GetOpenFile(fileName);
				if (view == null)
					return;
				
				ITextEditor textEditor = view.GetService<ITextEditor>();
				if (textEditor == null)
					return;
				
				List<TypeScriptTask> tasks = diagnostics
					.Select(diagnostic => TypeScriptTask.Create(fileName, diagnostic, textEditor.Document))
					.ToList();
				
				TaskService.AddRange(tasks);
			});
		}
		
		void ClearTasksForFileName(FileName fileName)
		{
			List<TypeScriptTask> tasks = TaskService
				.Tasks
				.OfType<TypeScriptTask>()
				.Where(t => t.FileName == fileName)
				.ToList();
			
			foreach (SDTask task in tasks) {
				TaskService.Remove(task);
			}
		}
	}
}
