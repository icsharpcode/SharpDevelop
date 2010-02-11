// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace FSharpBinding
{
	public class FSharpInteractive : AbstractPadContent
	{
		Queue<string> outputQueue = new Queue<string>();
		internal readonly Process fsiProcess = new Process();
		Panel panel = new Panel();
		TextBox input, output;
		internal readonly bool foundCompiler;
		
		public FSharpInteractive()
		{
			input = new TextBox {
				Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,
				Width = panel.Width
			};
			output = new TextBox {
				Multiline = true,
				Top = input.Height,
				Height = panel.Height - input.Height,
				Width = panel.Width,
				ReadOnly = true,
				ScrollBars = ScrollBars.Both,
				WordWrap = false,
				Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom
			};
			panel.Controls.Add(input);
			panel.Controls.Add(output);
			
			if (Array.Exists(ConfigurationManager.AppSettings.AllKeys, x => x == "alt_fs_bin_path")) {
				string path = Path.Combine(ConfigurationManager.AppSettings["alt_fs_bin_path"], "fsi.exe");
				if (File.Exists(path)) {
					fsiProcess.StartInfo.FileName = path;
					foundCompiler = true;
				} else {
					output.Text = "you are trying to use the app setting alt_fs_bin_path, but fsi.exe is not localed in the given directory";
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
					var possibleFiles = from fsdir in Directory.GetDirectories(programFiles, "FSharp*")
						//LoggingService.Debug("Trying to find fsi in '" + fsdir + "'");
						let fileInfo = new FileInfo(Path.Combine(fsdir, "bin\\fsi.exe"))
						where fileInfo.Exists
						orderby fileInfo.CreationTime
						select fileInfo;
					FileInfo file = possibleFiles.FirstOrDefault();
					if (file != null) {
						fsiProcess.StartInfo.FileName = file.FullName;
						foundCompiler = true;
					} else {
						output.Text = "Can not find the fsi.exe, ensure a version of the F# compiler is installed." + Environment.NewLine +
							"Please see http://research.microsoft.com/fsharp for details of how to install the compiler";
						foundCompiler = false;
					}
				}
			}
			
			if (foundCompiler) {
				input.KeyUp += delegate(object sender, KeyEventArgs e) {
					if (e.KeyData == Keys.Return) {
						fsiProcess.StandardInput.WriteLine(input.Text);
						input.Text = "";
					}
				};
				//fsiProcess.StartInfo.Arguments <- "--fsi-server sharpdevelopfsi";
				fsiProcess.StartInfo.UseShellExecute = false;
				fsiProcess.StartInfo.CreateNoWindow = true;
				fsiProcess.StartInfo.RedirectStandardError = true;
				fsiProcess.StartInfo.RedirectStandardInput = true;
				fsiProcess.StartInfo.RedirectStandardOutput = true;
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
					fsiProcess.Start();
				};
				fsiProcess.Start();
				fsiProcess.BeginErrorReadLine();
				fsiProcess.BeginOutputReadLine();
			} else {
				input.KeyUp += delegate(object sender, KeyEventArgs e) {
					if (e.KeyData == Keys.Return) {
						output.AppendText(Environment.NewLine + "F# not installed - could not execute command");
						input.Text = "";
					}
				};
			}
		}
		
		void ReadAll()
		{
			lock (outputQueue) {
				while (outputQueue.Count > 0)
					output.AppendText(outputQueue.Dequeue() + Environment.NewLine);
			}
		}
		
		public override Control Control {
			get { return panel; }
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
				ITextEditorControlProvider editorProvider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorControlProvider;
				if (editorProvider != null) {
					var textArea = editorProvider.TextEditorControl.ActiveTextAreaControl.TextArea;
					if (textArea.SelectionManager.HasSomethingSelected) {
						foreach (var selection in textArea.SelectionManager.SelectionCollection) {
							fsharpInteractive.fsiProcess.StandardInput.WriteLine(selection.SelectedText);
						}
					} else {
						var line = textArea.Document.GetLineNumberForOffset(textArea.Caret.Offset);
						var lineSegment = textArea.Document.GetLineSegment(line);
						var lineText = textArea.Document.GetText(lineSegment.Offset, lineSegment.TotalLength);
						fsharpInteractive.fsiProcess.StandardInput.WriteLine(lineText);
					}
					fsharpInteractive.fsiProcess.StandardInput.WriteLine(";;");
				}
			}
		}
	}
}
