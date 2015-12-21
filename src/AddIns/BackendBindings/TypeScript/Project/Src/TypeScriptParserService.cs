// 
// TypeScriptParserService.cs
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.TypeScriptBinding.Hosting;

namespace ICSharpCode.TypeScriptBinding
{
	public class TypeScriptParserService
	{
		DispatcherTimer timer;
		Task parseTask;
		
		public void Start()
		{
			timer = new DispatcherTimer(DispatcherPriority.Background);
			timer.Interval = TimeSpan.FromSeconds(1.5);
			timer.Tick += TimerTick;
			timer.Start();
		}
		
		public void Stop()
		{
			timer.Stop();
		}
		
		void TimerTick(object sender, EventArgs e)
		{
			Run();
		}
		
		void Run()
		{
			try {
				if (!ParserCurrentlyRunning()) {
					ParseActiveTypeScriptFile();
				}
			} catch (Exception ex) {
				LoggingService.DebugFormatted("TypeScriptParser error: {0}", ex.ToString());
			}
		}
		
		bool ParserCurrentlyRunning()
		{
			return (parseTask != null) && !parseTask.IsCompleted;
		}
		
		void ParseActiveTypeScriptFile()
		{
			CodeEditor editor = GetActiveTypeScriptCodeEditor();
			if (editor != null) {
				parseTask = CreateParseTypeScriptFileTask(editor);
			}
		}
		
		CodeEditor GetActiveTypeScriptCodeEditor()
		{
			IViewContent view = SD.Workbench.ActiveViewContent;
			if (view != null) {
				CodeEditor editor = view.GetService<CodeEditor>();
				if (editor != null) {
					if (TypeScriptParser.IsTypeScriptFileName(editor.FileName)) {
						return editor;
					}
				}
			}
			return null;
		}
		
		Task CreateParseTypeScriptFileTask(CodeEditor editor)
		{
			ITextSource fileContent = editor.Document.CreateSnapshot();
			TypeScriptProject project = TypeScriptService.GetProjectForFile(editor.FileName);
			return Task
				.Factory
				.StartNew(() => ParseTypeScriptFile(editor.FileName, fileContent, project))
				.ContinueWith(task => UpdateParseInformation(task.Result), TaskScheduler.FromCurrentSynchronizationContext());
		}
		
		ParseInformation ParseTypeScriptFile(FileName fileName, ITextSource fileContent, TypeScriptProject project)
		{
			var parser = new TypeScriptParser(new LanguageServiceNullLogger());
			List<TypeScriptFile> files = project.GetTypeScriptFiles().ToList();
			return parser.Parse(fileName, fileContent, project, files);
		}
		
		void UpdateParseInformation(ParseInformation parseInfo)
		{
			CodeEditor editor = GetActiveTypeScriptCodeEditor();
			if (editor != null) {
				if (editor.FileName == parseInfo.FileName) {
					editor.ParseInformationUpdated(parseInfo);
				}
			}
		}
	}
}
