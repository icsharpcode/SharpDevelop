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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Creates a TestProject that has one test class.
	/// </summary>
	[TestFixture]
	public class TestProjectWithOneClassTestFixture : NUnitTestProjectFixtureBase
	{
		NUnitTestClass testClass;
		List<NUnitTestClass> classesAdded;
		List<NUnitTestClass> classesRemoved;
		
		const string mainFileName = "file1.cs";
		
		public override void SetUp()
		{
			base.SetUp();
			classesAdded = new List<NUnitTestClass>();
			classesRemoved = new List<NUnitTestClass>();
			
			// Create a project.
			AddCodeFile(mainFileName, @"using NUnit.Framework;
namespace RootNamespace {
	[TestFixture]
	class MyTestFixture {
		
	}
	class NonNUnitTestClass { }
}");
			
			testClass = testProject.GetTestClass(new FullTypeName("RootNamespace.MyTestFixture"));
			testProject.NestedTests.CollectionChanged += testProject_TestClasses_CollectionChanged;
		}
		
		[Test]
		public void OneNUnitTestClass()
		{
			Assert.AreEqual(1, testProject.NestedTests.Count);
			Assert.AreSame(testClass, testProject.NestedTests.Single());
		}
		
		[Test]
		public void NUnitTestClassName()
		{
			Assert.AreEqual("MyTestFixture", testClass.ClassName);
		}
		
		[Test]
		public void NUnitTestClassQualifiedName()
		{
			Assert.AreEqual("RootNamespace.MyTestFixture", testClass.ReflectionName);
		}
		
		[Test]
		public void ProjectProperty()
		{
			Assert.AreSame(project, testProject.Project);
		}
		
		[Test]
		public void TestClassIsInProjectNestedTestsCollection()
		{
			Assert.AreSame(testClass, testProject.NestedTests.Single());
		}
		
		[Test]
		public void NoMatchingNUnitTestClass()
		{
			Assert.IsNull(testProject.GetTestClass(new FullTypeName("NoSuchClass.MyTestFixture")));
		}
		
		/// <summary>
		/// Tests that a new class is added to the TestProject
		/// from the parse info.
		/// </summary>
		[Test]
		public void NewClassInParserInfo()
		{
			UpdateCodeFile(mainFileName, @"using NUnit.Framework;
namespace RootNamespace {
	[TestFixture]
	class MyTestFixture { }
	[TestFixture]
	class MyNewTestFixture { }
}");
			
			Assert.IsNotNull(testProject.GetTestClass(new TopLevelTypeName("RootNamespace.MyNewTestFixture")));
			Assert.AreEqual(1, classesAdded.Count);
			Assert.AreEqual("RootNamespace.MyNewTestFixture", classesAdded[0].ReflectionName);
		}
		
		/// <summary>
		/// Tests that the TestProject.UpdateParseInfo handles the
		/// case when the old compilation unit does not have a
		/// test class, the new compilation unit does have the
		/// test class, but the test class has already been
		/// added to our TestProject.
		/// </summary>
		[Test]
		public void NUnitTestClassInNewCompilationUnitOnly()
		{
			AddCodeFile("file2.cs", @"using NUnit.Framework;
namespace RootNamespace {
	[TestFixture]
	class MyTestFixture { }
}");
			
			Assert.IsNotNull(testProject.GetTestClass(new TopLevelTypeName("RootNamespace.MyTestFixture")));
			Assert.AreEqual(0, classesAdded.Count);
		}
		
		/// <summary>
		/// New class without a test fixture attribute should not
		/// be added to the list of test classes.
		/// </summary>
		[Test]
		public void NewClassInParserInfoWithoutTestFixtureAttribute()
		{
			UpdateCodeFile(mainFileName, @"using NUnit.Framework;
namespace RootNamespace {
	[TestFixture]
	class MyTestFixture { }
	
	class SecondNonNUnitTestClass { }
}");
			
			Assert.IsNull(testProject.GetTestClass(new TopLevelTypeName("RootNamespace.MyNewTestFixture")));
		}
		
		[Test]
		public void NUnitTestClassRemovedInParserInfo()
		{
			UpdateCodeFile(mainFileName, @"using NUnit.Framework;
namespace RootNamespace {
	class NonNUnitTestClass { }
}");
			
			Assert.AreEqual(0, testProject.NestedTests.Count);
			Assert.AreEqual(1, classesRemoved.Count);
			Assert.AreSame(testClass, classesRemoved[0]);
		}
		
		[Test]
		public void NewCompilationUnitNull()
		{
			RemoveCodeFile(mainFileName);
			
			Assert.AreEqual(0, testProject.NestedTests.Count);
			Assert.AreEqual(1, classesRemoved.Count);
			Assert.AreSame(testClass, classesRemoved[0]);
		}
		
		/// <summary>
		/// Tests that a new method is added to the NUnitTestClass
		/// from the parse info. Also checks that the test method is
		/// taken from the CompoundClass via IClass.GetCompoundClass.
		/// A CompoundClass combines partial classes into one class so
		/// we do not get any duplicate classes with the same name.
		/// </summary>
		[Test]
		public void NewMethodInParserInfo()
		{
			UpdateCodeFile(mainFileName, @"using NUnit.Framework;
namespace RootNamespace {
	[TestFixture]
	class MyTestFixture {
		[Test] public void NewMethod() {}
	}}");
			
			Assert.AreEqual("NewMethod", testClass.NestedTests.Single().DisplayName);
		}
		
		void testProject_TestClasses_CollectionChanged(IReadOnlyCollection<ITest> removedItems, IReadOnlyCollection<ITest> addedItems)
		{
			classesRemoved.AddRange(removedItems.Cast<NUnitTestClass>());
			classesAdded.AddRange(addedItems.Cast<NUnitTestClass>());
		}
	}
}
