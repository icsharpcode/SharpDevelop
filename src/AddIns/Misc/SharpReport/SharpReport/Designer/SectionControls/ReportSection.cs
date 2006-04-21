/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 13.11.2004
 * Time: 22:18
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Globalization;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

using ICSharpCode.Core;

using SharpReportCore;

using SharpReport.ReportItems;
using SharpReport.Designer;

namespace SharpReport{
	/// <summary>
	/// This class builds a Section
	/// </summary>

	public class ReportSection : BaseSection,SharpReportCore.IStoreable{

		const string contextMenuPath = "/SharpReport/ContextMenu/Section";
		private ReportSectionControlBase visualControl;
		
		public event EventHandler <EventArgs> ItemSelected;
		public event EventHandler <EventArgs> Selected;
		
		#region Constructors
		
		internal ReportSection() : base(){
			this.Initialize();
		}
		
		internal ReportSection (string sectionName):base (){
			base.Name = sectionName;
			this.Initialize();
		}

		internal ReportSection(SharpReport.Designer.ReportSectionControlBase visualControl) : base(){
			this.Initialize();
			this.VisualControl = visualControl;
			base.SectionOffset = 0;
			
		}
		#endregion
		
		private void Initialize(){
			base.Items.Added += OnItemAddeded;
			base.Items.Removed += OnRemoveTopLevelItem;
		}
		
		internal ReportSectionControlBase VisualControl{
			get {
				return this.visualControl;
			}
			set { this.visualControl = value;
				this.visualControl.Body.Click += new EventHandler(VisualControlClick);
				this.visualControl.Body.MouseDown += new MouseEventHandler (VisualControlMouseUp);
			}
		}
		
		
		#region XmlStuff for Load and Save reports
		
		private XmlIgnoreAttribute CheckForXmlIgnore(PropertyInfo prop) {
			if (prop != null) {
				AttributeCollection attributes = TypeDescriptor.GetProperties(this)[prop.Name].Attributes;
				XmlIgnoreAttribute attribute = (XmlIgnoreAttribute)attributes[typeof(XmlIgnoreAttribute)];
				return attribute;
			} else {
				return null;
			}
		}
		

		void SerializeItemProperties (XmlElement xmlControl,
		                              BaseReportItem item,PropertyInfo [] prop) {
		                              
			XmlElement xmlProperty;
			XmlAttribute attPropValue;
			
			foreach (PropertyInfo p in prop) {
				AttributeCollection attributes = TypeDescriptor.GetProperties(item)[p.Name].Attributes;
				XmlIgnoreAttribute xmlIgnoreAttribute = (XmlIgnoreAttribute)attributes[typeof(XmlIgnoreAttribute)];
				if (xmlIgnoreAttribute == null){
					
					xmlProperty = xmlControl.OwnerDocument.CreateElement (p.Name);
					if (p.PropertyType == typeof(Font)) {
						XmlFormReader.BuildFontElement (item.Font,xmlProperty);
					}
					else {
						attPropValue = xmlControl.OwnerDocument.CreateAttribute ("value");
						attPropValue.InnerText = Convert.ToString(p.GetValue(item,null),
						                                         CultureInfo.InvariantCulture);
						xmlProperty.Attributes.Append(attPropValue);
					}
					xmlControl.AppendChild(xmlProperty);
				}
			}
			
		}
		
		private XmlElement SerializeControl (XmlDocument doc,BaseReportItem item) {
			Type type = item.GetType();
			PropertyInfo [] prop = type.GetProperties();
			XmlElement xmlControl = doc.CreateElement ("control");
			XmlAttribute typeAttr = doc.CreateAttribute ("type");
			typeAttr.InnerText = type.FullName;

			xmlControl.Attributes.Append(typeAttr);
			
			XmlAttribute baseAttr = doc.CreateAttribute ("basetype");
			baseAttr.InnerText = type.BaseType.ToString();
			xmlControl.Attributes.Append(baseAttr);
			SerializeItemProperties (xmlControl,item,prop);
			return xmlControl;
		
		}
		/// <summary>
		/// Read all ReportItenms of Section
		/// </summary>
		/// <param name="doc"></param>
		private XmlElement ReportItemsToXml (XmlDocument doc,ReportItemCollection items) {
			XmlElement xmlControls = doc.CreateElement ("controls");

			foreach (BaseReportItem item in items) {
				XmlElement xmlControl = SerializeControl (doc,item);
				
				IContainerItem iContainer = item as IContainerItem;
				
				if (iContainer != null) {
					xmlControl.AppendChild ( ReportItemsToXml(doc,iContainer.Items));
				}
				xmlControls.AppendChild(xmlControl);
			}
			return xmlControls;
		}
		
		
		private void SerializeSectionProperties (XmlDocument doc,
		                                       XmlElement section,
		                                       PropertyInfo [] prop) {
			
			XmlElement xmlProperty;
			XmlAttribute attPropValue;
			
			foreach (PropertyInfo p in prop) {
				if (this.CheckForXmlIgnore(p) == null ) {
					if (p.CanWrite) {
						xmlProperty = doc.CreateElement (p.Name);
						attPropValue = doc.CreateAttribute ("value");
						attPropValue.InnerText = Convert.ToString(p.GetValue(this,null),
						                                         CultureInfo.InvariantCulture);
						xmlProperty.Attributes.Append(attPropValue);
						section.AppendChild(xmlProperty);
					}
				}
			}
		}
		
