// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class MainClass
	{
		public static void Main()
		{
			GenericClass<int, string> gClass = new GenericClass<int, string>();
			gClass.Metod(1, "1!");
			gClass.GenericMethod<bool>(1, "1!");
			GenericClass<int, string>.StaticMetod(1, "1!");
			GenericClass<int, string>.StaticGenericMethod<bool>(1, "1!");
			
			GenericStruct<int, string> gStruct = new GenericStruct<int, string>();
			gStruct.Metod(1, "1!");
			gStruct.GenericMethod<bool>(1, "1!");
			GenericStruct<int, string>.StaticMetod(1, "1!");
			GenericStruct<int, string>.StaticGenericMethod<bool>(1, "1!");
			
			System.Diagnostics.Debugger.Break();
		}
	}
	
	public class GenericClass<V, K>
	{
		public K Metod(V v, K k)
		{
			System.Diagnostics.Debugger.Break();
			return k;
		}
		
		public T GenericMethod<T>(V v, K k)
		{
			System.Diagnostics.Debugger.Break();
			return default(T);
		}
		
		public static K StaticMetod(V v, K k)
		{
			System.Diagnostics.Debugger.Break();
			return k;
		}
		
		public static T StaticGenericMethod<T>(V v, K k)
		{
			System.Diagnostics.Debugger.Break();
			return default(T);
		}
	}
	
	public struct GenericStruct<V, K>
	{
		public K Metod(V v, K k)
		{
			System.Diagnostics.Debugger.Break();
			return k;
		}
		
		public T GenericMethod<T>(V v, K k)
		{
			System.Diagnostics.Debugger.Break();
			return default(T);
		}
		
		public static K StaticMetod(V v, K k)
		{
			System.Diagnostics.Debugger.Break();
			return k;
		}
		
		public static T StaticGenericMethod<T>(V v, K k)
		{
			System.Diagnostics.Debugger.Break();
			return default(T);
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void Generics()
		{
			StartTest("Generics");
			
			for(int i = 0; i < 8; i++) {
				WaitForPause();
				ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
				process.Continue();
			}
			
			WaitForPause();
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test name="Generics">
    <ProcessStarted />
    <ModuleLoaded symbols="False">mscorlib.dll</ModuleLoaded>
    <ModuleLoaded symbols="True">Generics.exe</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo Type="MethodInfo">
        <Name>Metod</Name>
        <FullName>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;.Metod</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>False</IsStatic>
        <Module>Generics.exe</Module>
        <DeclaringType>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;</DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=36,4 End=36,40</NextStatement>
      <ThisValue Type="Value">
        <IsArray>False</IsArray>
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <ArrayDimensions exception="Value is not an array" />
        <IsObject>True</IsObject>
        <IsPrimitive>False</IsPrimitive>
        <IsInteger>False</IsInteger>
        <PrimitiveValue exception="Value is not a primitive type" />
        <Expression>this</Expression>
        <Name>this</Name>
        <IsNull>False</IsNull>
        <AsString>{Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;}</AsString>
        <HasExpired>False</HasExpired>
        <Type>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;</Type>
      </ThisValue>
      <ContaingClassVariables Type="ValueCollection">
        <Count>0</Count>
      </ContaingClassVariables>
      <ArgumentCount>2</ArgumentCount>
      <Arguments Type="ValueCollection">
        <Count>2</Count>
        <Item Type="Value">
          <IsArray>False</IsArray>
          <ArrayLenght exception="Value is not an array" />
          <ArrayRank exception="Value is not an array" />
          <ArrayDimensions exception="Value is not an array" />
          <IsObject>False</IsObject>
          <IsPrimitive>True</IsPrimitive>
          <IsInteger>True</IsInteger>
          <PrimitiveValue>1</PrimitiveValue>
          <Expression>v</Expression>
          <Name>v</Name>
          <IsNull>False</IsNull>
          <AsString>1</AsString>
          <HasExpired>False</HasExpired>
          <Type>System.Int32</Type>
        </Item>
        <Item Type="Value">
          <IsArray>False</IsArray>
          <ArrayLenght exception="Value is not an array" />
          <ArrayRank exception="Value is not an array" />
          <ArrayDimensions exception="Value is not an array" />
          <IsObject>False</IsObject>
          <IsPrimitive>True</IsPrimitive>
          <IsInteger>False</IsInteger>
          <PrimitiveValue>1!</PrimitiveValue>
          <Expression>k</Expression>
          <Name>k</Name>
          <IsNull>False</IsNull>
          <AsString>1!</AsString>
          <HasExpired>False</HasExpired>
          <Type>System.String</Type>
        </Item>
      </Arguments>
      <LocalVariables Type="ValueCollection">
        <Count>0</Count>
      </LocalVariables>
    </ObjectDump>
    <DebuggingPaused>Break</DebuggingPaused>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo Type="MethodInfo">
        <Name>GenericMethod</Name>
        <FullName>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;.GenericMethod</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>False</IsStatic>
        <Module>Generics.exe</Module>
        <DeclaringType>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;</DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=42,4 End=42,40</NextStatement>
      <ThisValue Type="Value">
        <IsArray>False</IsArray>
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <ArrayDimensions exception="Value is not an array" />
        <IsObject>True</IsObject>
        <IsPrimitive>False</IsPrimitive>
        <IsInteger>False</IsInteger>
        <PrimitiveValue exception="Value is not a primitive type" />
        <Expression>this</Expression>
        <Name>this</Name>
        <IsNull>False</IsNull>
        <AsString>{Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;}</AsString>
        <HasExpired>False</HasExpired>
        <Type>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;</Type>
      </ThisValue>
      <ContaingClassVariables Type="ValueCollection">
        <Count>0</Count>
      </ContaingClassVariables>
      <ArgumentCount>2</ArgumentCount>
      <Arguments Type="ValueCollection">
        <Count>2</Count>
        <Item Type="Value">
          <IsArray>False</IsArray>
          <ArrayLenght exception="Value is not an array" />
          <ArrayRank exception="Value is not an array" />
          <ArrayDimensions exception="Value is not an array" />
          <IsObject>False</IsObject>
          <IsPrimitive>True</IsPrimitive>
          <IsInteger>True</IsInteger>
          <PrimitiveValue>1</PrimitiveValue>
          <Expression>v</Expression>
          <Name>v</Name>
          <IsNull>False</IsNull>
          <AsString>1</AsString>
          <HasExpired>False</HasExpired>
          <Type>System.Int32</Type>
        </Item>
        <Item Type="Value">
          <IsArray>False</IsArray>
          <ArrayLenght exception="Value is not an array" />
          <ArrayRank exception="Value is not an array" />
          <ArrayDimensions exception="Value is not an array" />
          <IsObject>False</IsObject>
          <IsPrimitive>True</IsPrimitive>
          <IsInteger>False</IsInteger>
          <PrimitiveValue>1!</PrimitiveValue>
          <Expression>k</Expression>
          <Name>k</Name>
          <IsNull>False</IsNull>
          <AsString>1!</AsString>
          <HasExpired>False</HasExpired>
          <Type>System.String</Type>
        </Item>
      </Arguments>
      <LocalVariables Type="ValueCollection">
        <Count>0</Count>
      </LocalVariables>
    </ObjectDump>
    <DebuggingPaused>Break</DebuggingPaused>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo Type="MethodInfo">
        <Name>StaticMetod</Name>
        <FullName>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;.StaticMetod</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <Module>Generics.exe</Module>
        <DeclaringType>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;</DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=48,4 End=48,40</NextStatement>
      <ThisValue exception="Static method does not have 'this'." />
      <ContaingClassVariables Type="ValueCollection">
        <Count>0</Count>
      </ContaingClassVariables>
      <ArgumentCount>2</ArgumentCount>
      <Arguments Type="ValueCollection">
        <Count>2</Count>
        <Item Type="Value">
          <IsArray>False</IsArray>
          <ArrayLenght exception="Value is not an array" />
          <ArrayRank exception="Value is not an array" />
          <ArrayDimensions exception="Value is not an array" />
          <IsObject>False</IsObject>
          <IsPrimitive>True</IsPrimitive>
          <IsInteger>True</IsInteger>
          <PrimitiveValue>1</PrimitiveValue>
          <Expression>v</Expression>
          <Name>v</Name>
          <IsNull>False</IsNull>
          <AsString>1</AsString>
          <HasExpired>False</HasExpired>
          <Type>System.Int32</Type>
        </Item>
        <Item Type="Value">
          <IsArray>False</IsArray>
          <ArrayLenght exception="Value is not an array" />
          <ArrayRank exception="Value is not an array" />
          <ArrayDimensions exception="Value is not an array" />
          <IsObject>False</IsObject>
          <IsPrimitive>True</IsPrimitive>
          <IsInteger>False</IsInteger>
          <PrimitiveValue>1!</PrimitiveValue>
          <Expression>k</Expression>
          <Name>k</Name>
          <IsNull>False</IsNull>
          <AsString>1!</AsString>
          <HasExpired>False</HasExpired>
          <Type>System.String</Type>
        </Item>
      </Arguments>
      <LocalVariables Type="ValueCollection">
        <Count>0</Count>
      </LocalVariables>
    </ObjectDump>
    <DebuggingPaused>Break</DebuggingPaused>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo Type="MethodInfo">
        <Name>StaticGenericMethod</Name>
        <FullName>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;.StaticGenericMethod</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <Module>Generics.exe</Module>
        <DeclaringType>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;</DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=54,4 End=54,40</NextStatement>
      <ThisValue exception="Static method does not have 'this'." />
      <ContaingClassVariables Type="ValueCollection">
        <Count>0</Count>
      </ContaingClassVariables>
      <ArgumentCount>2</ArgumentCount>
      <Arguments Type="ValueCollection">
        <Count>2</Count>
        <Item Type="Value">
          <IsArray>False</IsArray>
          <ArrayLenght exception="Value is not an array" />
          <ArrayRank exception="Value is not an array" />
          <ArrayDimensions exception="Value is not an array" />
          <IsObject>False</IsObject>
          <IsPrimitive>True</IsPrimitive>
          <IsInteger>True</IsInteger>
          <PrimitiveValue>1</PrimitiveValue>
          <Expression>v</Expression>
          <Name>v</Name>
          <IsNull>False</IsNull>
          <AsString>1</AsString>
          <HasExpired>False</HasExpired>
          <Type>System.Int32</Type>
        </Item>
        <Item Type="Value">
          <IsArray>False</IsArray>
          <ArrayLenght exception="Value is not an array" />
          <ArrayRank exception="Value is not an array" />
          <ArrayDimensions exception="Value is not an array" />
          <IsObject>False</IsObject>
          <IsPrimitive>True</IsPrimitive>
          <IsInteger>False</IsInteger>
          <PrimitiveValue>1!</PrimitiveValue>
          <Expression>k</Expression>
          <Name>k</Name>
          <IsNull>False</IsNull>
          <AsString>1!</AsString>
          <HasExpired>False</HasExpired>
          <Type>System.String</Type>
        </Item>
      </Arguments>
      <LocalVariables Type="ValueCollection">
        <Count>0</Count>
      </LocalVariables>
    </ObjectDump>
    <DebuggingPaused>Break</DebuggingPaused>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo Type="MethodInfo">
        <Name>Metod</Name>
        <FullName>Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;.Metod</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>False</IsStatic>
        <Module>Generics.exe</Module>
        <DeclaringType>Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;</DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=63,4 End=63,40</NextStatement>
      <ThisValue Type="Value">
        <IsArray>False</IsArray>
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <ArrayDimensions exception="Value is not an array" />
        <IsObject>True</IsObject>
        <IsPrimitive>False</IsPrimitive>
        <IsInteger>False</IsInteger>
        <PrimitiveValue exception="Value is not a primitive type" />
        <Expression>this</Expression>
        <Name>this</Name>
        <IsNull>False</IsNull>
        <AsString>{Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;}</AsString>
        <HasExpired>False</HasExpired>
        <Type>Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;</Type>
      </ThisValue>
      <ContaingClassVariables Type="ValueCollection">
        <Count>0</Count>
      </ContaingClassVariables>
      <ArgumentCount>2</ArgumentCount>
      <Arguments Type="ValueCollection">
        <Count>2</Count>
        <Item Type="Value">
          <IsArray>False</IsArray>
          <ArrayLenght exception="Value is not an array" />
          <ArrayRank exception="Value is not an array" />
          <ArrayDimensions exception="Value is not an array" />
          <IsObject>False</IsObject>
          <IsPrimitive>True</IsPrimitive>
          <IsInteger>True</IsInteger>
          <PrimitiveValue>1</PrimitiveValue>
          <Expression>v</Expression>
          <Name>v</Name>
          <IsNull>False</IsNull>
          <AsString>1</AsString>
          <HasExpired>False</HasExpired>
          <Type>System.Int32</Type>
        </Item>
        <Item Type="Value">
          <IsArray>False</IsArray>
          <ArrayLenght exception="Value is not an array" />
          <ArrayRank exception="Value is not an array" />
          <ArrayDimensions exception="Value is not an array" />
          <IsObject>False</IsObject>
          <IsPrimitive>True</IsPrimitive>
          <IsInteger>False</IsInteger>
          <PrimitiveValue>1!</PrimitiveValue>
          <Expression>k</Expression>
          <Name>k</Name>
          <IsNull>False</IsNull>
          <AsString>1!</AsString>
          <HasExpired>False</HasExpired>
          <Type>System.String</Type>
        </Item>
      </Arguments>
      <LocalVariables Type="ValueCollection">
        <Count>0</Count>
      </LocalVariables>
    </ObjectDump>
    <DebuggingPaused>Break</DebuggingPaused>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo Type="MethodInfo">
        <Name>GenericMethod</Name>
        <FullName>Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;.GenericMethod</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>False</IsStatic>
        <Module>Generics.exe</Module>
        <DeclaringType>Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;</DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=69,4 End=69,40</NextStatement>
      <ThisValue Type="Value">
        <IsArray>False</IsArray>
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <ArrayDimensions exception="Value is not an array" />
        <IsObject>True</IsObject>
        <IsPrimitive>False</IsPrimitive>
        <IsInteger>False</IsInteger>
        <PrimitiveValue exception="Value is not a primitive type" />
        <Expression>this</Expression>
        <Name>this</Name>
        <IsNull>False</IsNull>
        <AsString>{Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;}</AsString>
        <HasExpired>False</HasExpired>
        <Type>Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;</Type>
      </ThisValue>
      <ContaingClassVariables Type="ValueCollection">
        <Count>0</Count>
      </ContaingClassVariables>
      <ArgumentCount>2</ArgumentCount>
      <Arguments Type="ValueCollection">
        <Count>2</Count>
        <Item Type="Value">
          <IsArray>False</IsArray>
          <ArrayLenght exception="Value is not an array" />
          <ArrayRank exception="Value is not an array" />
          <ArrayDimensions exception="Value is not an array" />
          <IsObject>False</IsObject>
          <IsPrimitive>True</IsPrimitive>
          <IsInteger>True</IsInteger>
          <PrimitiveValue>1</PrimitiveValue>
          <Expression>v</Expression>
          <Name>v</Name>
          <IsNull>False</IsNull>
          <AsString>1</AsString>
          <HasExpired>False</HasExpired>
          <Type>System.Int32</Type>
        </Item>
        <Item Type="Value">
          <IsArray>False</IsArray>
          <ArrayLenght exception="Value is not an array" />
          <ArrayRank exception="Value is not an array" />
          <ArrayDimensions exception="Value is not an array" />
          <IsObject>False</IsObject>
          <IsPrimitive>True</IsPrimitive>
          <IsInteger>False</IsInteger>
          <PrimitiveValue>1!</PrimitiveValue>
          <Expression>k</Expression>
          <Name>k</Name>
          <IsNull>False</IsNull>
          <AsString>1!</AsString>
          <HasExpired>False</HasExpired>
          <Type>System.String</Type>
        </Item>
      </Arguments>
      <LocalVariables Type="ValueCollection">
        <Count>0</Count>
      </LocalVariables>
    </ObjectDump>
    <DebuggingPaused>Break</DebuggingPaused>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo Type="MethodInfo">
        <Name>StaticMetod</Name>
        <FullName>Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;.StaticMetod</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <Module>Generics.exe</Module>
        <DeclaringType>Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;</DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=75,4 End=75,40</NextStatement>
      <ThisValue exception="Static method does not have 'this'." />
      <ContaingClassVariables Type="ValueCollection">
        <Count>0</Count>
      </ContaingClassVariables>
      <ArgumentCount>2</ArgumentCount>
      <Arguments Type="ValueCollection">
        <Count>2</Count>
        <Item Type="Value">
          <IsArray>False</IsArray>
          <ArrayLenght exception="Value is not an array" />
          <ArrayRank exception="Value is not an array" />
          <ArrayDimensions exception="Value is not an array" />
          <IsObject>False</IsObject>
          <IsPrimitive>True</IsPrimitive>
          <IsInteger>True</IsInteger>
          <PrimitiveValue>1</PrimitiveValue>
          <Expression>v</Expression>
          <Name>v</Name>
          <IsNull>False</IsNull>
          <AsString>1</AsString>
          <HasExpired>False</HasExpired>
          <Type>System.Int32</Type>
        </Item>
        <Item Type="Value">
          <IsArray>False</IsArray>
          <ArrayLenght exception="Value is not an array" />
          <ArrayRank exception="Value is not an array" />
          <ArrayDimensions exception="Value is not an array" />
          <IsObject>False</IsObject>
          <IsPrimitive>True</IsPrimitive>
          <IsInteger>False</IsInteger>
          <PrimitiveValue>1!</PrimitiveValue>
          <Expression>k</Expression>
          <Name>k</Name>
          <IsNull>False</IsNull>
          <AsString>1!</AsString>
          <HasExpired>False</HasExpired>
          <Type>System.String</Type>
        </Item>
      </Arguments>
      <LocalVariables Type="ValueCollection">
        <Count>0</Count>
      </LocalVariables>
    </ObjectDump>
    <DebuggingPaused>Break</DebuggingPaused>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo Type="MethodInfo">
        <Name>StaticGenericMethod</Name>
        <FullName>Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;.StaticGenericMethod</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <Module>Generics.exe</Module>
        <DeclaringType>Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;</DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=81,4 End=81,40</NextStatement>
      <ThisValue exception="Static method does not have 'this'." />
      <ContaingClassVariables Type="ValueCollection">
        <Count>0</Count>
      </ContaingClassVariables>
      <ArgumentCount>2</ArgumentCount>
      <Arguments Type="ValueCollection">
        <Count>2</Count>
        <Item Type="Value">
          <IsArray>False</IsArray>
          <ArrayLenght exception="Value is not an array" />
          <ArrayRank exception="Value is not an array" />
          <ArrayDimensions exception="Value is not an array" />
          <IsObject>False</IsObject>
          <IsPrimitive>True</IsPrimitive>
          <IsInteger>True</IsInteger>
          <PrimitiveValue>1</PrimitiveValue>
          <Expression>v</Expression>
          <Name>v</Name>
          <IsNull>False</IsNull>
          <AsString>1</AsString>
          <HasExpired>False</HasExpired>
          <Type>System.Int32</Type>
        </Item>
        <Item Type="Value">
          <IsArray>False</IsArray>
          <ArrayLenght exception="Value is not an array" />
          <ArrayRank exception="Value is not an array" />
          <ArrayDimensions exception="Value is not an array" />
          <IsObject>False</IsObject>
          <IsPrimitive>True</IsPrimitive>
          <IsInteger>False</IsInteger>
          <PrimitiveValue>1!</PrimitiveValue>
          <Expression>k</Expression>
          <Name>k</Name>
          <IsNull>False</IsNull>
          <AsString>1!</AsString>
          <HasExpired>False</HasExpired>
          <Type>System.String</Type>
        </Item>
      </Arguments>
      <LocalVariables Type="ValueCollection">
        <Count>0</Count>
      </LocalVariables>
    </ObjectDump>
    <DebuggingPaused>Break</DebuggingPaused>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT