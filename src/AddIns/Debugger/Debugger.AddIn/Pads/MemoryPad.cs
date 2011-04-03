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
		int currentAddressIndex;
		ConsoleControl console;
		int addressStep = 16;
		
		Process debuggedProcess;
		List<Tuple<long, long>> memoryAddresses = new List<Tuple<long, long>>();
		Dictionary<long, int> addressesMapping = new Dictionary<long, int>();
		
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
			if (process == null)
				return;
			
			debuggedProcess = process;
			memoryAddresses = debuggedProcess.GetMemoryAddresses();
			currentAddressIndex = 0;
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
				
				// find index for the address or the near addess
				currentAddressIndex = memoryAddresses.Search(addr);
				if (currentAddressIndex == -1) {
					MessageService.ShowMessage(
						string.Format(ResourceService.GetString("MainWindow.Windows.Debug.MemoryPad.AddressNotFound"), address), 
						ResourceService.GetString("MainWindow.Windows.Debug.MemoryPad"));
					
					currentAddressIndex = 0;
					return;
				}
				
				// refresh pad
				Refresh();
				
				// find line
				long mod = addr % addressStep;
				int line;
				if (addressesMapping.ContainsKey(addr - mod))
					line = addressesMapping[addr - mod];
				else
					line = 1;

				// jump
				console.SelectText(line, 0, 8);
				console.JumpToLine(line);
				
			} catch (System.Exception ex) {
				#if DEBUG
				LoggingService.Error(ex.Message);
				#endif
			}
		}
		
		public void Refresh()
		{
			if (debuggedProcess == null || debugger.IsProcessRunning)
				return;
			
			if (memoryAddresses.Count == 0)
				return;
			
			console.Clear();addressesMapping.Clear();
			
			// get current address
			var item = memoryAddresses[currentAddressIndex];
			long address = item.Item1;
			long size = item.Item2;
			
			byte[] memory = debuggedProcess.ReadProcessMemory(address, size);
			System.Diagnostics.Debug.Assert(memory != null);
			
			int div = memory.Length / addressStep;
			int mod = memory.Length % addressStep;
			int index = 0;
			
			while (index < div) {
				StringBuilder sb = new StringBuilder();
				addressesMapping.Add(address, index + 1);
				// write address
				sb.Append(address.ToString("X8")); address += (long)addressStep;
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
		
		public void MoveToPreviousAddress()
		{
			if (debuggedProcess == null || debugger.IsProcessRunning)
				return;
			
			if (currentAddressIndex == 0)
				return;
			
			currentAddressIndex--;
			Refresh();
		}
		
		public void MoveToNextAddress()
		{
			if (debuggedProcess == null || debugger.IsProcessRunning)
				return;
			
			if (currentAddressIndex == memoryAddresses.Count)
				return;
			
			currentAddressIndex++;
			Refresh();
		}
	}
	
	internal static class MemoryPadExtensions
	{
		internal static int Search(this List<Tuple<long, long>> source, long item1)
		{
			if (source == null)
				throw new NullReferenceException("Source is null!");
			
			for (int i = 0; i < source.Count - 1; i++) {
				if (source[i + 1].Item1 < item1)
					continue;
				
				return i;
			}
			
			return -1;
		}
	}
}
