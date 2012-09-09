// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
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
	public class TestProjectWithOneClassTestFixture : ProjectTestFixtureBase
	{
		TestClass testClass;
		bool resultChangedCalled;
		List<TestClass> classesAdded;
		List<TestClass> classesRemoved;
		
		const string mainFileName = "file1.cs";
		
		[SetUp]
		public void Init()
		{
			resultChangedCalled = false;
			classesAdded = new List<TestClass>();
			classesRemoved = new List<TestClass>();
			
			// Create a project.
			CreateNUnitProject(Parse(@"using NUnit.Framework;
namespace RootNamespace {
	[TestFixture]
	class MyTestFixture {
		
	}
	class NonTestClass { }
}", mainFileName));
			
			testClass = testProject.TestClasses[0];
		}
		
		[Test]
		public void OneTestClass()
		{
			Assert.AreEqual(1, testProject.TestClasses.Count);
		}
		
		[Test]
		public void TestClassName()
		{
			Assert.AreEqual("MyTestFixture", testClass.Name);
		}
		
		[Test]
		public void TestClassQualifiedName()
		{
			Assert.AreEqual("RootNamespace.MyTestFixture", testClass.QualifiedName);
		}
		
		[Test]
		public void ProjectProperty()
		{
			Assert.AreSame(project, testProject.Project);
		}
		
		[Test]
		public void FindTestClass()
		{
			Assert.AreSame(testClass, testProject.GetTestClass("RootNamespace.MyTestFixture"));
		}
		
		[Test]
		public void NoMatchingTestClass()
		{
			Assert.IsNull(testProject.GetTestClass("NoSuchClass.MyTestFixture"));
		}
		
		[Test]
		public void TestClassResultChanged()
		{
			try {
				testClass.TestResultChanged += ResultChanged;
				testClass.TestResult = TestResultType.Success;
			} finally {
				testClass.TestResultChanged -= ResultChanged;
			}
			
			Assert.IsTrue(resultChangedCalled);
		}
		
		/// <summary>
		/// Tests that a new class is added to the TestProject
		/// from the parse info.
		/// </summary>
		[Test]
		public void NewClassInParserInfo()
		{
			UpdateCodeFile(@"using NUnit.Framework;
namespace RootNamespace {
	[TestFixture]
	class MyTestFixture { }
	[TestFixture]
	class MyNewTestFixture { }
}", mainFileName);
			
			Assert.IsNotNull(testProject.GetTestClass("RootNamespace.MyNewTestFixture"));
			Assert.AreEqual(1, classesAdded.Count);
			Assert.AreEqual("RootNamespace.MyNewTestFixture", classesAdded[0].QualifiedName);
		}
		
		/// <summary>
		/// Tests that the TestProject.UpdateParseInfo handles the
		/// case when the old compilation unit does not have a
		/// test class, the new compilation unit does have the
		/// test class, but the test class has already been
		/// added to our TestProject.
		/// </summary>
		[Test]
		public void TestClassInNewCompilationUnitOnly()
		{
			UpdateCodeFile(@"using NUnit.Framework;
namespace RootNamespace {
	[TestFixture]
	class MyTestFixture { }
}", "file2.cs");
			
			Assert.IsNotNull(testProject.GetTestClass("RootNamespace.MyTestFixture"));
			Assert.AreEqual(0, classesAdded.Count);
		}
		
		/// <summary>
		/// New class without a test fixture attribute should not
		/// be added to the list of test classes.
		/// </summary>
		[Test]
		public void NewClassInParserInfoWithoutTestFixtureAttribute()
		{
			UpdateCodeFile(@"using NUnit.Framework;
namespace RootNamespace {
	[TestFixture]
	class MyTestFixture { }
	
	class SecondNonTestClass { }
}", mainFileName);
			
			Assert.IsNull(testProject.GetTestClass("RootNamespace.MyNewTestFixture"));
		}
		
		[Test]
		public void TestClassRemovedInParserInfo()
		{
			UpdateCodeFile(@"using NUnit.Framework;
namespace RootNamespace {
	class NonTestClass { }
}", mainFileName);
			
			Assert.AreEqual(0, testProject.TestClasses.Count);
			Assert.AreEqual(1, classesRemoved.Count);
			Assert.AreSame(testClass, classesRemoved[0]);
		}
		
		[Test]
		public void NewCompilationUnitNull()
		{
			RemoveCodeFile(mainFileName);
			
			Assert.AreEqual(0, testProject.TestClasses.Count);
			Assert.AreEqual(1, classesRemoved.Count);
			Assert.AreSame(testClass, classesRemoved[0]);
		}
		
		/// <summary>
		/// Tests that a new method is added to the TestClass
		/// from the parse info. Also checks that the test method is
		/// taken from the CompoundClass via IClass.GetCompoundClass.
		/// A CompoundClass combines partial classes into one class so
		/// we do not get any duplicate classes with the same name.
		/// </summary>
		[Test]
		public void NewMethodInParserInfo()
		{
			UpdateCodeFile(@"using NUnit.Framework;
namespace RootNamespace {
	[TestFixture]
	class MyTestFixture {
		[Test] public void NewMethod() {}
	}", mainFileName);
			
			Assert.AreEqual("NewMethod", testClass.Members.Single().Name);
		}
		
		void ResultChanged(object source, EventArgs e)
		{
			resultChangedCalled = true;
		}
		
		void testProject_TestClasses_CollectionChanged(object source, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
				classesAdded.AddRange(e.NewItems.Cast<TestClass>());
			else if (e.Action == NotifyCollectionChangedAction.Remove)
				classesRemoved.AddRange(e.OldItems.Cast<TestClass>());
		}
	}
}
