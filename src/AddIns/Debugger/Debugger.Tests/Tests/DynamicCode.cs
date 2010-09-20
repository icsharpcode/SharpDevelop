// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace Debugger.Tests
{
	public class DynamicCode
	{
		public static void Main()
		{			
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.Name = "DynamicllyGeneratedAssembly";
			AssemblyBuilder assemblyBuilder = System.Threading.Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
			
			ConstructorInfo daCtor = typeof(DebuggableAttribute).GetConstructor(new Type[] { typeof(DebuggableAttribute.DebuggingModes) });
			CustomAttributeBuilder daBuilder = new CustomAttributeBuilder(daCtor, new object[] { DebuggableAttribute.DebuggingModes.DisableOptimizations | DebuggableAttribute.DebuggingModes.Default });
			assemblyBuilder.SetCustomAttribute(daBuilder);
			
			ModuleBuilder module = assemblyBuilder.DefineDynamicModule("DynamicllyGeneratedModule.exe", true);
			
			ISymbolDocumentWriter doc = module.DefineDocument(@"Source.txt", Guid.Empty, Guid.Empty, Guid.Empty);
			TypeBuilder typeBuilder = module.DefineType("DynamicllyGeneratedType", TypeAttributes.Public | TypeAttributes.Class);
			MethodBuilder methodbuilder = typeBuilder.DefineMethod("Main", MethodAttributes.HideBySig | MethodAttributes.Static | MethodAttributes.Public, typeof(void), new Type[] { typeof(string[]) });
			ILGenerator ilGenerator = methodbuilder.GetILGenerator();
			
			ilGenerator.MarkSequencePoint(doc, 1, 1, 1, 100);
			ilGenerator.Emit(OpCodes.Ldstr, "Hello world!");
			MethodInfo infoWriteLine = typeof(System.Console).GetMethod("WriteLine", new Type[] { typeof(string) });
			ilGenerator.EmitCall(OpCodes.Call, infoWriteLine, null);
			
			ilGenerator.MarkSequencePoint(doc, 2, 1, 2, 100);
			ilGenerator.Emit(OpCodes.Ret);
			
			Type helloWorldType = typeBuilder.CreateType();			
			
			System.Diagnostics.Debugger.Break();
			helloWorldType.GetMethod("Main").Invoke(null, new string[] { null });
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	using NUnit.Framework;
	
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void DynamicCode()
		{
			StartTest();
			
			process.SelectedStackFrame.StepOver();
			process.SelectedStackFrame.StepInto();
			Assert.AreEqual("Source.txt", process.SelectedStackFrame.NextStatement.Filename);
						
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="DynamicCode.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>DynamicCode.exe (Has symbols)</ModuleLoaded>
    <ModuleLoaded>DynamicllyGeneratedAssembly (No symbols)</ModuleLoaded>
    <ModuleLoaded>ISymWrapper.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>System.dll (No symbols)</ModuleLoaded>
    <DebuggingPaused>Break DynamicCode.cs:42,4-42,40</DebuggingPaused>
    <DebuggingPaused>StepComplete DynamicCode.cs:43,4-43,73</DebuggingPaused>
    <DebuggingPaused>StepComplete Source.txt:1,1-1,100</DebuggingPaused>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
