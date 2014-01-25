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
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace FSharpBinding
{
	public class FSharpInteractive : AbstractConsolePad
	{
		Queue<string> outputQueue = new Queue<string>();
		internal readonly Process fsiProcess = new Process();
		internal readonly bool foundCompiler;
		
		public FSharpInteractive()
		{
			if (Array.Exists(ConfigurationManager.AppSettings.AllKeys, x => x == "alt_fs_bin_path")) {
				string path = Path.Combine(ConfigurationManager.AppSettings["alt_fs_bin_path"], "fsi.exe");
				if (File.Exists(path)) {
					fsiProcess.StartInfo.FileName = path;
					foundCompiler = true;
				} else {
					AppendLine("you are trying to use the app setting alt_fs_bin_path, but fsi.exe is not localed in the given directory");
					foundCompiler = false;
				}
			} else {
				string[] paths = Environment.GetEnvironmentVariable("PATH").Split(';');
				string path = Array.Find(paths, x => {
				                         	try {
				                         		return File.Exists(Path.Combine(x, "fsi.exe"));
				                         	} catch {
				                         		return false;
				                         	}});
				if (path != null) {
					fsiProcess.StartInfo.FileName = Path.Combine(path, "fsi.exe");
					foundCompiler = true;
				} else {
					path = FindFSharpInteractiveInProgramFilesFolder();
					if (path != null) {
						fsiProcess.StartInfo.FileName = path;
						foundCompiler = true;
					} else {
						AppendLine("Can not find the fsi.exe, ensure a version of the F# compiler is installed." + Environment.NewLine +
						           "Please see http://research.microsoft.com/fsharp for details of how to install the compiler");
						foundCompiler = false;
					}
				}
			}
			
			if (foundCompiler) {
				//fsiProcess.StartInfo.Arguments <- "--fsi-server sharpdevelopfsi";
				fsiProcess.StartInfo.UseShellExecute = false;
				fsiProcess.StartInfo.CreateNoWindow = true;
				fsiProcess.StartInfo.RedirectStandardError = true;
				fsiProcess.StartInfo.RedirectStandardInput = true;
				fsiProcess.StartInfo.RedirectStandardOutput = true;
				fsiProcess.EnableRaisingEvents = true;
				fsiProcess.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e) {
					lock (outputQueue) {
						outputQueue.Enqueue(e.Data);
					}
					SD.MainThread.InvokeAsyncAndForget(ReadAll);
				};
				fsiProcess.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e) {
					lock (outputQueue) {
						outputQueue.Enqueue(e.Data);
					}
					SD.MainThread.InvokeAsyncAndForget(ReadAll);
				};
				fsiProcess.Exited += delegate(object sender, EventArgs e) {
					lock (outputQueue) {
						outputQueue.Enqueue("fsi.exe died");
						outputQueue.Enqueue("restarting ...");
					}
					SD.MainThread.InvokeAsyncAndForget(ReadAll);
					SD.MainThread.InvokeAsyncAndForget(StartFSharp);
				};
				StartFSharp();
			}
		}
		
		string FindFSharpInteractiveInProgramFilesFolder()
		{
			var fileNames = new string [] {
				@"Microsoft SDKs\F#\3.0\Framework\v4.0\Fsi.exe",
				@"Microsoft F#\v4.0\Fsi.exe"
			};
			return FindFirstMatchingFileInProgramFiles(fileNames);
		}
		
		string FindFirstMatchingFileInProgramFiles(string[] fileNames)
		{
			string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			return fileNames.Select(fileName => Path.Combine(programFiles, fileName))
				.FirstOrDefault(fullPath => File.Exists(fullPath));
		}
		
		void StartFSharp()
		{
			fsiProcess.Start();
			fsiProcess.BeginErrorReadLine();
			fsiProcess.BeginOutputReadLine();
		}
		
		int expectedPrompts;
		
		void ReadAll()
		{
			StringBuilder b = new StringBuilder();
			lock (outputQueue) {
				while (outputQueue.Count > 0)
					b.AppendLine(outputQueue.Dequeue());
			}
			int offset = 0;
			// ignore prompts inserted by fsi.exe (we only see them too late as we're reading line per line)
			for (int i = 0; i < expectedPrompts; i++) {
				if (offset + 1 < b.Length && b[offset] == '>' && b[offset + 1] == ' ')
					offset += 2;
				else
					break;
			}
			expectedPrompts = 0;
			InsertBeforePrompt(b.ToString(offset, b.Length - offset));
		}
		
		protected override string Prompt {
			get { return "> "; }
		}
		
		protected override bool AcceptCommand(string command)
		{
			if (command.TrimEnd().EndsWith(";;", StringComparison.Ordinal)) {
				expectedPrompts++;
				fsiProcess.StandardInput.WriteLine(command);
				return true;
			}
			return false;
		}
	}
	
	public class SentToFSharpInteractive : AbstractMenuCommand
	{
		public override void Run()
		{
			PadDescriptor pad = SD.Workbench.GetPad(typeof(FSharpInteractive));
			pad.BringPadToFront();
			FSharpInteractive fsharpInteractive = (FSharpInteractive)pad.PadContent;
			if (fsharpInteractive.foundCompiler) {
				ITextEditor textEditor = SD.GetActiveViewContentService<ITextEditor>();
				if (textEditor != null) {
					if (textEditor.SelectionLength > 0) {
						fsharpInteractive.fsiProcess.StandardInput.WriteLine(textEditor.SelectedText);
					} else {
						var line = textEditor.Document.GetLineByNumber(textEditor.Caret.Line);
						fsharpInteractive.fsiProcess.StandardInput.WriteLine(textEditor.Document.GetText(line));
					}
					fsharpInteractive.fsiProcess.StandardInput.WriteLine(";;");
				}
			}
		}
	}
}
