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

namespace Debugger.Tests
{
	public class Process_MemoryReadWrite
	{
		public static void Main()
		{
			string hello = "Hello";
			string world = "     !";
			System.Diagnostics.Debugger.Break();
			System.Diagnostics.Debug.WriteLine(hello + " " + world);
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void Process_MemoryReadWrite()
		{
			StartTest();
			
			ulong addrHello = this.CurrentStackFrame.GetLocalVariableValue("hello").Address;
			ulong addrWorld = this.CurrentStackFrame.GetLocalVariableValue("world").Address;
			
			addrHello = DeRef(process.ReadMemory(addrHello, 4));
			addrWorld = DeRef(process.ReadMemory(addrWorld, 4));
			
			byte[] hello = process.ReadMemory(addrHello + 4, 14);
			byte[] world = process.ReadMemory(addrWorld + 4, 16);
			
			ObjectDump("hello", ToHex(hello));
			ObjectDump("world", ToHex(world));
			
			process.WriteMemory(addrWorld + 8, new byte[] {0x77, 0x0, 0x6F, 0x0, 0x72, 0x0, 0x6C, 0x0, 0x64, 0x0});
			
			EndTest();
		}
		
		ulong DeRef(byte[] ptr)
		{
			return (ulong)(ptr[0] + (ptr[1] << 8) + (ptr[2] << 16) + (ptr[3] << 24));
		}
		
		string ToHex(byte[] buffer)
		{
			string hex = "";
			foreach(byte b in buffer) {
				hex += b.ToString("X") + " ";
			}
			return hex;
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="Process_MemoryReadWrite.cs">
    <Started />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>Process_MemoryReadWrite.exe (Has symbols)</ModuleLoaded>
    <ModuleLoaded>System.dll (No symbols)</ModuleLoaded>
    <Paused>Process_MemoryReadWrite.cs:29,4-29,40</Paused>
    <hello>5 0 0 0 48 0 65 0 6C 0 6C 0 6F 0 </hello>
    <world>6 0 0 0 20 0 20 0 20 0 20 0 20 0 21 0 </world>
    <ModuleLoaded>System.Configuration.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>System.Core.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>System.Xml.dll (No symbols)</ModuleLoaded>
    <LogMessage>Hello world!\r\n</LogMessage>
    <Exited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
