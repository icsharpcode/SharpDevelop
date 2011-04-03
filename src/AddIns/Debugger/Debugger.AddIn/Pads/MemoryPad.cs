// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

using Debugger;
using Debugger.Interop;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public sealed class MemoryPad : DebuggerPad
	{
		Dictionary<long, int> addressesMapping = new Dictionary<long, int>();
		ConsoleControl console;
		int addressStep = 16;
		
		Process debuggedProcess;
		
		public MemoryPad()
		{
			this.console = new ConsoleControl();
			this.panel.Children.Add(console);
			this.console.Encoding = Encoding.Default;
			RefreshPad();
			this.console.SetReadonly();
		}
		
		protected override ToolBar BuildToolBar()
		{
			return ToolBarService.CreateToolBar(panel, this, "/SharpDevelop/Pads/MemoryPad/ToolBar");
		}
		
		protected override void SelectProcess(Process process)
		{
			if (debuggedProcess != null) {
				debuggedProcess.Paused -= OnProcessPaused;
			}
			debuggedProcess = process;
			if (debuggedProcess != null) {
				debuggedProcess.Paused += OnProcessPaused;
			}
		}
		
		public override void RefreshPad()
		{
			Refresh();
			base.RefreshPad();
		}
		
		public void JumpToAddress(string address)
		{
			try {
				if (address.StartsWith("0x"))
					address = address.Substring(2);
				
				long addr = Int64.Parse(address, NumberStyles.AllowHexSpecifier);
				long mod = addr % addressStep;
				
				int line;
				if (addressesMapping.ContainsKey(addr - mod))
					line = addressesMapping[addr - mod];
				else
					line = 1;
				
				console.SelectText(line, 0, 8);
				console.JumpToLine(line);
			} catch (System.Exception ex) {
				#if DEBUG
				LoggingService.Error(ex.Message);
				#endif
			}
		}
		
		public void Refresh(bool force = false)
		{
			if (debuggedProcess == null || debugger.IsProcessRunning)
				return;
			
			if (!force && addressesMapping.Count > 0)
				return;
			
			if (force) {
				addressesMapping.Clear();
				console.Clear();
			}
			
			long address;
			byte[] memory = debuggedProcess.ReadProcessMemory(out address);
			
			if (memory == null)
				return;
			
			int index = 0;
			int div = memory.Length / addressStep;
			int mod = memory.Length % addressStep;
			
			while (index < div) {
				StringBuilder sb = new StringBuilder();
				addressesMapping.Add(address, index + 1);
				// write address
				sb.Append(address.ToString("X8"));address += addressStep;
				sb.Append(" ");
				
				// write bytes
				for (int i = 0; i < addressStep; ++i) {
					sb.Append(memory[index * addressStep + i].ToString("X2") + " ");
				}
				// write chars
				StringBuilder sb1 = new StringBuilder();
				for (int i = 0; i < addressStep; ++i) {
					sb1.Append(((char)memory[index * addressStep + i]).ToString());
				}
				string s = sb1.ToString();
				s = Regex.Replace(s, @"\r\n", string.Empty);
				s = Regex.Replace(s, @"\n", string.Empty);
				s = Regex.Replace(s, @"\r", string.Empty);					
				sb.Append(s);
				sb.Append(Environment.NewLine);
				
				// start writing in console
				console.Append(sb.ToString());
				
				index++;
			}
			
			if (mod != 0) {
				// write the rest of memory
				StringBuilder sb = new StringBuilder();
				addressesMapping.Add(address, index + 1);
				// write address
				sb.Append(address.ToString("X8"));
				sb.Append(" ");
				
				// write bytes
				for (int i = 0; i < mod; ++i) {
					sb.Append(memory[index * addressStep + i].ToString("X2") + " ");
				}
				// write chars
				StringBuilder sb1 = new StringBuilder();
				for (int i = 0; i < mod; ++i) {
					sb1.Append(((char)memory[index * addressStep + i]).ToString());
				}
				string s = sb1.ToString();
				s = Regex.Replace(s, @"\r\n", string.Empty);
				s = Regex.Replace(s, @"\n", string.Empty);
				s = Regex.Replace(s, @"\r", string.Empty);					
				sb.Append(s);
				
				sb.Append(Environment.NewLine);
				
				// start writing in console
				console.Append(sb.ToString());
			}
		}
		
		private void OnProcessPaused(object sender, ProcessEventArgs e)
		{
			Refresh();
		}
	}
}
