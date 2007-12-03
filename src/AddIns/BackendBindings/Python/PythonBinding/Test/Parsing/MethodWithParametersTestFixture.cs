// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor.Document;
using NUnit.Framework;
using PythonBinding.Tests;

namespace PythonBinding.Tests.Parsing
{
	/// <summary>
	/// Description of MethodWithParametersTestFixture.
	/// </summary>
	[TestFixture]
	public class MethodWithParametersTestFixture
	{
		IMethod method;
		IParameter senderParameter;
		IParameter eventArgsParameter;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string python = "class Test(Base):\r\n" +
							"\tdef foo(self, sender, e):\r\n" +
							"\t\tpass";
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			PythonParser parser = new PythonParser();
			ICompilationUnit compilationUnit = parser.Parse(projectContent, @"C:\test.py", python);			
			if (compilationUnit.Classes.Count > 0) {
				IClass c = compilationUnit.Classes[0];
				method = c.Methods[0];
				if (method.Parameters.Count > 1) {
					senderParameter = method.Parameters[0];
					eventArgsParameter = method.Parameters[1];
				}
			}
		}
		
		[Test]
		public void MethodHasTwoParameters()
		{
			Assert.AreEqual(2, method.Parameters.Count);
		}
		
		[Test]
		public void FirstParameterIsSender()
		{
			Assert.AreEqual("sender", senderParameter.Name);
		}
		
		[Test]
		public void SecondParameterIsEventArgs()
		{
			Assert.AreEqual("e", eventArgsParameter.Name);
		}
	}
}
