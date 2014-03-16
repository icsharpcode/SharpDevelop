/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 24.02.2014
 * Time: 20:02
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.Xml;
using ICSharpCode.Reporting.Addin.Services;

namespace ICSharpCode.Reporting.Addin.DesignerBinding
{
	/// <summary>
	/// Description of ReportDesignerLoader.
	/// </summary>
	class ReportDesignerLoader: BasicDesignerLoader
	{
		IDesignerLoaderHost host;
		IDesignerGenerator generator;
		ReportModel reportModel;
		Stream stream;
		
		#region Constructors

		public ReportDesignerLoader(IDesignerGenerator generator, Stream stream){
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
		
		public override void BeginLoad(IDesignerLoaderHost host){
			LoggingService.Info("ReportDesignerLoader:BeginLoad"); 
			if (host == null) {
				throw new ArgumentNullException("host");
			}
			this.host = host;
			host.AddService(typeof(ComponentSerializationService), new CodeDomComponentSerializationService((IServiceProvider)host));
			host.AddService(typeof(IDesignerSerializationService), new DesignerSerializationService((IServiceProvider)host));
			base.BeginLoad(host);
		}
		
		
		protected override void PerformLoad(IDesignerSerializationManager serializationManager){
			var internalLoader = new InternalReportLoader(host,generator, stream);
			reportModel = internalLoader.LoadOrCreateReport();
//			reportModel = internalLoader.ReportModel;
		}
 
		protected override void PerformFlush(IDesignerSerializationManager designerSerializationManager){
			LoggingService.Info("ReportDesignerLoader:PerformFlush");
			generator.MergeFormChanges((System.CodeDom.CodeCompileUnit)null);
		}
		
		#endregion

		
		#region Reportmodel
		
		public ReportModel ReportModel {
			get { return reportModel; }
		}
		
		public static ReportModel CreateRenderableModel()
		{
			Console.WriteLine("ReportDesignerLoader:CreateRenderableModel");
			/*
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
			*/
			return null;
		}
		
		public XmlDocument CreateXmlModel()
		{
			Console.WriteLine("ReportDesignerLoader:CreateXmlModel");
			/*
			ReportModel m = new ReportModel();
			generator.MergeFormChanges((System.CodeDom.CodeCompileUnit)null);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(generator.ViewContent.ReportFileContent);
			return xmlDocument;
			*/
			return null;
		}
		
		#endregion
	}

	/// <summary>
	/// Load Report from File
	/// </summary>
	/// <param name="baseType"></param>
	/// <returns></returns>

	public class aaReportLoader : ModelLoader
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
