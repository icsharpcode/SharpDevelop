// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using IronPython.Compiler.Ast;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the string "System.Windows.Forms.Cursors.AppStarting" can be resolved by the 
	/// PythonCodeDeserializer.
	/// </summary>
	[TestFixture]
	public class CursorTypeResolutionTestFixture
	{
		string pythonCode = "self.Cursors = System.Windows.Forms.Cursors.AppStarting";
		Node rhsAssignmentNode;
		object obj;
		MockDesignerLoaderHost mockDesignerLoaderHost;
		MockTypeResolutionService typeResolutionService;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			PythonParser parser = new PythonParser();
			PythonAst ast = parser.CreateAst(@"snippet.py", pythonCode);
			SuiteStatement suiteStatement = (SuiteStatement)ast.Body;
			AssignmentStatement assignment = suiteStatement.Statements[0] as AssignmentStatement;
			rhsAssignmentNode = assignment.Right;
			
			mockDesignerLoaderHost = new MockDesignerLoaderHost();
			typeResolutionService = mockDesignerLoaderHost.TypeResolutionService;
			PythonCodeDeserializer deserializer = new PythonCodeDeserializer(mockDesignerLoaderHost);
			obj = deserializer.Deserialize(rhsAssignmentNode);
		}
		
		[Test]
		public void RhsAssignmentNodeExists()
		{
			Assert.IsNotNull(rhsAssignmentNode);
		}
		
		[Test]
		public void DeserializedObjectIsCursorsAppStarting()
		{
			Assert.AreEqual(Cursors.AppStarting, obj);
		}
		
		[Test]
		public void CursorsTypeResolved()
		{
			Assert.AreEqual("System.Windows.Forms.Cursors", typeResolutionService.LastTypeNameResolved);
		}
	}
}
