// 
// CompileTypeScriptOnSaveFileAction.cs
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
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TypeScriptBinding.Hosting;

namespace ICSharpCode.TypeScriptBinding
{
	public class CompileTypeScriptOnSaveFileAction : CompileTypeScriptAction
	{
		public void Compile(FileName fileName, TypeScriptProject project, TypeScriptContext context)
		{
			ReportCompileStarting(fileName);
			
			project.CreateOutputDirectory();
			
			var compiler = new LanguageServiceCompiler(context);
			UpdateFile(context, fileName);
			LanguageServiceCompilerResult result = compiler.Compile(fileName, project);
			
			if (result.HasErrors) {
				Report(result.GetError());
			}
			ReportCompileFinished(result.HasErrors);
		}
		
		void ReportCompileStarting(FileName fileName)
		{
			ClearOutputWindow();
			Report("Compiling TypeScript file: {0}", fileName.GetFileNameWithoutPath());
		}
	}
}
