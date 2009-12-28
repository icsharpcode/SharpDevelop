// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.IO;

using ICSharpCode.RubyBinding;
using ICSharpCode.FormsDesigner;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class RubyDesignerGeneratorTestFixture
	{		
		[Test]
		public void GetMethodReplaceRegion()
		{
			MockMethod method = new MockMethod();
			DomRegion bodyRegion = new DomRegion(0, 4, 1, 4);
			method.BodyRegion = bodyRegion;
			DomRegion expectedRegion = new DomRegion(bodyRegion.BeginLine + 1, 1, bodyRegion.EndLine, 1);
			DerivedRubyDesignerGenerator generator = new DerivedRubyDesignerGenerator();
			
			Assert.AreEqual(expectedRegion, RubyDesignerGenerator.GetBodyRegionInDocument(method));
		}		
		
		[Test]
		public void GenerateEventHandlerWithEmptyMethodBody()
		{
			DerivedRubyDesignerGenerator generator = new DerivedRubyDesignerGenerator();
			string eventHandler = generator.CallCreateEventHandler("button1_click", String.Empty, "\t");
			string expectedEventHandler = "\tdef button1_click(sender, e)\r\n" +
										"\t\t\r\n" +
										"\tend";
			Assert.AreEqual(expectedEventHandler, eventHandler);
		}
		
		[Test]
		public void GenerateEventHandlerWithNullMethodBody()
		{
			DerivedRubyDesignerGenerator generator = new DerivedRubyDesignerGenerator();
			string eventHandler = generator.CallCreateEventHandler("button2_click", null, String.Empty);
			string expectedEventHandler = "def button2_click(sender, e)\r\n" +
											"\t\r\n" +
											"end";
			Assert.AreEqual(expectedEventHandler, eventHandler);
		}
	}
}
