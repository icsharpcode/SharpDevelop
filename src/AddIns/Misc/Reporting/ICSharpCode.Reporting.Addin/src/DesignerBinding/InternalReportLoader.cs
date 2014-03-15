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
			Console.WriteLine("---------InternalReportLoader------------");
			this.host = host;
			this.generator = generator;
			this.stream = stream;
		}
		
		public void LoadOrCreateReport()
		{
			Console.WriteLine("LoadOrCreateReport()");
			Application.UseWaitCursor = true;
			try {
				var component = this.host.CreateComponent(typeof(RootReportModel),"RootReportModel");
				var rootControl = component as RootReportModel;
				UpdateStatusbar();
				ReportModel = CreateNamedSurface();
				rootControl.Size = this.ReportModel.ReportSettings.PageSize;
				
			} catch (Exception e) {
				MessageService.ShowException(e,"LoadOrCreateReport");
			} finally {
				Application.UseWaitCursor = false;
			}
		}
		
		
		private void UpdateStatusbar ()
		{
			string message;
			if (this.generator.ViewContent.PrimaryFile.IsDirty) {
				message = String.Format("Create Report <{0}> ...",Path.GetFileName(this.generator.ViewContent.PrimaryFile.FileName));
			} else {
				message = String.Format("Load  Report <{0}> ...",Path.GetFileName(this.generator.ViewContent.PrimaryFile.FileName));
			}
			SD.StatusBar.SetMessage(message);
		}
		
		
		ReportModel CreateNamedSurface ()
		{
			var deserializer = new ReportDefinitionDeserializer(host,stream);
			return deserializer.LoadObjectFromFileDefinition();
		}
		
		
		public ReportModel ReportModel {get; private set;}

	}
}
