// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Utils.Tests
{
	[TestFixture]
	public class MockDocumentLineTests
	{
		MockDocumentLine documentLine;
		
		[SetUp]
		public void Init()
		{
			documentLine = new MockDocumentLine();
		}
		
		[Test]
		public void CanSetAndGetLineNumberProperty()
		{
			documentLine.LineNumber = 2;
			Assert.AreEqual(2, documentLine.LineNumber);
		}
	}
}
