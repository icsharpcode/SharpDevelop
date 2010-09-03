// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;

using ICSharpCode.Scripting;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Testing
{
	[TestFixture]
	public class CreateTextWriterFromCreateTextWriterInfoTestFixture
	{
		TextWriter textWriter;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string fileName = Path.GetTempFileName();
			CreateTextWriterInfo info = new CreateTextWriterInfo(fileName, Encoding.UTF8, false);
			textWriter = info.CreateTextWriter();
		}
		
		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			textWriter.Dispose();
		}
		
		[Test]
		public void CreatedTextWriterEncodingIsUtf8()
		{
			Assert.AreEqual(Encoding.UTF8, textWriter.Encoding);
		}
	}
}
