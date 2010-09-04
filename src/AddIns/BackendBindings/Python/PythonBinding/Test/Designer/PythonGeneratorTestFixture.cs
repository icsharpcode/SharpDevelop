// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.IO;

using ICSharpCode.FormsDesigner;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class PythonGeneratorTestFixture
	{		
		[Test]
		public void GetMethodReplaceRegion()
		{
			MockMethod method = new MockMethod(MockClass.CreateMockClassWithoutAnyAttributes());
			DomRegion bodyRegion = new DomRegion(0, 4, 1, 4);
			method.BodyRegion = bodyRegion;
			DomRegion expectedRegion = new DomRegion(bodyRegion.BeginLine + 1, 1, bodyRegion.EndLine + 1, 1);
			DerivedPythonDesignerGenerator generator = new DerivedPythonDesignerGenerator();
			
			Assert.AreEqual(expectedRegion, generator.GetBodyRegionInDocument(method));
		}		
		
		[Test]
		public void GenerateEventHandlerWithEmptyMethodBody()
		{
			DerivedPythonDesignerGenerator generator = new DerivedPythonDesignerGenerator();
			string eventHandler = generator.CallCreateEventHandler("button1_click", String.Empty, "\t");
			string expectedEventHandler = "\tdef button1_click(self, sender, e):\r\n" +
										"\t\tpass";
			Assert.AreEqual(expectedEventHandler, eventHandler);
		}
		
		[Test]
		public void GenerateEventHandlerWithNullMethodBody()
		{
			DerivedPythonDesignerGenerator generator = new DerivedPythonDesignerGenerator();
			string eventHandler = generator.CallCreateEventHandler("button2_click", null, String.Empty);
			string expectedEventHandler = "def button2_click(self, sender, e):\r\n" +
											"\tpass";
			Assert.AreEqual(expectedEventHandler, eventHandler);
		}
		
	}
}
