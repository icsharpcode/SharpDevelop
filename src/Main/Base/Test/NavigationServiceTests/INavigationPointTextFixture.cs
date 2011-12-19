// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.SharpDevelop.Tests.NavigationServiceTests
{
	[TestFixture]
	public class INavigationPointTextFixture
	{
		INavigationPoint p1;
		INavigationPoint p2;
		
		[TestFixtureSetUp]
		public void Init()
		{
			int line = 3;
			
			p1 = new TestNavigationPoint("test1.cs", line);
			p2 = new TestNavigationPoint("test2.cs", line+2);
		}

		[Test]
		/// <summary>
		/// A navigation point must store the filename of the file containing
		/// them so that they can switch to the correct file, reopening it
		/// if needed.
		/// </summary>
		public void FileNameTest()
		{
			Assert.AreNotEqual("test1.css", p1.FileName, "FileName differs: expected test1.cs, received "+p1.FileName);
			Assert.AreEqual("test1.cs", p1.FileName);
		}
		
		[Test]
		/// <summary>
		/// A navigation point must remember whatever additional data is needed
		/// to navigation to the appropriate view-position within that file.
		/// </summary>
		public void DataTagTest()
		{
			Assert.AreEqual(3, (int)p1.NavigationData);
		}
		
		[Test]
		/// <summary>
		/// A navigation point must provide a description of itself for the
		/// menu subsystem (ie. for menu text).
		/// </summary>
		public void DescriptionTest()
		{
			Assert.AreNotEqual(String.Empty, p1.Description);
		}
		
		[Test]
		/// <summary>
		/// A navigation point must provide an index for sorting purposes.
		/// </summary>
		public void IndexTest()
		{
			Assert.AreNotEqual(null, p1.Description);
		}
		
		[Test]
		/// <summary>
		/// A navigation point must provide a tooltip for the menu system.
		/// </summary>
		public void ToolTipText()
		{
			Assert.AreNotEqual(String.Empty, p1.ToolTip);
		}
		
		[Test]
		/// <summary>
		/// A navigation point must take responsibility for restoring the
		/// cursor to whatever state is "held" (i.e. "described by") it's data.
		/// </summary>
		public void JumpToTest()
		{
			TestNavigationPoint.CurrentTestPosition = null;
			Assert.AreEqual(TestNavigationPoint.CurrentTestPosition, null);
			
			p1.JumpTo();
			Assert.AreEqual("test1.cs", TestNavigationPoint.CurrentTestPosition.FileName);
			Assert.AreEqual(3, (int)TestNavigationPoint.CurrentTestPosition.NavigationData);
			
			p2.JumpTo();
			Assert.AreEqual("test2.cs", TestNavigationPoint.CurrentTestPosition.FileName);
			
		}
		
		[Test] 
		/// <summary>
		/// A navigation point must take responsibility for evaluating equality tests.
		/// </summary>
		public void EqualityTest()
		{
			Assert.IsTrue(p1.Equals(p1));
			Assert.IsFalse(p1.Equals(p2));
			Assert.IsFalse(p1.Equals("not an INavigationPoint..."));
			
			Assert.AreNotEqual(p1.GetHashCode(), p2.GetHashCode());
		}
	}
}
