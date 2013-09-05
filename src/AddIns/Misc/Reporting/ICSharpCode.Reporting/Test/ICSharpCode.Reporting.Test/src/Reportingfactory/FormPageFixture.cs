/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 19.03.2013
 * Time: 19:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
