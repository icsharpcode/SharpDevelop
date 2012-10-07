// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeAttribute2Tests
	{
		CodeAttribute2 codeAttribute;
		AttributeHelper helper;
		
		void CreateAttribute()
		{
			codeAttribute = new CodeAttribute2(helper.Attribute);
		}
		
		void CreateMSBuildAttribute(string fullName)
		{
			CreateMSBuildAttribute(fullName, fullName);
		}
		
		void CreateMSBuildAttribute(string fullName, string shortName)
		{
			helper = new AttributeHelper();
			helper.CreateAttribute(fullName, shortName);
		}
		
		[Test]
		public void FullName_AttributeIsDataAnnotationsDisplayColumnAttribute_ReturnsDisplayColumnAttributeFullyQualifiedName()
		{
			CreateMSBuildAttribute("System.ComponentModel.DataAnnotations.DisplayColumnAttribute");
			CreateAttribute();
			
			string name = codeAttribute.FullName;
			
			Assert.AreEqual("System.ComponentModel.DataAnnotations.DisplayColumnAttribute", name);
		}
		
		[Test]
		public void Name_AttributeIsDataAnnotationsDisplayColumnAttribute_ReturnsShortDisplayColumnAttributeNameWithoutTheAttributePart()
		{
			CreateMSBuildAttribute("System.ComponentModel.DataAnnotations.DisplayColumnAttribute", "DisplayColumnAttribute");
			CreateAttribute();
			
			string name = codeAttribute.Name;
			
			Assert.AreEqual("DisplayColumn", name);
		}
		
		[Test]
		public void Value_AttributeHasOneStringPositionalArgument_ReturnsStringInQuotes()
		{
			CreateMSBuildAttribute("Test.MyAttribute");
			helper.AddPositionalArguments("StringValue");
			CreateAttribute();
			
			string attributeValue = codeAttribute.Value;
			
			Assert.AreEqual("\"StringValue\"", attributeValue);
		}
		
		[Test]
		public void Value_AttributeHasOneBooleanPositionalArgument_ReturnsBooleanValue()
		{
			CreateMSBuildAttribute("Test.MyAttribute");
			helper.AddPositionalArguments(true);
			CreateAttribute();
			
			string attributeValue = codeAttribute.Value;
			
			Assert.AreEqual("True", attributeValue);
		}
		
		[Test]
		public void Value_AttributeHasStringAndBooleanPositionalArgument_ReturnsArgumentCommandSeparated()
		{
			CreateMSBuildAttribute("Test.MyAttribute");
			helper.AddPositionalArguments("Test", true);
			CreateAttribute();
			
			string attributeValue = codeAttribute.Value;
			
			Assert.AreEqual("\"Test\", True", attributeValue);
		}
		
		[Test]
		public void Arguments_AttributeHasOneStringPositionalArgument_ReturnsOneAttributeArgumentWithNoName()
		{
			CreateMSBuildAttribute("Test.MyAttribute");
			helper.AddPositionalArguments("StringValue");
			CreateAttribute();
			
			global::EnvDTE.CodeElements args = codeAttribute.Arguments;
			
			CodeAttributeArgument attributeArg = args.FirstCodeAttributeArgumentOrDefault();
			
			Assert.AreEqual(1, args.Count);
			Assert.AreEqual(String.Empty, attributeArg.Name);
			Assert.AreEqual("\"StringValue\"", attributeArg.Value);
		}
		
		[Test]
		public void Arguments_AttributeHasOneStringNamedArgument_ReturnsOneAttributeArgumentWithName()
		{
			CreateMSBuildAttribute("Test.MyAttribute");
			helper.AddNamedArgument("Name", "StringValue");
			CreateAttribute();
			
			global::EnvDTE.CodeElements args = codeAttribute.Arguments;
			
			CodeAttributeArgument attributeArg = args.FirstCodeAttributeArgumentOrDefault();
			
			Assert.AreEqual("Name", attributeArg.Name);
			Assert.AreEqual("\"StringValue\"", attributeArg.Value);
		}
		
		[Test]
		public void Arguments_GetArgumentByItemIndexWhenTwoPositionalArguments_ReturnsArgumentAtIndex()
		{
			CreateMSBuildAttribute("Test.MyAttribute");
			helper.AddPositionalArguments("StringValue", false);
			CreateAttribute();
			
			global::EnvDTE.CodeElements args = codeAttribute.Arguments;
			
			CodeAttributeArgument arg = args.Item(2) as CodeAttributeArgument;
			
			Assert.AreEqual("False", arg.Value);
		}
		
		[Test]
		public void Arguments_GetArgumentByItemNameWhenTwoNamedArguments_ReturnsArgument()
		{
			CreateMSBuildAttribute("Test.MyAttribute");
			helper.AddNamedArgument("One", "OneValue");
			helper.AddNamedArgument("Two", false);
			CreateAttribute();
			
			global::EnvDTE.CodeElements args = codeAttribute.Arguments;
			
			CodeAttributeArgument arg = args.Item("Two") as CodeAttributeArgument;
			
			Assert.AreEqual("False", arg.Value);
		}
		
		[Test]
		public void Name_AttributeIsNotLastPartOfName_ReturnsShortNameContainingAttributePart()
		{
			CreateMSBuildAttribute("Tests.TestAttributeColumn", "TestAttributeColumn");
			CreateAttribute();
			
			string name = codeAttribute.Name;
			
			Assert.AreEqual("TestAttributeColumn", name);
		}
		
		[Test]
		public void Kind_AttributeIsDataAnnotationsDisplayColumnAttribute_ReturnsAttribute()
		{
			CreateMSBuildAttribute("System.ComponentModel.DataAnnotations.DisplayColumnAttribute");
			CreateAttribute();
			
			global::EnvDTE.vsCMElement kind = codeAttribute.Kind;
			
			Assert.AreEqual(global::EnvDTE.vsCMElement.vsCMElementAttribute, kind);
		}
	}
}
