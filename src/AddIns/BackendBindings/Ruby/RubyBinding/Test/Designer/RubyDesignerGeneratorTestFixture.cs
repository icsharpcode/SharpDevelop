// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
