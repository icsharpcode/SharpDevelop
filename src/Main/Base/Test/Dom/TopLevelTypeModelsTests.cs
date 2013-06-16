// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using Rhino.Mocks;
namespace ICSharpCode.SharpDevelop.Dom
{
	class TopLevelTypeModelsTests : CSharpModelTestBase
	{
		[Test]
		public void EmptyProject()
		{
			Assert.AreEqual(0, topLevelTypeModels.Count);
			Assert.IsNull(topLevelTypeModels[new TopLevelTypeName("MissingClass")]);
		}
		#region Simple class
		[Test]
		public void AddSimpleClass()
		{
			AddCodeFile("test.cs", @"class SimpleClass {}");
			Assert.AreEqual(1, topLevelTypeModels.Count);
			var simpleClass = topLevelTypeModels.Single();
			Assert.IsEmpty(topLevelChangeEventArgs.Single().OldItems);
			Assert.AreEqual(new[] {
				simpleClass
			}, topLevelChangeEventArgs.Single().NewItems);
			topLevelChangeEventArgs.Clear();
			// clear for follow-up tests
		}
		[Test]
		public void UpdateSimpleClass()
		{
			AddSimpleClass();
			var simpleClass = topLevelTypeModels.Single();
			Assert.AreEqual(0, simpleClass.Members.Count);
			UpdateCodeFile("test.cs", "class SimpleClass { void Method() {} }");
			Assert.AreSame(simpleClass, topLevelTypeModels.Single());
			Assert.AreEqual(1, simpleClass.Members.Count);
			Assert.IsEmpty(topLevelChangeEventArgs);
		}
		[Test]
		public void ReplaceSimpleClass()
		{
			AddSimpleClass();
			var simpleClass = topLevelTypeModels.Single();
			UpdateCodeFile("test.cs", "class OtherClass { }");
			var otherClass = topLevelTypeModels.Single();
			Assert.AreNotSame(simpleClass, otherClass);
			Assert.AreEqual(new[] {
				simpleClass
			}, topLevelChangeEventArgs.Single().OldItems);
			Assert.AreEqual(new[] {
				otherClass
			}, topLevelChangeEventArgs.Single().NewItems);
		}
		[Test]
		public void RemoveSimpleClassCodeFile()
		{
			AddSimpleClass();
			var simpleClass = topLevelTypeModels.Single();
			RemoveCodeFile("test.cs");
			Assert.IsEmpty(topLevelTypeModels);
			// check removal event
			Assert.AreEqual(new[] {
				simpleClass
			}, topLevelChangeEventArgs.Single().OldItems);
			Assert.IsEmpty(topLevelChangeEventArgs.Single().NewItems);
			// test that accessing properties of a removed class still works
			Assert.AreEqual(new FullTypeName("SimpleClass"), simpleClass.FullTypeName);
		}
		#endregion
		#region TwoPartsInSingleFile
		[Test]
		public void TwoPartsInSingleFile()
		{
			AddCodeFile("test.cs", @"
partial class SimpleClass { void Member1() {} }
partial class SimpleClass { void Member2() {} }");
			Assert.AreEqual(1, topLevelTypeModels.Count);
			topLevelChangeEventArgs.Clear();
			// clear for follow-up tests
		}
		[Test]
		public void UpdateTwoPartsInSingleFile()
		{
			TwoPartsInSingleFile();
			var simpleClass = topLevelTypeModels.Single();
			UpdateCodeFile("test.cs", @"
partial class SimpleClass { void Member1b() {} }
partial class SimpleClass { void Member2b() {} }");
			Assert.AreSame(simpleClass, topLevelTypeModels.Single());
			Assert.AreEqual(new[] {
				"Member1b",
				"Member2b"
			}, simpleClass.Members.Select(m => m.Name).ToArray());
		}
		[Test]
		public void RemoveOneOfPartsInSingleFile()
		{
			TwoPartsInSingleFile();
			var simpleClass = topLevelTypeModels.Single();
			var member1 = simpleClass.Members.First();
			UpdateCodeFile("test.cs", "class SimpleClass { void Member1() {} }");
			Assert.AreSame(simpleClass, topLevelTypeModels.Single());
			Assert.AreSame(member1, simpleClass.Members.Single());
		}
		[Test]
		public void RemoveBothPartsInSingleFile()
		{
			TwoPartsInSingleFile();
			var simpleClass = topLevelTypeModels.Single();
			UpdateCodeFile("test.cs", "class OtherClass {}");
			Assert.AreNotSame(simpleClass, topLevelTypeModels.Single());
		}
		#endregion
		#region TestRegionIsFromPrimaryPart
		[Test]
		public void AddingDesignerPartDoesNotChangeRegion()
		{
			AddCodeFile("Form.cs", "partial class MyForm {}");
			Assert.AreEqual("Form.cs", topLevelTypeModels.Single().Region.FileName);
			AddCodeFile("Form.Designer.cs", "partial class MyForm {}");
			Assert.AreEqual("Form.cs", topLevelTypeModels.Single().Region.FileName);
		}
		[Test]
		public void AddingPrimaryPartChangesRegion()
		{
			AddCodeFile("Form.Designer.cs", "partial class MyForm {}");
			Assert.AreEqual("Form.Designer.cs", topLevelTypeModels.Single().Region.FileName);
			AddCodeFile("Form.cs", "partial class MyForm {}");
			Assert.AreEqual("Form.cs", topLevelTypeModels.Single().Region.FileName);
		}
		[Test]
		public void RemovingPrimaryPartChangesRegionToNextBestPart()
		{
			AddCodeFile("Form.cs", "partial class MyForm { int primaryMember; }");
			AddCodeFile("Form.Designer.Designer.cs", "partial class MyForm { int designer2; }");
			var form = topLevelTypeModels.Single();
			Assert.AreEqual(new[] {
				"primaryMember",
				"designer2"
			}, form.Members.Select(m => m.Name).ToArray());
			AddCodeFile("Form.Designer.cs", "partial class MyForm { int designer; }");
			Assert.AreEqual("Form.cs", form.Region.FileName);
			Assert.AreEqual(new[] {
				"primaryMember",
				"designer",
				"designer2"
			}, form.Members.Select(m => m.Name).ToArray());
			RemoveCodeFile("Form.cs");
			Assert.AreSame(form, topLevelTypeModels.Single());
			Assert.AreEqual("Form.Designer.cs", form.Region.FileName);
			Assert.AreEqual(new[] {
				"designer",
				"designer2"
			}, form.Members.Select(m => m.Name).ToArray());
		}
		#endregion
	}
}


