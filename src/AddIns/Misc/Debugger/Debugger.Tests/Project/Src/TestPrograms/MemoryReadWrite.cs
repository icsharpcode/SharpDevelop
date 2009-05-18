// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class MemoryReadWrite
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
		[NUnit.Framework.Ignore("Ignored because it fails too frequently - the expected output seems to be different on each machine (or at least for each .NET/Windows version combination)")]
		public void MemoryReadWrite()
		{
			StartTest("MemoryReadWrite.cs");
			
			ulong addrHello = process.SelectedStackFrame.GetLocalVariableValue("hello").Address;
			ulong addrWorld = process.SelectedStackFrame.GetLocalVariableValue("world").Address;
			
			addrHello = DeRef(process.ReadMemory(addrHello, 4));
			addrWorld = DeRef(process.ReadMemory(addrWorld, 4));
			
			byte[] hello = process.ReadMemory(addrHello, 22);
			byte[] world = process.ReadMemory(addrWorld, 24);
			
			ObjectDump("hello", ToHex(hello));
			ObjectDump("world", ToHex(world));
			
			process.WriteMemory(addrWorld + 12, new byte[] {0x77, 0x0, 0x6F, 0x0, 0x72, 0x0, 0x6C, 0x0, 0x64, 0x0});
			
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
    name="MemoryReadWrite.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>MemoryReadWrite.exe (Has symbols)</ModuleLoaded>
    <ModuleLoaded>System.dll (No symbols)</ModuleLoaded>
    <DebuggingPaused>Break MemoryReadWrite.cs:18,4-18,40</DebuggingPaused>
    <hello>0 A 33 79 6 0 0 0 5 0 0 0 48 0 65 0 6C 0 6C 0 6F 0 </hello>
    <world>0 A 33 79 7 0 0 0 6 0 0 0 20 0 20 0 20 0 20 0 20 0 21 0 </world>
    <ModuleLoaded>System.Configuration.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>System.Xml.dll (No symbols)</ModuleLoaded>
    <LogMessage>Hello world!\r\n</LogMessage>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT