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
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeAttribute2Tests : CodeModelTestBase
	{
		CodeAttribute2 codeAttribute;
		
		public override void SetUp()
		{
			base.SetUp();
			AddCodeFile("myAttribute.cs", @"using System;
namespace Test {
	public class MyAttribute : Attribute
	{
		public MyAttribute() {}
		public MyAttribute(object arg) {}
		public MyAttribute(object arg1, object arg2) {}
		
		public object One { get; set; }
		public object Two { get; set; }
	}
}");
		}
		
		void CreateAttribute(string code)
		{
			AddCodeFile("attr.cs", code + " class ClassWithAttribute {}");
			var compilation = projectContent.CreateCompilation();
			var testClass = compilation.FindType(new FullTypeName("ClassWithAttribute")).GetDefinition();
			var attribute = testClass.Attributes.Single();
			codeAttribute = new CodeAttribute2(codeModelContext, attribute);
		}
		
		[Test]
		public void FullName_AttributeWithFullName_ReturnsFullName()
		{
			CreateAttribute("[System.FlagsAttribute]");
			
			string name = codeAttribute.FullName;
			
			Assert.AreEqual("System.FlagsAttribute", name);
		}
		
		[Test]
		public void Name_AttributeWithFullName_ReturnsShortName()
		{
			CreateAttribute("[System.FlagsAttribute]");
			
			string name = codeAttribute.Name;
			
			Assert.AreEqual("Flags", name);
		}
		
		[Test]
		public void Value_AttributeHasOneStringPositionalArgument_ReturnsStringInQuotes()
		{
			CreateAttribute("[Test.MyAttribute(\"StringValue\")]");
			
			string attributeValue = codeAttribute.Value;
			
			Assert.AreEqual("\"StringValue\"", attributeValue);
		}
		
		[Test]
		public void Value_AttributeHasOneBooleanPositionalArgument_ReturnsBooleanValue()
		{
			CreateAttribute("[Test.MyAttribute(true)]");
			
			string attributeValue = codeAttribute.Value;
			
			Assert.AreEqual("true", attributeValue);
		}
		
		[Test]
		public void Value_AttributeHasStringAndBooleanPositionalArgument_ReturnsArgumentCommandSeparated()
		{
			CreateAttribute("[Test.MyAttribute(\"Test\", true)]");
			
			string attributeValue = codeAttribute.Value;
			
			Assert.AreEqual("\"Test\", true", attributeValue);
		}
		
		[Test]
		public void Arguments_AttributeHasOneStringPositionalArgument_ReturnsOneAttributeArgumentWithNoName()
		{
			CreateAttribute("[Test.MyAttribute(\"StringValue\")]");
			
			global::EnvDTE.CodeElements args = codeAttribute.Arguments;
			
			CodeAttributeArgument attributeArg = args.FirstCodeAttributeArgumentOrDefault();
			
			Assert.AreEqual(1, args.Count);
			Assert.AreEqual(String.Empty, attributeArg.Name);
			Assert.AreEqual("\"StringValue\"", attributeArg.Value);
		}
		
		[Test]
		public void Arguments_AttributeHasOneStringNamedArgument_ReturnsOneAttributeArgumentWithName()
		{
			CreateAttribute("[Test.MyAttribute(One = \"StringValue\")]");
			
			global::EnvDTE.CodeElements args = codeAttribute.Arguments;
			
			CodeAttributeArgument attributeArg = args.FirstCodeAttributeArgumentOrDefault();
			
			Assert.AreEqual("One", attributeArg.Name);
			Assert.AreEqual("\"StringValue\"", attributeArg.Value);
		}
		
		[Test]
		public void Arguments_GetArgumentByItemIndexWhenTwoPositionalArguments_ReturnsArgumentAtIndex()
		{
			CreateAttribute("[Test.MyAttribute(\"StringValue\", false)]");
			
			global::EnvDTE.CodeElements args = codeAttribute.Arguments;
			
			CodeAttributeArgument arg = args.Item(2) as CodeAttributeArgument;
			
			Assert.AreEqual("false", arg.Value);
		}
		
		[Test]
		public void Arguments_GetArgumentByItemNameWhenTwoNamedArguments_ReturnsArgument()
		{
			CreateAttribute("[Test.MyAttribute(One = \"OneValue\", Two = false)]");
			
			global::EnvDTE.CodeElements args = codeAttribute.Arguments;
			
			var arg = args.Item("Two") as CodeAttributeArgument;
			
			Assert.AreEqual("false", arg.Value);
		}
		
		[Test]
		public void Name_AttributeIsNotLastPartOfName_ReturnsShortNameContainingAttributePart()
		{
			CreateAttribute(
				"public class TestAttributeColumn : Attribute {}\r\n" +
				"[TestAttributeColumn]");
			
			string name = codeAttribute.Name;
			
			Assert.AreEqual("TestAttributeColumn", name);
		}
		
		[Test]
		public void Kind_AttributeIsDataAnnotationsDisplayColumnAttribute_ReturnsAttribute()
		{
			CreateAttribute("[System.FlagsAttribute]");
			
			global::EnvDTE.vsCMElement kind = codeAttribute.Kind;
			
			Assert.AreEqual(global::EnvDTE.vsCMElement.vsCMElementAttribute, kind);
		}
	}
}
