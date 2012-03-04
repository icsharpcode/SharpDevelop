// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
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
		int columnsNumber = 16;
		byte displayByteSize = 1;
		byte[] memory;
		Process debuggedProcess;
		List<Tuple<long, long>> memoryAddresses = new List<Tuple<long, long>>();
		Dictionary<long, int> addressesMapping = new Dictionary<long, int>();
		
		/// <summary>
		/// Gets or sets the number of columns in the display
		/// </summary>
		[DefaultValue(16)]
		public int ColumnsNumber {
			get { return columnsNumber; }
			set {
				if (value != columnsNumber) {
					columnsNumber = value;
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the display byte size: 1, 2, 4
		/// </summary>
		[DefaultValue(1)]
		public byte DisplayByteSize {
			get { return displayByteSize; }
			set {
				// check is value is a power of 2 between 1 and 4.
				if ((value & (value - 1)) != 0)
					return;
				if (value < 1 || value > 4)
					return;
				
				if (displayByteSize != value) {
					displayByteSize = value;
				}
			}
		}
		
		public MemoryPad()
		{
			this.console = new ConsoleControl();
			this.panel.Children.Add(console);
			this.console.Encoding = Encoding.Default;
			RefreshPad(); // exception
			this.console.SetReadonly();
			
			DebuggerService.DebugStopped += DebuggerService_DebugStopped;
		}

		void DebuggerService_DebugStopped(object sender, EventArgs e)
		{
			memoryAddresses.Clear();
			addressesMapping.Clear();
			memory = null;
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
			memoryAddresses = debuggedProcess.GetVirtualMemoryAddresses();
			currentAddressIndex = 0;
		}
		
		public void JumpToAddress(string address)
		{
			try {
				if (address.StartsWith("0x"))
					address = address.Substring(2);
				
				long addr = Int64.Parse(address, NumberStyles.AllowHexSpecifier);
				
				memoryAddresses = debuggedProcess.GetVirtualMemoryAddresses();
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
				long mod = addr % (columnsNumber * displayByteSize);
				int line;
				long key = addr - mod;
				//int index = addressesMapping.BinarySearch(key);
				if (addressesMapping.ContainsKey(key))
					line = addressesMapping[key];
				else
					line = 1;

				// jump
				console.SelectText(line, 0, 8);
				console.JumpToLine(line);
				
			} catch (System.Exception ex) {
				LoggingService.Error(ex.Message);
			}
		}
		
		public bool Refresh(bool refreshMemoryAddresses = false)
		{
			if (console == null)
				return false;
			
			console.Clear();
			if (debuggedProcess == null || debugger.IsProcessRunning) {
				console.Append(ResourceService.GetString("MainWindow.Windows.Debug.MemoryPad.NotDebuggingOrProcessRunning"));
				return false;
			}
			
			if (currentAddressIndex <= -1) {
				console.Append(ResourceService.GetString("MainWindow.Windows.Debug.MemoryPad.NoMappings"));
				currentAddressIndex = -1;
				return false;
			}
			
			if (refreshMemoryAddresses)
				memoryAddresses = debuggedProcess.GetVirtualMemoryAddresses();
			
			if (memoryAddresses.Count == 0) {
				console.Append(ResourceService.GetString("MainWindow.Windows.Debug.MemoryPad.NoMappings"));
				return false;
			}
			
			if (currentAddressIndex >= memoryAddresses.Count) {
				console.Append(ResourceService.GetString("MainWindow.Windows.Debug.MemoryPad.NoMappings"));
				currentAddressIndex = memoryAddresses.Count ;
				return false;
			}

			RetrieveMemory();
			return true;
		}
		
		void RetrieveMemory()
		{
			// refresh data
			addressesMapping.Clear();
			
			// get current address
			var item = memoryAddresses[currentAddressIndex];
			long address = item.Item1;
			long size = item.Item2;
			
			memory = debuggedProcess.ReadProcessMemory(address, size);
			if (memory == null) {
				console.Append(string.Format(ResourceService.GetString("MainWindow.Windows.Debug.MemoryPad.UnableToReadFormat"), address.ToString("X8"), size));
				return;
			}
			
			DisplayMemory();
		}

		public void DisplayMemory()
		{
			if (memory == null || memory.Length == 0)
				return;
			
			if (console == null)
				return;
			
			console.Clear();
			addressesMapping.Clear();
			var item = memoryAddresses[currentAddressIndex];
			long address = item.Item1;
			
			int totalBytesPerRow = columnsNumber * displayByteSize;
			int numberOfLines = memory.Length / totalBytesPerRow;
			int remainingMemory = memory.Length % totalBytesPerRow;
			int currentLine = 2;// line in the console
			int index = 0;// index in memory arrray of current line
			
			StringBuilder sb = new StringBuilder();
			sb.Append(string.Format(ResourceService.GetString("MainWindow.Windows.Debug.MemoryPad.ReadingFromFormat"), address.ToString("X8"), (address + memory.Length).ToString("X8"), memory.Length));
			sb.Append(Environment.NewLine);
			
			while (index < numberOfLines) {
				addressesMapping.Add(address, currentLine);
				// write address
				sb.Append(address.ToString("X8"));
				address += (long)totalBytesPerRow;
				sb.Append(" ");
				
				// write bytes
				int start = index * totalBytesPerRow;
				for (int i = 0; i < columnsNumber; ++i) {
					for (int j = 0; j < displayByteSize; ++j) {
						sb.Append(memory[start++].ToString("X2"));
					}
					sb.Append(" ");
				}
				
				// write chars
				start = index * totalBytesPerRow;
				StringBuilder sb1 = new StringBuilder();
				for (int i = 0; i < totalBytesPerRow; ++i) {
					sb1.Append(((char)memory[start++]).ToString());
				}
				string s = sb1.ToString();
				s = Regex.Replace(s, "\\r\\n", string.Empty);
				s = Regex.Replace(s, "\\n", string.Empty);
				s = Regex.Replace(s, "\\r", string.Empty);
				sb.Append(s);
				sb.Append(Environment.NewLine);
				currentLine++;
				index++;
			}
			
			// write the rest of memory
			if (remainingMemory != 0) {
				addressesMapping.Add(address, currentLine);
				// write address
				sb.Append(address.ToString("X8"));
				sb.Append(" ");
				
				// write bytes
				int start = index * remainingMemory * displayByteSize;
				for (int i = 0; i < remainingMemory; ++i) {
					for (int j = 0; j < displayByteSize; j++) {
						sb.Append(memory[start++].ToString("X2"));
					}
					sb.Append(" ");
				}
				
				// write chars
				start = index * remainingMemory * displayByteSize;
				StringBuilder sb1 = new StringBuilder();
				for (int i = 0; i < remainingMemory * displayByteSize; ++i) {
					sb1.Append(((char)memory[start++]).ToString());
				}
				string s = sb1.ToString();
				s = Regex.Replace(s, "\\r\\n", string.Empty);
				s = Regex.Replace(s, "\\n", string.Empty);
				s = Regex.Replace(s, "\\r", string.Empty);
				sb.Append(s);
			}
			
			console.Append(sb.ToString());
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
		/// <returns>The nearest index.</returns>
		internal static int BinarySearch<T>(this List<Tuple<long, T>> source, long item1)
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
