// 
// TypeScriptWorkbenchMonitor.cs
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
using System.IO;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.TypeScriptBinding.Hosting;

namespace ICSharpCode.TypeScriptBinding
{
	public class TypeScriptWorkbenchMonitor
	{
		TypeScriptContextProvider provider;
		
		public TypeScriptWorkbenchMonitor(
			IWorkbench workbench,
			TypeScriptContextProvider provider)
		{
			workbench.ViewOpened += ViewOpened;
			workbench.ViewClosed += ViewClosed;
			this.provider = provider;
		}
		
		void ViewOpened(object sender, ViewContentEventArgs e)
		{
			if (StandaloneTypeScriptFileOpened(e)) {
				CreateTypeScriptContext(e.Content);
			}
		}
		
		void CreateTypeScriptContext(IViewContent view)
		{
			provider.CreateContext(view.PrimaryFileName, GetText(view));
		}
		
		string GetText(IViewContent view)
		{
			ITextEditor textEditor = view.GetService<ITextEditor>();
			return textEditor.Document.Text;
		}
		
		bool StandaloneTypeScriptFileOpened(ViewContentEventArgs e)
		{
			return StandaloneTypeScriptFileOpened(e.Content.PrimaryFileName);
		}
		
		bool StandaloneTypeScriptFileOpened(FileName fileName)
		{
			return TypeScriptParser.IsTypeScriptFileName(fileName) &&
				!TypeScriptFileInAnyProject(fileName);
		}
		
		bool TypeScriptFileInAnyProject(FileName fileName)
		{
			return TypeScriptService.ContextProvider.IsFileInsideProject(fileName);
		}
		
		void ViewClosed(object sender, ViewContentEventArgs e)
		{
			FileName fileName = e.Content.PrimaryFileName;
			if (TypeScriptParser.IsTypeScriptFileName(fileName)) {
				if (TypeScriptFileInAnyProject(fileName)) {
					UpdateTypeScriptContextWithFileContentFromDisk(fileName);
				} else {
					provider.DisposeContext(fileName);
				}
			}
		}
		
		void UpdateTypeScriptContextWithFileContentFromDisk(FileName fileName)
		{
			if (File.Exists(fileName)) {
				TypeScriptContext context = TypeScriptService.ContextProvider.GetContext(fileName);
				string fileContent = FileReader.ReadFileContent(fileName, SD.FileService.DefaultFileEncoding);
				context.UpdateFile(fileName, fileContent);
			}
		}
	}
}
