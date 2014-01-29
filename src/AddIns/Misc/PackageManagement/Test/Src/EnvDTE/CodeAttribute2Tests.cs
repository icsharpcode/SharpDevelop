// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		public MyAttribute(object arg) {}
		public MyAttribute(object arg1, object arg2) {}
		
		public object One, Two;
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
		[Ignore("Why is true expected to be uppercase?")]
		public void Value_AttributeHasOneBooleanPositionalArgument_ReturnsBooleanValue()
		{
			CreateAttribute("[Test.MyAttribute(true)]");
			
			string attributeValue = codeAttribute.Value;
			
			Assert.AreEqual("True", attributeValue);
		}
		
		[Test]
		[Ignore("Why is true expected to be uppercase?")]
		public void Value_AttributeHasStringAndBooleanPositionalArgument_ReturnsArgumentCommandSeparated()
		{
			CreateAttribute("[Test.MyAttribute(\"Test\", true)]");
			
			string attributeValue = codeAttribute.Value;
			
			Assert.AreEqual("\"Test\", true", attributeValue);
		}
		
//		[Test]
//		public void Arguments_AttributeHasOneStringPositionalArgument_ReturnsOneAttributeArgumentWithNoName()
//		{
//			CreateMSBuildAttribute("Test.MyAttribute");
//			helper.AddPositionalArguments("StringValue");
//			CreateAttribute();
//			
//			global::EnvDTE.CodeElements args = codeAttribute.Arguments;
//			
//			CodeAttributeArgument attributeArg = args.FirstCodeAttributeArgumentOrDefault();
//			
//			Assert.AreEqual(1, args.Count);
//			Assert.AreEqual(String.Empty, attributeArg.Name);
//			Assert.AreEqual("\"StringValue\"", attributeArg.Value);
//		}
//		
//		[Test]
//		public void Arguments_AttributeHasOneStringNamedArgument_ReturnsOneAttributeArgumentWithName()
//		{
//			CreateMSBuildAttribute("Test.MyAttribute");
//			helper.AddNamedArgument("Name", "StringValue");
//			CreateAttribute();
//			
//			global::EnvDTE.CodeElements args = codeAttribute.Arguments;
//			
//			CodeAttributeArgument attributeArg = args.FirstCodeAttributeArgumentOrDefault();
//			
//			Assert.AreEqual("Name", attributeArg.Name);
//			Assert.AreEqual("\"StringValue\"", attributeArg.Value);
//		}
//		
//		[Test]
//		public void Arguments_GetArgumentByItemIndexWhenTwoPositionalArguments_ReturnsArgumentAtIndex()
//		{
//			CreateMSBuildAttribute("Test.MyAttribute");
//			helper.AddPositionalArguments("StringValue", false);
//			CreateAttribute();
//			
//			global::EnvDTE.CodeElements args = codeAttribute.Arguments;
//			
//			CodeAttributeArgument arg = args.Item(2) as CodeAttributeArgument;
//			
//			Assert.AreEqual("False", arg.Value);
//		}
//		
//		[Test]
//		public void Arguments_GetArgumentByItemNameWhenTwoNamedArguments_ReturnsArgument()
//		{
//			CreateMSBuildAttribute("Test.MyAttribute");
//			helper.AddNamedArgument("One", "OneValue");
//			helper.AddNamedArgument("Two", false);
//			CreateAttribute();
//			
//			global::EnvDTE.CodeElements args = codeAttribute.Arguments;
//			
//			CodeAttributeArgument arg = args.Item("Two") as CodeAttributeArgument;
//			
//			Assert.AreEqual("False", arg.Value);
//		}
//		
//		[Test]
//		public void Name_AttributeIsNotLastPartOfName_ReturnsShortNameContainingAttributePart()
//		{
//			CreateMSBuildAttribute("Tests.TestAttributeColumn", "TestAttributeColumn");
//			CreateAttribute();
//			
//			string name = codeAttribute.Name;
//			
//			Assert.AreEqual("TestAttributeColumn", name);
//		}
//		
//		[Test]
//		public void Kind_AttributeIsDataAnnotationsDisplayColumnAttribute_ReturnsAttribute()
//		{
//			CreateMSBuildAttribute("System.ComponentModel.DataAnnotations.DisplayColumnAttribute");
//			CreateAttribute();
//			
//			global::EnvDTE.vsCMElement kind = codeAttribute.Kind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMElement.vsCMElementAttribute, kind);
//		}
	}
}
