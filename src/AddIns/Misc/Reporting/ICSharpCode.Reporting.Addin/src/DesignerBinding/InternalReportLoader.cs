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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Reporting.Items;
using ICSharpCode.SharpDevelop;
using ICSharpCode.Reporting.Addin.Designer;

namespace ICSharpCode.Reporting.Addin.DesignerBinding
{
	/// <summary>
	/// Description of ReportLoader.
	/// </summary>
	class InternalReportLoader
	{
		readonly IDesignerLoaderHost host;
		readonly Stream stream;
		readonly IDesignerGenerator generator;
		
		public InternalReportLoader(IDesignerLoaderHost host,IDesignerGenerator generator, Stream stream)
		{
			if (host == null) {
				throw new ArgumentNullException("host");
			}
		
			if (generator == null) {
				throw new ArgumentNullException("generator");
			}
			if (stream == null) {
				throw new ArgumentNullException("stream");
			}
			this.host = host;
			this.generator = generator;
			this.stream = stream;
		}
		
		public ReportModel LoadOrCreateReport()
		{
			Application.UseWaitCursor = true;
			var rootComponent = host.CreateComponent(typeof(RootReportModel),"RootReportModel");
			var rootControl = rootComponent as RootReportModel;
			UpdateStatusbar();
			var reportModel = CreateNamedSurface();
			rootControl.Size = reportModel.ReportSettings.PageSize;
			Application.UseWaitCursor = false;
			return reportModel;
		}
		
		
		void UpdateStatusbar ()
		{
			string message;
			if (generator.ViewContent.PrimaryFile.IsDirty) {
				message = String.Format(CultureInfo.CurrentCulture,"Create Report <{0}> ...",Path.GetFileName(generator.ViewContent.PrimaryFile.FileName));
			} else {
				message = String.Format(CultureInfo.CurrentCulture,"Load  Report <{0}> ...",Path.GetFileName(generator.ViewContent.PrimaryFile.FileName));
			}
			SD.StatusBar.SetMessage(message);
		}
		
		
		ReportModel CreateNamedSurface ()
		{
			var deserializer = new ReportDefinitionDeserializer();
			var document = ReportDefinitionDeserializer.LoadXmlFromStream(stream);
			var reportModel = deserializer.CreateModelFromXml(document.DocumentElement,host);
			return reportModel;
		}
	}
}
