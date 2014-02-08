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
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.UnitTesting
{
	public class UnitTestDebuggerService : BaseDebuggerService
	{
		public override bool CanDebug(ICSharpCode.SharpDevelop.Project.IProject project)
		{
			return SD.Debugger.CanDebug(project);
		}
		public override bool Supports(DebuggerFeatures feature)
		{
			return SD.Debugger.Supports(feature);
		}
		public override void Start(System.Diagnostics.ProcessStartInfo processStartInfo)
		{
			SD.Debugger.Start(processStartInfo);
		}
		public override void StartWithoutDebugging(System.Diagnostics.ProcessStartInfo processStartInfo)
		{
			SD.Debugger.StartWithoutDebugging(processStartInfo);
		}
		public override void Stop()
		{
			SD.Debugger.Stop();
		}
		public override void Break()
		{
			SD.Debugger.Break();
		}
		public override void Continue()
		{
			SD.Debugger.Continue();
		}
		public override void StepInto()
		{
			SD.Debugger.StepInto();
		}
		public override void StepOver()
		{
			SD.Debugger.StepOver();
		}
		public override void StepOut()
		{
			SD.Debugger.StepOut();
		}
		public override void ShowAttachDialog()
		{
			SD.Debugger.ShowAttachDialog();
		}
		public override void Attach(System.Diagnostics.Process process)
		{
			SD.Debugger.Attach(process);
		}
		public override void Detach()
		{
			SD.Debugger.Detach();
		}
		public override bool SetInstructionPointer(string filename, int line, int column, bool dryRun)
		{
			return SD.Debugger.SetInstructionPointer(filename, line, column, dryRun);
		}
		public override void HandleToolTipRequest(ICSharpCode.SharpDevelop.Editor.ToolTipRequestEventArgs e)
		{
			SD.Debugger.HandleToolTipRequest(e);
		}
		public override void ToggleBreakpointAt(ICSharpCode.SharpDevelop.Editor.ITextEditor editor, int lineNumber)
		{
			SD.Debugger.ToggleBreakpointAt(editor, lineNumber);
		}
		public override void RemoveCurrentLineMarker()
		{
			SD.Debugger.RemoveCurrentLineMarker();
		}
		public override bool IsDebugging {
			get { return SD.Debugger.IsDebugging; }
		}
		public override bool IsProcessRunning {
			get {
				return SD.Debugger.IsProcessRunning;
			}
		}
		public override bool BreakAtBeginning {
			get {
				return SD.Debugger.BreakAtBeginning;
			}
			set {
				SD.Debugger.BreakAtBeginning = value;
			}
		}
		public override bool IsAttached {
			get {
				return SD.Debugger.IsAttached;
			}
		}
	}
}
