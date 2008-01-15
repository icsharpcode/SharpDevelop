// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class PropertyVariable
	{
		string PrivateProperty {
			get {
				return "private";
			}
		}
		
		public string PublicProperty {
			get {
				return "public";
			}
		}
		
		public string ExceptionProperty {
			get {
				throw new NotSupportedException();
			}
		}
		
		public static string StaticProperty {
			get {
				return "static";
			}
		}
		
		public static void Main()
		{
			PropertyVariable var = new PropertyVariable();
			System.Diagnostics.Debugger.Break();
			System.Diagnostics.Debugger.Break();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		/*
		[NUnit.Framework.Test]
		public void PropertyVariable()
		{
			StartProgram("PropertyVariable.cs");
			WaitForPause();
			NamedValueCollection props = process.SelectedFunction.LocalVariables["var"].GetMembers(null, Debugger.BindingFlags.All);
			
			Assert.AreEqual(typeof(UnavailableValue), props["PrivateProperty"].Value.GetType());
			process.StartEvaluation();
			WaitForPause();
			Assert.AreEqual("private", props["PrivateProperty"].AsString);
			
			Assert.AreEqual(typeof(UnavailableValue), props["PublicProperty"].Value.GetType());
			process.StartEvaluation();
			WaitForPause();
			Assert.AreEqual("public", props["PublicProperty"].AsString);
			
			Assert.AreEqual(typeof(UnavailableValue), props["ExceptionProperty"].Value.GetType());
			process.StartEvaluation();
			WaitForPause();
			Assert.AreEqual(typeof(UnavailableValue), props["ExceptionProperty"].Value.GetType());
			
			Assert.AreEqual(typeof(UnavailableValue), props["StaticProperty"].Value.GetType());
			process.StartEvaluation();
			WaitForPause();
			Assert.AreEqual("static", props["StaticProperty"].AsString);
			
			process.Continue();
			WaitForPause(PausedReason.Break, null);
			
			process.Continue();
			process.WaitForPrecessExit();
			CheckXmlOutput();
		}
		*/
	}
}
#endif

#if EXPECTED_OUTPUT
#endif // EXPECTED_OUTPUT