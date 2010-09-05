// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.IO;

using ICSharpCode.FormsDesigner;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using RubyBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class RubyDesignerGeneratorTestFixture
	{
		RubyDesignerGenerator generator;
		
		[SetUp]
		public void Init()
		{
			MockTextEditorOptions options = new MockTextEditorOptions();
			generator = new RubyDesignerGenerator(options);
		}
		
		[Test]
		public void GetMethodReplaceRegion()
		{
			MockMethod method = MockMethod.CreateMockMethodWithoutAnyAttributes();
			DomRegion bodyRegion = new DomRegion(0, 4, 1, 4);
			method.BodyRegion = bodyRegion;
			DomRegion expectedRegion = new DomRegion(bodyRegion.BeginLine + 1, 1, bodyRegion.EndLine, 1);
			DomRegion region = generator.GetBodyRegionInDocument(method);
			
			Assert.AreEqual(expectedRegion, region);
		}		
		
		[Test]
		public void GenerateEventHandlerWithEmptyMethodBody()
		{
			string eventHandler = generator.CreateEventHandler("button1_click", String.Empty, "\t");
			string expectedEventHandler =
				"\tdef button1_click(sender, e)\r\n" +
				"\t\t\r\n" +
				"\tend";
			Assert.AreEqual(expectedEventHandler, eventHandler);
		}
		
		[Test]
		public void GenerateEventHandlerWithNullMethodBody()
		{
			string eventHandler = generator.CreateEventHandler("button2_click", null, String.Empty);
			string expectedEventHandler = 
				"def button2_click(sender, e)\r\n" +
				"\t\r\n" +
				"end";
			Assert.AreEqual(expectedEventHandler, eventHandler);
		}
	}
}