		private XmlElement SectionItemToXml (XmlDocument doc) {
			XmlElement section = doc.CreateElement ("section");
			
			Type type = this.GetType();
			PropertyInfo [] prop = type.GetProperties();
			
			XmlAttribute att = section.OwnerDocument.CreateAttribute ("name");
			
			att.InnerText = this.VisualControl.GetType().Name;
			section.Attributes.Append(att);
			this.SerializeSectionProperties (doc,section,prop);
			
			return section;
		}
		
		#endregion
		
		
		
		#region iStoreable Interface
		public  XmlDocument GetXmlData(){
			//Only a temp DocumentObject
			XmlDocument xmlDocument = SharpReportCore.XmlHelper.BuildXmlDocument ();
			XmlElement xmlRoot = xmlDocument.CreateElement ("Sections");
			
			xmlDocument.AppendChild(xmlRoot);
			
			XmlElement xmlSection = SectionItemToXml (xmlDocument);
			//Then read all ReportItems of this Section
			
			XmlElement xmlControls = ReportItemsToXml (xmlDocument,base.Items);
			xmlSection.AppendChild(xmlControls);
			
			xmlRoot.AppendChild(xmlSection);
			xmlDocument.AppendChild(xmlRoot);

			return xmlDocument;
		}
		
		#endregion
		
		
		public void OnItemSelect(object sender, EventArgs e){
			if (ItemSelected != null)
				ItemSelected(sender, e);

		}
		
		public void VisualControlClick(object sender, EventArgs e){
			this.OnSelect();
		}
		
		public void ReportItemSelected(object sender, EventArgs e){
			this.OnSelect ();
			this.OnItemSelect(sender, e);
		}
		
		
		public void OnPropertyChanged (object sender ,EventArgs e) {
			if (!base.Suspend) {
				base.NotifyPropertyChanged("Section");
			}
		}
		
		
		void VisualControlMouseUp (object sender,MouseEventArgs e) {
			if (e.Button == MouseButtons.Right) {
				ContextMenuStrip ctMen = MenuService.CreateContextMenu (this,contextMenuPath);
				ctMen.Show (this.VisualControl,new Point (e.X,e.Y));
			}
		}
		
		public void OnSelect(){
			if (Selected != null)
				Selected(this,EventArgs.Empty);
		}
	
		private void OnItemAddeded(object sender, CollectionItemEventArgs<IItemRenderer> e){
			
			SharpReport.Designer.IDesignable iDesignable = e.Item as SharpReport.Designer.IDesignable;
		
			if (iDesignable != null) {
				if (this.VisualControl != null) {
					iDesignable.Selected += new EventHandler <EventArgs>(this.ReportItemSelected);
					this.VisualControl.Body.Controls.Add(iDesignable.VisualControl);
					iDesignable.VisualControl.BringToFront();
					iDesignable.VisualControl.Focus();
					iDesignable.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler (OnPropertyChanged);
				}
			}
		}
		
		private void OnRemoveTopLevelItem(object sender, CollectionItemEventArgs<IItemRenderer> e){
			//We have to Convert to IDesignable to
			//get the VisualControl
		
			SharpReport.Designer.IDesignable iDes = e.Item as SharpReport.Designer.IDesignable;
			if (iDes != null) {
				this.VisualControl.Body.Controls.Remove (iDes.VisualControl);
			}
		}
		
		
		
		public  void Render (ReportSettings settings,
		                     SharpReportCore.ReportPageEventArgs e) {
			base.Render (e);
		}
		
		#region property's
		

		
		public override Color BackColor{
			get {
				if (this.VisualControl != null) {
					return VisualControl.Body.BackColor;
				} else {
					return System.Drawing.Color.White;
				}
			}
			set {
				base.BackColor = value;
				if (this.VisualControl != null){
					VisualControl.Body.BackColor = value;
				}
			}
		}
		
		public override Size Size {
			get {
				if (this.visualControl != null) {
					return visualControl.Body.Size;
				} else {
					return base.Size;
				}
			}
			set {
				base.Size = value;
				if (this.visualControl != null) {
					this.visualControl.Body.Size = value;
				}
			}
		}
		

		
		
		[Browsable(false)]
		[XmlIgnoreAttribute]
		public override bool Visible {
			get {
				return (this.Size.Height > 5);
			}
		}	
		#endregion
	}
}
