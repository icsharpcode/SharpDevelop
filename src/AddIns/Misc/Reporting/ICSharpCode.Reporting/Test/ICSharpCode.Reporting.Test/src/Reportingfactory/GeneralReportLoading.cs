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
using ICSharpCode.Reporting.Globals;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.Reportingfactory
{
	[TestFixture]
	public class GeneralReportLoading
	{
		private Stream stream;
		
		[Test]
		public void CanLoadFromResource()
		{
			Assert.IsNotNull(stream);
		}
		
		
		[Test]
		public void LoadPlainModel()
		{
			var rf = new ReportingFactory();
			var model = rf.LoadReportModel(stream);
			Assert.IsNotNull(model);
		}
		
		
		[Test]
		public void ReportSettingsFromPlainModel()
		{
			var rf = new ReportingFactory();
			var model = rf.LoadReportModel(stream);
			Assert.That(model.ReportSettings,Is.Not.Null);
		}
		
		
		[Test]
		public void ReportSettingsReportName()
		{
			var rf = new ReportingFactory();
			var model = rf.LoadReportModel(stream);
			Assert.That(model.ReportSettings.ReportName,Is.EqualTo(GlobalValues.DefaultReportName));
		}
		
		
		[Test]
		public void ReportSettingsDataModelFormSheet()
		{
			var rf = new ReportingFactory();
			var model = rf.LoadReportModel(stream);
			Assert.That(model.ReportSettings.DataModel,Is.EqualTo(GlobalEnums.PushPullModel.FormSheet));
		}
		
		
		[Test]
		public void ReportSettingsPageSize()
		{
			var rf = new ReportingFactory();
			var model = rf.LoadReportModel(stream);
			Assert.That(model.ReportSettings.PageSize,Is.EqualTo(Globals.GlobalValues.DefaultPageSize));
		}
		
		
		
		[SetUp]
		public void LoadFromStream()
		{
			var asm = Assembly.GetExecutingAssembly();
			stream = asm.GetManifestResourceStream(TestHelper.PlainReportFileName);
		}	
	}
}
