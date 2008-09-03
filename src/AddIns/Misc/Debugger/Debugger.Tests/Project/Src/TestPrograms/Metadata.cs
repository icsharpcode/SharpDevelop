// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

// Never used field
#pragma warning disable 0169
// Field will always have default value
#pragma warning disable 0649

using System;

namespace Debugger.Tests.TestPrograms
{
	public class Metadata
	{
		private   int privateField;
		public    int publicField;
		protected int protectedField;
		internal  int internalField;
		static    int staticField;
		
		private   int privateProperty { get { return 0; } }
		public    int publicProperty { get { return 0; } }
		protected int protectedProperty { get { return 0; } }
		internal  int internalProperty { get { return 0; } }
		static    int staticProperty { get { return 0; } }
		
		private   void privateMethod() {}
		public    void publicMethod() {}
		protected void protectedMethod() {}
		internal  void internalMethod() {}
		static    void staticMethod() {}
		
		public static void Main()
		{
			System.Diagnostics.Debugger.Break();
		}
	}
	
	public class Metadata2 {}
	public class Metadata3 {}
	public class Metadata4 {}
	public class Metadata5 {}
	public class Metadata6 {}
	public class Metadata7 {}
}

#if TEST_CODE
namespace Debugger.Tests {
	using Debugger.MetaData;
	
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void Metadata()
		{
			StartTest("Metadata.cs");
			
			ObjectDump("Members", process.SelectedStackFrame.MethodInfo.DeclaringType.GetMembers(BindingFlags.All));
			ObjectDump("Types", process.SelectedStackFrame.MethodInfo.Module.GetNamesOfDefinedTypes());
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="Metadata.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>Metadata.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break Metadata.cs:39,4-39,40</DebuggingPaused>
    <Members
      Capacity="64"
      Count="36">
      <Item>
        <FieldInfo
          DeclaringType="Debugger.Tests.TestPrograms.Metadata"
          FullName="Debugger.Tests.TestPrograms.Metadata.privateField"
          IsInternal="False"
          IsLiteral="False"
          IsPrivate="True"
          IsProtected="False"
          IsPublic="False"
          IsStatic="False"
          Module="Metadata.exe"
          Name="privateField" />
      </Item>
      <Item>
        <FieldInfo
          DeclaringType="Debugger.Tests.TestPrograms.Metadata"
          FullName="Debugger.Tests.TestPrograms.Metadata.publicField"
          IsInternal="False"
          IsLiteral="False"
          IsPrivate="False"
          IsProtected="False"
          IsPublic="True"
          IsStatic="False"
          Module="Metadata.exe"
          Name="publicField" />
      </Item>
      <Item>
        <FieldInfo
          DeclaringType="Debugger.Tests.TestPrograms.Metadata"
          FullName="Debugger.Tests.TestPrograms.Metadata.protectedField"
          IsInternal="False"
          IsLiteral="False"
          IsPrivate="False"
          IsProtected="True"
          IsPublic="False"
          IsStatic="False"
          Module="Metadata.exe"
          Name="protectedField" />
      </Item>
      <Item>
        <FieldInfo
          DeclaringType="Debugger.Tests.TestPrograms.Metadata"
          FullName="Debugger.Tests.TestPrograms.Metadata.internalField"
          IsInternal="True"
          IsLiteral="False"
          IsPrivate="False"
          IsProtected="False"
          IsPublic="False"
          IsStatic="False"
          Module="Metadata.exe"
          Name="internalField" />
      </Item>
      <Item>
        <FieldInfo
          DeclaringType="Debugger.Tests.TestPrograms.Metadata"
          FullName="Debugger.Tests.TestPrograms.Metadata.staticField"
          IsInternal="False"
          IsLiteral="False"
          IsPrivate="True"
          IsProtected="False"
          IsPublic="False"
          IsStatic="True"
          Module="Metadata.exe"
          Name="staticField" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Debugger.Tests.TestPrograms.Metadata"
          FullName="Debugger.Tests.TestPrograms.Metadata.get_privateProperty"
          IsInternal="False"
          IsPrivate="True"
          IsProtected="False"
          IsPublic="False"
          IsSpecialName="True"
          IsStatic="False"
          Module="Metadata.exe"
          Name="get_privateProperty"
          StepOver="False" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Debugger.Tests.TestPrograms.Metadata"
          FullName="Debugger.Tests.TestPrograms.Metadata.get_publicProperty"
          IsInternal="False"
          IsPrivate="False"
          IsProtected="False"
          IsPublic="True"
          IsSpecialName="True"
          IsStatic="False"
          Module="Metadata.exe"
          Name="get_publicProperty"
          StepOver="False" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Debugger.Tests.TestPrograms.Metadata"
          FullName="Debugger.Tests.TestPrograms.Metadata.get_protectedProperty"
          IsInternal="False"
          IsPrivate="False"
          IsProtected="True"
          IsPublic="False"
          IsSpecialName="True"
          IsStatic="False"
          Module="Metadata.exe"
          Name="get_protectedProperty"
          StepOver="False" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Debugger.Tests.TestPrograms.Metadata"
          FullName="Debugger.Tests.TestPrograms.Metadata.get_internalProperty"
          IsInternal="True"
          IsPrivate="False"
          IsProtected="False"
          IsPublic="False"
          IsSpecialName="True"
          IsStatic="False"
          Module="Metadata.exe"
          Name="get_internalProperty"
          StepOver="False" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Debugger.Tests.TestPrograms.Metadata"
          FullName="Debugger.Tests.TestPrograms.Metadata.get_staticProperty"
          IsInternal="False"
          IsPrivate="True"
          IsProtected="False"
          IsPublic="False"
          IsSpecialName="True"
          IsStatic="True"
          Module="Metadata.exe"
          Name="get_staticProperty"
          StepOver="False" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Debugger.Tests.TestPrograms.Metadata"
          FullName="Debugger.Tests.TestPrograms.Metadata.privateMethod"
          IsInternal="False"
          IsPrivate="True"
          IsProtected="False"
          IsPublic="False"
          IsSpecialName="False"
          IsStatic="False"
          Module="Metadata.exe"
          Name="privateMethod"
          StepOver="False" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Debugger.Tests.TestPrograms.Metadata"
          FullName="Debugger.Tests.TestPrograms.Metadata.publicMethod"
          IsInternal="False"
          IsPrivate="False"
          IsProtected="False"
          IsPublic="True"
          IsSpecialName="False"
          IsStatic="False"
          Module="Metadata.exe"
          Name="publicMethod"
          StepOver="False" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Debugger.Tests.TestPrograms.Metadata"
          FullName="Debugger.Tests.TestPrograms.Metadata.protectedMethod"
          IsInternal="False"
          IsPrivate="False"
          IsProtected="True"
          IsPublic="False"
          IsSpecialName="False"
          IsStatic="False"
          Module="Metadata.exe"
          Name="protectedMethod"
          StepOver="False" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Debugger.Tests.TestPrograms.Metadata"
          FullName="Debugger.Tests.TestPrograms.Metadata.internalMethod"
          IsInternal="True"
          IsPrivate="False"
          IsProtected="False"
          IsPublic="False"
          IsSpecialName="False"
          IsStatic="False"
          Module="Metadata.exe"
          Name="internalMethod"
          StepOver="False" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Debugger.Tests.TestPrograms.Metadata"
          FullName="Debugger.Tests.TestPrograms.Metadata.staticMethod"
          IsInternal="False"
          IsPrivate="True"
          IsProtected="False"
          IsPublic="False"
          IsSpecialName="False"
          IsStatic="True"
          Module="Metadata.exe"
          Name="staticMethod"
          StepOver="False" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Debugger.Tests.TestPrograms.Metadata"
          FullName="Debugger.Tests.TestPrograms.Metadata.Main"
          IsInternal="False"
          IsPrivate="False"
          IsProtected="False"
          IsPublic="True"
          IsSpecialName="False"
          IsStatic="True"
          Module="Metadata.exe"
          Name="Main"
          StepOver="False" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Debugger.Tests.TestPrograms.Metadata"
          FullName="Debugger.Tests.TestPrograms.Metadata..ctor"
          IsInternal="False"
          IsPrivate="False"
          IsProtected="False"
          IsPublic="True"
          IsSpecialName="True"
          IsStatic="False"
          Module="Metadata.exe"
          Name=".ctor"
          StepOver="True" />
      </Item>
      <Item>
        <PropertyInfo
          DeclaringType="Debugger.Tests.TestPrograms.Metadata"
          FullName="Debugger.Tests.TestPrograms.Metadata.privateProperty"
          GetMethod="get_privateProperty"
          IsInternal="False"
          IsPrivate="True"
          IsProtected="False"
          IsPublic="False"
          IsStatic="False"
          Module="Metadata.exe"
          Name="privateProperty"
          SetMethod="null" />
      </Item>
      <Item>
        <PropertyInfo
          DeclaringType="Debugger.Tests.TestPrograms.Metadata"
          FullName="Debugger.Tests.TestPrograms.Metadata.publicProperty"
          GetMethod="get_publicProperty"
          IsInternal="False"
          IsPrivate="False"
          IsProtected="False"
          IsPublic="True"
          IsStatic="False"
          Module="Metadata.exe"
          Name="publicProperty"
          SetMethod="null" />
      </Item>
      <Item>
        <PropertyInfo
          DeclaringType="Debugger.Tests.TestPrograms.Metadata"
          FullName="Debugger.Tests.TestPrograms.Metadata.protectedProperty"
          GetMethod="get_protectedProperty"
          IsInternal="False"
          IsPrivate="False"
          IsProtected="True"
          IsPublic="False"
          IsStatic="False"
          Module="Metadata.exe"
          Name="protectedProperty"
          SetMethod="null" />
      </Item>
      <Item>
        <PropertyInfo
          DeclaringType="Debugger.Tests.TestPrograms.Metadata"
          FullName="Debugger.Tests.TestPrograms.Metadata.internalProperty"
          GetMethod="get_internalProperty"
          IsInternal="True"
          IsPrivate="False"
          IsProtected="False"
          IsPublic="False"
          IsStatic="False"
          Module="Metadata.exe"
          Name="internalProperty"
          SetMethod="null" />
      </Item>
      <Item>
        <PropertyInfo
          DeclaringType="Debugger.Tests.TestPrograms.Metadata"
          FullName="Debugger.Tests.TestPrograms.Metadata.staticProperty"
          GetMethod="get_staticProperty"
          IsInternal="False"
          IsPrivate="True"
          IsProtected="False"
          IsPublic="False"
          IsStatic="True"
          Module="Metadata.exe"
          Name="staticProperty"
          SetMethod="null" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="System.Object"
          FullName="System.Object..ctor"
          IsInternal="False"
          IsPrivate="False"
          IsProtected="False"
          IsPublic="True"
          IsSpecialName="True"
          IsStatic="False"
          Module="mscorlib.dll"
          Name=".ctor"
          StepOver="True" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="System.Object"
          FullName="System.Object.ToString"
          IsInternal="False"
          IsPrivate="False"
          IsProtected="False"
          IsPublic="True"
          IsSpecialName="False"
          IsStatic="False"
          Module="mscorlib.dll"
          Name="ToString"
          StepOver="True" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="System.Object"
          FullName="System.Object.Equals"
          IsInternal="False"
          IsPrivate="False"
          IsProtected="False"
          IsPublic="True"
          IsSpecialName="False"
          IsStatic="False"
          Module="mscorlib.dll"
          Name="Equals"
          StepOver="True" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="System.Object"
          FullName="System.Object.InternalEquals"
          IsInternal="True"
          IsPrivate="False"
          IsProtected="False"
          IsPublic="False"
          IsSpecialName="False"
          IsStatic="True"
          Module="mscorlib.dll"
          Name="InternalEquals"
          StepOver="True" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="System.Object"
          FullName="System.Object.Equals"
          IsInternal="False"
          IsPrivate="False"
          IsProtected="False"
          IsPublic="True"
          IsSpecialName="False"
          IsStatic="True"
          Module="mscorlib.dll"
          Name="Equals"
          StepOver="True" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="System.Object"
          FullName="System.Object.ReferenceEquals"
          IsInternal="False"
          IsPrivate="False"
          IsProtected="False"
          IsPublic="True"
          IsSpecialName="False"
          IsStatic="True"
          Module="mscorlib.dll"
          Name="ReferenceEquals"
          StepOver="True" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="System.Object"
          FullName="System.Object.GetHashCode"
          IsInternal="False"
          IsPrivate="False"
          IsProtected="False"
          IsPublic="True"
          IsSpecialName="False"
          IsStatic="False"
          Module="mscorlib.dll"
          Name="GetHashCode"
          StepOver="True" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="System.Object"
          FullName="System.Object.InternalGetHashCode"
          IsInternal="True"
          IsPrivate="False"
          IsProtected="False"
          IsPublic="False"
          IsSpecialName="False"
          IsStatic="True"
          Module="mscorlib.dll"
          Name="InternalGetHashCode"
          StepOver="True" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="System.Object"
          FullName="System.Object.GetType"
          IsInternal="False"
          IsPrivate="False"
          IsProtected="False"
          IsPublic="True"
          IsSpecialName="False"
          IsStatic="False"
          Module="mscorlib.dll"
          Name="GetType"
          StepOver="True" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="System.Object"
          FullName="System.Object.Finalize"
          IsInternal="False"
          IsPrivate="False"
          IsProtected="True"
          IsPublic="False"
          IsSpecialName="False"
          IsStatic="False"
          Module="mscorlib.dll"
          Name="Finalize"
          StepOver="True" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="System.Object"
          FullName="System.Object.MemberwiseClone"
          IsInternal="False"
          IsPrivate="False"
          IsProtected="True"
          IsPublic="False"
          IsSpecialName="False"
          IsStatic="False"
          Module="mscorlib.dll"
          Name="MemberwiseClone"
          StepOver="True" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="System.Object"
          FullName="System.Object.FieldSetter"
          IsInternal="False"
          IsPrivate="True"
          IsProtected="False"
          IsPublic="False"
          IsSpecialName="False"
          IsStatic="False"
          Module="mscorlib.dll"
          Name="FieldSetter"
          StepOver="True" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="System.Object"
          FullName="System.Object.FieldGetter"
          IsInternal="False"
          IsPrivate="True"
          IsProtected="False"
          IsPublic="False"
          IsSpecialName="False"
          IsStatic="False"
          Module="mscorlib.dll"
          Name="FieldGetter"
          StepOver="True" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="System.Object"
          FullName="System.Object.GetFieldInfo"
          IsInternal="False"
          IsPrivate="True"
          IsProtected="False"
          IsPublic="False"
          IsSpecialName="False"
          IsStatic="False"
          Module="mscorlib.dll"
          Name="GetFieldInfo"
          StepOver="True" />
      </Item>
    </Members>
    <Types
      Capacity="8"
      Count="7">
      <Item>Debugger.Tests.TestPrograms.Metadata</Item>
      <Item>Debugger.Tests.TestPrograms.Metadata2</Item>
      <Item>Debugger.Tests.TestPrograms.Metadata3</Item>
      <Item>Debugger.Tests.TestPrograms.Metadata4</Item>
      <Item>Debugger.Tests.TestPrograms.Metadata5</Item>
      <Item>Debugger.Tests.TestPrograms.Metadata6</Item>
      <Item>Debugger.Tests.TestPrograms.Metadata7</Item>
    </Types>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT