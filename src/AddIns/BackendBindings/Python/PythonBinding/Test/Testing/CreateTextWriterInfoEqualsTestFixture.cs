// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Testing
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
