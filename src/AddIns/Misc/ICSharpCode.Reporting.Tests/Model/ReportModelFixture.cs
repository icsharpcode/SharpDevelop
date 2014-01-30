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
using ICSharpCode.Reporting.Items;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Tests.Model
{
	[TestFixture]
	public class ReportModelFixture
	{
		private ReportModel model;
		
		[Test]
		public void CanCreateReportModel()
		{
			Assert.That(model,Is.Not.Null);
		}
		
		
		[Test]
		public void ModelInitializeReportSettings()
		{
			Assert.That(model.ReportSettings,Is.Not.Null);
		}
		
		
		[Test]
		public void ModelReturnsPlainReportName()
		{
			Assert.That(model.ReportSettings.ReportName,Is.EqualTo(Globals.GlobalValues.DefaultReportName));
		}
		
		
		[SetUp]
		public void CreateModel()
		{
			model = ReportModel.Create();
		}
			
	}
}
