/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 19.03.2013
 * Time: 19:14
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Reflection;

using NUnit.Framework;

namespace ICSharpCode.Reporting.Tests.Factory
{
	[TestFixture]
	public class ReportingFactoryFixture
	{
		private const string nS = "ICSharpCode.Reporting.Tests.TestReports.";
		private const string reportName = "PlainModel.srd";
		private Stream stream;
		
		[Test]
		public void CreateFormSheetBuilder()
		{
			var r = new ReportingFactory();
			var x = r.CreatePageBuilder(stream);
			Assert.That(x,Is.Null);
		}
		
		[SetUp]
		public void LoadFromStream()
		{
			System.Reflection.Assembly asm = Assembly.GetExecutingAssembly();
			stream = asm.GetManifestResourceStream(nS + reportName);
		}
	}
}
