/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 02.12.2004
 * Time: 22:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Forms;
using System.Xml;

namespace SharpReport.Visitors{
	
	/// <summary>
	/// Description of SaveReportVisitor.
	/// </summary>
	
	public class SaveReportVisitor :SharpReportCore.AbstractModelVisitor
	{
		
		private XmlDocument xmlDoc;
		private SharpReport.Designer.BaseDesignerControl baseDesigner;
		
		public SaveReportVisitor ():base(String.Empty) {
			
		}
		public override void Visit (Control designer) {
			this.baseDesigner = designer as SharpReport.Designer.BaseDesignerControl;
			
			if (this.baseDesigner == null) {
				throw new ArgumentException("designer");
			}
			xmlDoc = this.BuildReportDocument(baseDesigner);
		}
		
		private XmlDocument BuildReportDocument (SharpReport.Designer.BaseDesignerControl designer) {

			XmlDocument xmlDoc = SharpReportCore.XmlHelper.BuildXmlDocument ();

			XmlElement root = xmlDoc.CreateElement (SharpReportCore.GlobalValues.SharpReportString);
			xmlDoc.AppendChild(root);
			
			XmlDocument docSetting = designer.ReportModel.ReportSettings.GetXmlData();
			XmlNode nodeSetting = xmlDoc.ImportNode (docSetting.SelectSingleNode("Sections"),true);
			xmlDoc.DocumentElement.AppendChild (nodeSetting.LastChild);
			
			//Loop over all Sections
			foreach(ReportSection section in designer.SectionsCollection){
				section.Items.SortByLocation();
				XmlDocument dd = section.GetXmlData();
				XmlNode n = xmlDoc.ImportNode (dd.SelectSingleNode("Sections"),true);
				xmlDoc.DocumentElement.AppendChild (n.LastChild);
			}
			return xmlDoc;
		}
		
		public XmlDocument XmlDocument {
			get {
				return xmlDoc;
			}
		}
		
		
	}
	
}
