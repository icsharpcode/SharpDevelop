// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			
			ulong addrHello = process.SelectedStackFrame.GetLocalVariableValue("hello").Address;
			ulong addrWorld = process.SelectedStackFrame.GetLocalVariableValue("world").Address;
			
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
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>Process_MemoryReadWrite.exe (Has symbols)</ModuleLoaded>
    <ModuleLoaded>System.dll (No symbols)</ModuleLoaded>
    <DebuggingPaused>Break Process_MemoryReadWrite.cs:14,4-14,40</DebuggingPaused>
    <hello>5 0 0 0 48 0 65 0 6C 0 6C 0 6F 0 </hello>
    <world>6 0 0 0 20 0 20 0 20 0 20 0 20 0 21 0 </world>
    <ModuleLoaded>System.Configuration.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>System.Xml.dll (No symbols)</ModuleLoaded>
    <LogMessage>Hello world!\r\n</LogMessage>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
