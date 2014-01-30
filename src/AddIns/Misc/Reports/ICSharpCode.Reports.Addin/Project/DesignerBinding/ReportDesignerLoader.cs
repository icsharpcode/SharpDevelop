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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Xml;

using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;

namespace ICSharpCode.Reports.Addin
{
	
	public class ReportDesignerLoader: BasicDesignerLoader
	{
		private IDesignerLoaderHost host;
		private IDesignerGenerator generator;
		private ReportModel reportModel;
		private Stream stream;
		
		#region Constructors

		public ReportDesignerLoader(IDesignerGenerator generator, Stream stream)		
		{
			Console.WriteLine("ReportDesignerLoader:Ctor");
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (generator == null) {
				throw new ArgumentNullException("generator");
			}
			this.generator = generator;
			this.stream = stream;
		}
		
		#endregion

		
		#region Overriden methods of BasicDesignerLoader
		
		public override void BeginLoad(IDesignerLoaderHost host)
		{
			if (host == null) {
				throw new ArgumentNullException("host");
			}
			this.host = host;
			host.AddService(typeof(ComponentSerializationService), new CodeDomComponentSerializationService((IServiceProvider)host));
			host.AddService(typeof(IDesignerSerializationService), new DesignerSerializationService((IServiceProvider)host));
			base.BeginLoad(host);
		}
		
		
		protected override void PerformLoad(IDesignerSerializationManager serializationManager)
		{
			InternalReportLoader internalLoader = new InternalReportLoader(this.host,generator, stream);
			internalLoader.LoadOrCreateReport();
			this.reportModel = internalLoader.ReportModel;
		}

		
		protected override void OnEndLoad(bool successful, ICollection errors)
		{
			base.OnEndLoad(successful, errors);
		}
		
		
		protected override void PerformFlush(IDesignerSerializationManager designerSerializationManager)
		{
			System.Diagnostics.Trace.WriteLine("ReportDesignerLoader:PerformFlush");
			generator.MergeFormChanges((System.CodeDom.CodeCompileUnit)null);
		}
		
		#endregion

		
		#region Reportmodel
		
		public ReportModel ReportModel {
			get { return reportModel; }
		}
		
		public ReportModel CreateRenderableModel()
		{
			Console.WriteLine("ReportDesignerLoader:CreateRenderableModel");
			ReportModel m = new ReportModel();
			generator.MergeFormChanges((System.CodeDom.CodeCompileUnit)null);
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(generator.ViewContent.ReportFileContent);
			ReportLoader rl = new ReportLoader();
			object root = rl.Load(doc.DocumentElement);
			m = root as ReportModel;
			
			m.ReportSettings.FileName = generator.ViewContent.PrimaryFileName;
			FilePathConverter.AdjustReportName(m);
			return m;
		}
		
		public XmlDocument CreateXmlModel()
		{
			Console.WriteLine("ReportDesignerLoader:CreateXmlModel");
			ReportModel m = new ReportModel();
			generator.MergeFormChanges((System.CodeDom.CodeCompileUnit)null);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(generator.ViewContent.ReportFileContent);
			return xmlDocument;
		}
		
		#endregion
		
		#region Dispose
		
		public override void Dispose()
		{
			// Always remove attached event handlers in Dispose.
			base.Dispose();
		}
		
		#endregion
		
	}

	/// <summary>
	/// Load Report from File
	/// </summary>
	/// <param name="baseType"></param>
	/// <returns></returns>

	public class ReportLoader : BaseItemLoader
	{
		static Dictionary<Type, Type> baseToReport;
		
		public static Type GetReportType(Type baseType)
		{
			Console.WriteLine("ReportLoader:GetReportType");
			if (baseType == null) return null;
			if (baseToReport == null) {
				baseToReport = new Dictionary<Type, Type>();
				foreach (Type t in typeof(BaseSection).Assembly.GetTypes()) {
					
					if (t.BaseType != null && t.BaseType.Name.StartsWith("Base",
					                                                     StringComparison.InvariantCulture)) {
						baseToReport[t.BaseType] = t;
					}
				}
			}
			Type r;
			baseToReport.TryGetValue(baseType, out r);
			return r ?? baseType;
		}
		
		
		protected override Type GetTypeByName(string ns, string name)
		{
			return GetReportType(base.GetTypeByName(ns, name));
		}
	}
}
