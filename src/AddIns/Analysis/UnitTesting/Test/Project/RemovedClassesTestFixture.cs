// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	[TestFixture]
	public class RemovedClassesTestFixture
	{
		RemovedClasses removedClasses;
		MockClass myClass;
		MockClass anotherClass;
		MockClass innerClass;
		
		[SetUp]
		public void Init()
		{
			myClass = new MockClass("MyTests.MyClass");
			innerClass = new MockClass("MyTests.MyClass.InnerClass", "MyTests.MyClass+InnerClass");
			myClass.InnerClasses.Add(innerClass);
			
			anotherClass = new MockClass("MyTests.AnotherClass");
			
			List<IClass> classes = new List<IClass>();
			classes.Add(myClass);
			classes.Add(anotherClass);
			
			removedClasses = new RemovedClasses();
			removedClasses.Add(classes);
		}
		
		[Test]
		public void InnerClassesIncludedInMissingClasses()
		{
			List<IClass> expectedClasses = new List<IClass>();
			expectedClasses.Add(myClass);
			expectedClasses.Add(innerClass);
			expectedClasses.Add(anotherClass);
			
			AssertContains(expectedClasses, removedClasses.GetMissingClasses());
		}
		
		/// <summary>
		/// Should remove inner class too.
		/// </summary>
		[Test]
		public void RemoveMyClass()
		{
			removedClasses.Remove(myClass);
			
			List<IClass> expectedClasses = new List<IClass>();
			expectedClasses.Add(anotherClass);
			
			AssertContains(expectedClasses, removedClasses.GetMissingClasses());
		}
		
		[Test]
		public void RemoveInnerClass()
		{
			removedClasses.Remove(innerClass);
			
			List<IClass> expectedClasses = new List<IClass>();
			expectedClasses.Add(myClass);
			expectedClasses.Add(anotherClass);
			
			AssertContains(expectedClasses, removedClasses.GetMissingClasses());
		}
		
		[Test]
		public void DotNetNameUsedWhenAddingClasses()
		{
			MockClass c = new MockClass("MyTests.MyClass.InnerClass", "MyTests.MyClass+InnerClass");
			List<IClass> classes = new List<IClass>();
			classes.Add(c);
			
			RemovedClasses removedClasses = new RemovedClasses();
			removedClasses.Add(classes);
			removedClasses.Remove(c);
			
			Assert.AreEqual(0, removedClasses.GetMissingClasses().Count);
		}
		
		void AssertContains(IList<IClass> expectedClasses, IList<IClass> actualClasses)
		{
			foreach (IClass c in expectedClasses) {
				Assert.IsTrue(actualClasses.Contains(c), "Class missing: " + c.FullyQualifiedName + " Actual:\r\n" + GetClassNames(actualClasses));
			}
			Assert.AreEqual(expectedClasses.Count, actualClasses.Count, "Actual:\r\n" + GetClassNames(actualClasses));
		}
		
		string GetClassNames(IList<IClass> classes)
		{
			StringBuilder names = new StringBuilder();
			foreach (IClass c in classes) {
				names.AppendLine(c.FullyQualifiedName);
			}
			return names.ToString();
		}
	}
}
