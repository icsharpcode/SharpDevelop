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
using System.IO;
using System.Reflection;

using ICSharpCode.Reporting.PageBuilder;
using NUnit.Framework;
using ICSharpCode.Reporting.Test;

namespace ICSharpCode.Reporting.Test.Reportingfactory
{
	[TestFixture]
	public class FormSheetFixture
	{
		Stream stream;
		
		
		[Test]
		public void CanCreateReportCreatorFromFormSheet () {
			var reportingFactory  = new ReportingFactory();
			var rc = reportingFactory.ReportCreator(stream);
			Assert.That(rc,Is.Not.Null);
			Assert.That(rc,Is.TypeOf(typeof(FormPageBuilder)));
		}
		

	
		[SetUp]
		public void LoadFromStream()
		{
			var asm = Assembly.GetExecutingAssembly();
			stream = asm.GetManifestResourceStream(TestHelper.PlainReportFileName);
		}	
	}
}
