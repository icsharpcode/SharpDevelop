/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 24.02.2014
 * Time: 20:02
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.Reporting.Factories;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.Addin.Services;

namespace ICSharpCode.Reporting.Addin.DesignerBinding
{
	/// <summary>
	/// Description of ReportDesignerLoader.
	/// </summary>
	public class ReportDesignerLoader: BasicDesignerLoader
	{
		IDesignerLoaderHost host;
		readonly IDesignerGenerator generator;
		Stream stream;
		
		#region Constructors

		public ReportDesignerLoader(IDesignerGenerator generator, Stream stream){
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
			LoggingService.Info("ReportDesignerLoader:PerformLoad"); 
			var internalLoader = new InternalReportLoader(host,generator, stream);
			internalLoader.LoadOrCreateReport();
		}
 
		
		protected override void PerformFlush(IDesignerSerializationManager designerSerializationManager){
			LoggingService.Info("ReportDesignerLoader:PerformFlush");
			generator.MergeFormChanges((System.CodeDom.CodeCompileUnit)null);
		}
		
		#endregion

		
		#region Serialize to Xml
		
		public XmlDocument SerializeModel()
		{
			generator.MergeFormChanges((System.CodeDom.CodeCompileUnit)null);
			var doc = new XmlDocument();
			doc.LoadXml(generator.ViewContent.ReportFileContent);
			return doc;
		}
		
		#endregion
	}
}
