// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using IronPython.Compiler.Ast;
using Microsoft.Scripting;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the PythonFormVisitor throws an exception if no InitializeComponent or 
	/// InitializeComponent method can be found.
	/// </summary>
	[TestFixture]
	public class MissingInitializeComponentMethodTestFixture : LoadFormTestFixtureBase
	{		
		string pythonCode = "from System.Windows.Forms import Form\r\n" +
							"\r\n" +
							"class MainForm(System.Windows.Forms.Form):\r\n" +
							"    def __init__(self):\r\n" +
							"    self.MissingMethod()\r\n" +
							"\r\n" +
							"    def MissingMethod(self):\r\n" +
							"        pass\r\n";		
		[Test]
		[ExpectedException(typeof(PythonFormWalkerException))]
		public void PythonFormWalkerExceptionThrown()
		{
			PythonFormWalker walker = new PythonFormWalker(this, new MockDesignerLoaderHost());
			walker.CreateForm(pythonCode);
			Assert.Fail("Exception should have been thrown before this.");
		}
		
		/// <summary>
		/// Check that the PythonFormWalker does not try to walk the class body if it is null.
		/// </summary>
		[Test]
		public void ClassWithNoBody()
		{
			ClassDefinition classDef = new ClassDefinition(new SymbolId(10), null, null);
			PythonFormWalker walker = new PythonFormWalker(this, new MockDesignerLoaderHost());
			walker.Walk(classDef);
		}
		
		/// <summary>
		/// Make sure we do not get an ArgumentOutOfRangeException when walking the
		/// AssignmentStatement.
		/// </summary>
		[Test]
		public void NoLeftHandSideExpressionsInAssignment()
		{
			List<Expression> lhs = new List<Expression>();
			AssignmentStatement assign = new AssignmentStatement(lhs.ToArray(), null);
			PythonFormWalker walker = new PythonFormWalker(this, new MockDesignerLoaderHost());
			walker.Walk(assign);
		}
	}
}
