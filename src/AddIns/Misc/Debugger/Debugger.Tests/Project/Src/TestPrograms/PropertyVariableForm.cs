// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

namespace Debugger.Tests.TestPrograms
{
	public class PropertyVariableForm
	{
		public static void Main()
		{
			Form form = new Form();
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
		public void PropertyVariableForm()
		{
			Variable local = null;
			
			StartProgram("PropertyVariableForm.cs");
			WaitForPause();
			foreach(Variable var in process.SelectedFunction.LocalVariables) {
				local = var;
			}
			Assert.AreEqual("form", local.Name);
			Assert.AreEqual(typeof(Variable), local.GetType());
			
			foreach(Variable var in local.Value.SubVariables) {
				Assert.AreEqual(typeof(UnavailableValue), var.Value.GetType(), "Variable name: " + var.Name);
				process.StartEvaluation();
				WaitForPause();
				Assert.AreNotEqual(null, var.Value.AsString, "Variable name: " + var.Name);
			}
			
			process.Continue();
			WaitForPause();
			
			foreach(Variable var in local.Value.SubVariables) {
				Assert.AreEqual(typeof(UnavailableValue), var.Value.GetType(), "Variable name: " + var.Name);
			}
			process.StartEvaluation();
			WaitForPause();
			
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