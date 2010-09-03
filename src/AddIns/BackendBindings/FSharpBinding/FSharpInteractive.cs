// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
					string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
					path = Path.Combine(programFiles, @"Microsoft F#\v4.0\Fsi.exe");
					if (File.Exists(path)) {
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
					WorkbenchSingleton.SafeThreadAsyncCall(ReadAll);
				};
				fsiProcess.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e) {
					lock (outputQueue) {
						outputQueue.Enqueue(e.Data);
					}
					WorkbenchSingleton.SafeThreadAsyncCall(ReadAll);
				};
				fsiProcess.Exited += delegate(object sender, EventArgs e) {
					lock (outputQueue) {
						outputQueue.Enqueue("fsi.exe died");
						outputQueue.Enqueue("restarting ...");
					}
					WorkbenchSingleton.SafeThreadAsyncCall(ReadAll);
					WorkbenchSingleton.SafeThreadAsyncCall(StartFSharp);
				};
				StartFSharp();
			}
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
			PadDescriptor pad = WorkbenchSingleton.Workbench.GetPad(typeof(FSharpInteractive));
			pad.BringPadToFront();
			FSharpInteractive fsharpInteractive = (FSharpInteractive)pad.PadContent;
			if (fsharpInteractive.foundCompiler) {
				ITextEditorProvider editorProvider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
				if (editorProvider != null) {
					var textEditor = editorProvider.TextEditor;
					if (textEditor.SelectionLength > 0) {
						fsharpInteractive.fsiProcess.StandardInput.WriteLine(textEditor.SelectedText);
					} else {
						var line = textEditor.Document.GetLine(textEditor.Caret.Line);
						fsharpInteractive.fsiProcess.StandardInput.WriteLine(line.Text);
					}
					fsharpInteractive.fsiProcess.StandardInput.WriteLine(";;");
				}
			}
		}
	}
}
