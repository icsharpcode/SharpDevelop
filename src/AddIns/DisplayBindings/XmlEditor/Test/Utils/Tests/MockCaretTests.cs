// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Utils.Tests
{
	[TestFixture]
	public class MockCaretTests
	{
		MockCaret caret;
		
		[SetUp]
		public void Init()
		{
			caret = new MockCaret();
		}
		
		[Test]
		public void CanSetAndGetCaretOffset()
		{
			caret.Offset = 3;
			Assert.AreEqual(3, caret.Offset);
		}
		
		[Test]
		public void InitialCaretOffsetIsMinusOne()
		{
			Assert.AreEqual(-1, caret.Offset);
		}
		
		[Test]
		public void CanGetAndSetCaretPosition()
		{
			Location location = new Location(3, 2);
			caret.Position = location;
			Assert.AreEqual(location, caret.Position);
		}
		
		[Test]
		public void DefaultCaretPostionIsEmpty()
		{
			Assert.AreEqual(Location.Empty, caret.Position);
		}
	}
}
