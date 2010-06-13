// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Text;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Testing
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
