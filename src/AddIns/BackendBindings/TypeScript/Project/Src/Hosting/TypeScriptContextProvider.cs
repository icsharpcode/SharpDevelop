// 
// TypeScriptContextProvider.cs
// 
// Author:
//   Matt Ward <ward.matt@gmail.com>
// 
// Copyright (C) 2013 Matthew Ward
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
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.TypeScriptBinding.Hosting
{
	public class TypeScriptContextProvider
	{
		ITypeScriptContextFactory factory;
		Dictionary<FileName, TypeScriptContext> cachedContexts = 
			new Dictionary<FileName, TypeScriptContext>();
		
		Dictionary<FileName, TypeScriptContext> cachedContextsInsideProjects = 
			new Dictionary<FileName, TypeScriptContext>();
		
		List<TypeScriptContext> projectContexts = new List<TypeScriptContext>();
		
		public TypeScriptContextProvider()
			: this(new TypeScriptContextFactory())
		{
		}
		
		public TypeScriptContextProvider(ITypeScriptContextFactory factory)
		{
			this.factory = factory;
		}
		
		public TypeScriptContext CreateContext(FileName fileName, string text)
		{
			TypeScriptContext context = factory.CreateContext();
			context.AddFile(fileName, text);
			context.RunInitialisationScript();
			context.GetCompletionItemsForTheFirstTime();
			
			cachedContexts.Add(fileName, context);
			
			return context;
		}
		
		public TypeScriptContext GetContext(FileName fileName)
		{
			TypeScriptContext context = null;
			if (cachedContexts.TryGetValue(fileName, out context)) {
				return context;
			} else if (cachedContextsInsideProjects.TryGetValue(fileName, out context)) {
				return context;
			}
			return null;
		}
		
		public bool IsFileInsideProject(FileName fileName)
		{
			return cachedContextsInsideProjects.ContainsKey(fileName);
		}
		
		public void DisposeContext(FileName fileName)
		{
			TypeScriptContext context = GetContext(fileName);
			if (context != null) {
				context.Dispose();
			}
			cachedContexts.Remove(fileName);
			cachedContextsInsideProjects.Remove(fileName);
		}
		
		public TypeScriptContext CreateProjectContext(TypeScriptProject project)
		{
			TypeScriptContext context = factory.CreateContext();
			projectContexts.Add(context);
			
			foreach (FileName typeScriptFileName in project.GetTypeScriptFileNames()) {
				AddFileToProjectContext(context, typeScriptFileName);
			}
			
			context.RunInitialisationScript();
			context.UseProjectForOptions(project);
			context.GetCompletionItemsForTheFirstTime();
			
			return context;
		}
		
		void AddFileToProjectContext(TypeScriptContext context, FileName fileName)
		{
			cachedContextsInsideProjects.Add(fileName, context);
			ITextSource fileContent = SD.FileService.GetFileContent(fileName);
			context.AddFile(fileName, fileContent.Text);
		}
		
		public void DisposeAllProjectContexts()
		{
			foreach (TypeScriptContext context in projectContexts) {
				context.Dispose();
			}
			cachedContextsInsideProjects = new Dictionary<FileName, TypeScriptContext>();
			projectContexts = new List<TypeScriptContext>();
		}
		
		public void AddFileToProjectContext(TypeScriptProject project, FileName fileName)
		{
			if (projectContexts.Count == 0) {
				CreateProjectContext(project);
			} else {
				TypeScriptContext context = projectContexts[0];
				AddFileToProjectContext(context, fileName);
			}
		}
	}
}
