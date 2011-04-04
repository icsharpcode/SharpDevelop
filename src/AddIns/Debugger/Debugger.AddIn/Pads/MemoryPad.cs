// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
		Dictionary<string, int> addressesMapping = new Dictionary<string, int>();
		
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
				
				memoryAddresses = debuggedProcess.GetMemoryAddresses();
				// find index for the address or the near addess
				currentAddressIndex = memoryAddresses.BinarySearch(addr);
				if (currentAddressIndex == -1) {
					MessageService.ShowMessage(
						string.Format(ResourceService.GetString("MainWindow.Windows.Debug.MemoryPad.AddressNotFound"), address),
						ResourceService.GetString("MainWindow.Windows.Debug.MemoryPad"));
					
					currentAddressIndex = 0;
					return;
				}
				
				// refresh pad
				if (!Refresh())
					return;
				
				// find line
				long mod = addr % addressStep;
				int line;
				string key = (addr - mod).ToString("X8");
				if (addressesMapping.ContainsKey(key))
					line = addressesMapping[key];
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
		
		public bool Refresh(bool refreshMemoryAddresses = false)
		{
			console.Clear();
			
			if (debuggedProcess == null || debugger.IsProcessRunning) {
				console.Append("Not debugging or process is running!");
				return false;
			}
			
			if (currentAddressIndex <= -1) {
				console.Append("No mappings for memory addresses!");
				currentAddressIndex = -1;
				return false;
			}
			
			if (refreshMemoryAddresses)
				memoryAddresses = debuggedProcess.GetMemoryAddresses();
			
			if (memoryAddresses.Count == 0) {
				console.Append("No mappings for memory addresses!");
				return false;
			}
			
			if (currentAddressIndex >= memoryAddresses.Count) {
				console.Append("No mappings for memory addresses!");
				currentAddressIndex = memoryAddresses.Count ;
				return false;
			}

			console.Append(DoWork());
			return true;
		}
		
		string DoWork()
		{
			// refresh data
			addressesMapping.Clear();
			
			// get current address
			var item = memoryAddresses[currentAddressIndex];
			long address = item.Item1;
			long size = item.Item2;
			
			byte[] memory = debuggedProcess.ReadProcessMemory(address, size);
			System.Diagnostics.Debug.Assert(memory != null);
			
			int numberOfLines = memory.Length / addressStep;
			int mod = memory.Length % addressStep;
			int currentLine = 0;
			StringBuilder sb = new StringBuilder();
			
			while (currentLine < numberOfLines) {
				addressesMapping.Add(address.ToString("X8"), currentLine + 1);
				// write address
				sb.Append(address.ToString("X8")); address += (long)addressStep;
				sb.Append(" ");
				
				// write bytes
				for (int i = 0; i < addressStep; ++i) {
					sb.Append(memory[currentLine * addressStep + i].ToString("X2") + " ");
				}
				// write chars
				StringBuilder sb1 = new StringBuilder();
				for (int i = 0; i < addressStep; ++i) {
					sb1.Append(((char)memory[currentLine * addressStep + i]).ToString());
				}
				string s = sb1.ToString();
				s = Regex.Replace(s, @"\r\n", string.Empty);
				s = Regex.Replace(s, @"\n", string.Empty);
				s = Regex.Replace(s, @"\r", string.Empty);
				sb.Append(s);
				sb.Append(Environment.NewLine);
				
				currentLine++;
			}
			
			if (mod != 0) {
				// write the rest of memory
				addressesMapping.Add(address.ToString("X8"), currentLine + 1);
				// write address
				sb.Append(address.ToString("X8"));
				sb.Append(" ");
				
				// write bytes
				for (int i = 0; i < mod; ++i) {
					sb.Append(memory[currentLine * addressStep + i].ToString("X2") + " ");
				}
				// write chars
				StringBuilder sb1 = new StringBuilder();
				for (int i = 0; i < mod; ++i) {
					sb1.Append(((char)memory[currentLine * addressStep + i]).ToString());
				}
				string s = sb1.ToString();
				s = Regex.Replace(s, @"\r\n", string.Empty);
				s = Regex.Replace(s, @"\n", string.Empty);
				s = Regex.Replace(s, @"\r", string.Empty);
				sb.Append(s);
			}
			
			return sb.ToString();
		}
		
		public void MoveToPreviousAddress()
		{
			currentAddressIndex--;
			Refresh();
		}
		
		public void MoveToNextAddress()
		{
			currentAddressIndex++;
			Refresh();
		}
	}
	
	internal static class MemoryPadExtensions
	{
		/// <summary>
		/// Does a binary search when the Item1 from Tuple is sorted.
		/// </summary>
		/// <param name="source">Source of data.</param>
		/// <param name="item1">Item to search.</param>
		/// <returns>The nearast index.</returns>
		internal static int BinarySearch(this List<Tuple<long, long>> source, long item1)
		{
			// base checks
			if (source == null)
				throw new NullReferenceException("Source is null!");
			
			if (source.Count == 0)
				return -1;
			
			if (item1 < source[0].Item1)
				return 0;
			
			if (item1 > source[source.Count - 1].Item1)
				return source.Count;
			
			// do a binary search since the source is sorted
			int first = 0; int last = source.Count;
			while (first < last - 1) {
				int middle = (first + last) / 2;
				if (source[middle].Item1 == item1)
					return middle;
				else
					if (source[middle].Item1 < item1)
						first = middle;
					else
						last = middle;
			}
			
			return first;
		}
	}
}
