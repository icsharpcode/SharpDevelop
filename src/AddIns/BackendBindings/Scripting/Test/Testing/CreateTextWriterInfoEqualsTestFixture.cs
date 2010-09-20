// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;
using ICSharpCode.Scripting;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Testing
{
	[TestFixture]
	public class CreateTextWriterInfoEqualsTestFixture
	{
		[Test]
		public void CreateTextWriterInfosAreEqualWhenFileNameAndEncodingAndAppendAreEqual()
		{
			CreateTextWriterInfo lhs = new CreateTextWriterInfo("test.txt", Encoding.UTF8, true);
			CreateTextWriterInfo rhs = new CreateTextWriterInfo("test.txt", Encoding.UTF8, true);
			Assert.AreEqual(lhs, rhs);
		}
		
		[Test]
		public void CreateTextWriterInfosAreNotEqualWhenFileNamesAreDifferent()
		{
			CreateTextWriterInfo lhs = new CreateTextWriterInfo("test.txt", Encoding.UTF8, true);
			CreateTextWriterInfo rhs = new CreateTextWriterInfo("different-filename.txt", Encoding.UTF8, true);
			Assert.AreNotEqual(lhs, rhs);
		}
		
		[Test]
		public void CreateTextWriterInfosAreNotEqualWhenEncodingsAreDifferent()
		{
			CreateTextWriterInfo lhs = new CreateTextWriterInfo("test.txt", Encoding.UTF8, true);
			CreateTextWriterInfo rhs = new CreateTextWriterInfo("test.txt", Encoding.ASCII, true);
			Assert.AreNotEqual(lhs, rhs);
		}
		
		[Test]
		public void CreateTextWriterInfosAreNotEqualWhenAppendIsDifferent()
		{
			CreateTextWriterInfo lhs = new CreateTextWriterInfo("test.txt", Encoding.UTF8, true);
			CreateTextWriterInfo rhs = new CreateTextWriterInfo("test.txt", Encoding.UTF8, false);
			Assert.AreNotEqual(lhs, rhs);
		}
		
		[Test]
		public void CreateTextWriterInfoEqualsReturnsFalseWhenNullPassedAsParameter()
		{
			CreateTextWriterInfo lhs = new CreateTextWriterInfo("test.txt", Encoding.UTF8, true);
			Assert.IsFalse(lhs.Equals(null));
		}
	}
}
