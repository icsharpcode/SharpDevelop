/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 13.11.2004
 * Time: 22:18
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing.Printing;
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
		public event SelectedEventHandler ItemSelected;
		public event SelectedEventHandler Selected;
		
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
//			this.visualControl.ItemSelected += new SelectedEventHandler (OnItemSelectfrom);
			base.SectionOffset = 0;
			
		}
		#endregion
		
		private void Initialize(){
//			base.Items.Added += new ItemCollectionEventHandler(OnItemAddeded);
//			base.Items.Removed += new ItemCollectionEventHandler(OnItemRemoveded);
				base.Items.Added += OnItemAddeded;
			base.Items.Removed += OnItemRemoveded;
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
		
		/// <summary>
		/// Read all ReportItenms of Section
		/// </summary>
		/// <param name="doc"></param>
		private void ReportItemsToXml (XmlElement ctrlElement) {
			
			foreach (BaseReportItem it in base.Items) {
				Type type = it.GetType();
				PropertyInfo [] prop = type.GetProperties();
				
				XmlElement ctrl = ctrlElement.OwnerDocument.CreateElement ("control");
				XmlAttribute typeAttr = ctrlElement.OwnerDocument.CreateAttribute ("type");
				typeAttr.InnerText = type.FullName;
				ctrl.Attributes.Append(typeAttr);
				
				XmlAttribute baseAttr = ctrlElement.OwnerDocument.CreateAttribute ("basetype");
				baseAttr.InnerText = type.BaseType.ToString();
				ctrl.Attributes.Append(baseAttr);
				
				XmlElement xmlProperty;
				XmlAttribute attPropValue;
				
				foreach (PropertyInfo p in prop) {
					AttributeCollection attributes = TypeDescriptor.GetProperties(it)[p.Name].Attributes;
					XmlIgnoreAttribute xmlIgnoreAttribute = (XmlIgnoreAttribute)attributes[typeof(XmlIgnoreAttribute)];
					if (xmlIgnoreAttribute == null){
						
						xmlProperty = ctrl.OwnerDocument.CreateElement (p.Name);
						if (p.PropertyType == typeof(Font)) {
							XmlFormReader.BuildFontElement (it.Font,xmlProperty);
						}
						else {
							attPropValue = ctrl.OwnerDocument.CreateAttribute ("value");
							attPropValue.InnerText = Convert.ToString(p.GetValue(it,null));
							xmlProperty.Attributes.Append(attPropValue);
						}
						ctrl.AppendChild(xmlProperty);
					}
				}
				ctrlElement.AppendChild(ctrl);
			}
		}
		
		
		private void SectionItemToXml (XmlElement xmlSection) {
			Type type = this.GetType();
			PropertyInfo [] prop = type.GetProperties();

			XmlAttribute att = xmlSection.OwnerDocument.CreateAttribute ("name");
			
			att.InnerText = this.VisualControl.GetType().Name;
			xmlSection.Attributes.Append(att);
			
			XmlElement xmlProperty;
			XmlAttribute attPropValue;
			
			foreach (PropertyInfo p in prop) {
				
				AttributeCollection attributes = TypeDescriptor.GetProperties(this)[p.Name].Attributes;
				XmlIgnoreAttribute xmlIgnoreAttribute = (XmlIgnoreAttribute)attributes[typeof(XmlIgnoreAttribute)];
				
				if (this.CheckForXmlIgnore(p) == null ) {
					if (p.CanWrite) {
						xmlProperty = xmlSection.OwnerDocument.CreateElement (p.Name);
						attPropValue = xmlSection.OwnerDocument.CreateAttribute ("value");
						attPropValue.InnerText = Convert.ToString(p.GetValue(this,null));
						xmlProperty.Attributes.Append(attPropValue);
						xmlSection.AppendChild(xmlProperty);
					}
				}
			}
		}
		
		#endregion
		
		
		
		#region iStoreable Interface
		public  XmlDocument GetXmlData(){
			//Only a temp DocumentObject
			XmlDocument doc = SharpReportCore.XmlHelper.BuildXmlDocument ();
			XmlElement root = doc.CreateElement ("Sections");
			
			doc.AppendChild(root);
			
			//Read the 'section'
			XmlElement section = doc.CreateElement ("section");
			SectionItemToXml (section);
			//Then read all ReportItems of this Section
			
			XmlElement xmlControls = doc.CreateElement ("controls");
			ReportItemsToXml (xmlControls);
			section.AppendChild(xmlControls);
			//and Append this to RootElement
			root.AppendChild(section);
			doc.AppendChild(root);
			return doc;
		}
		
		#endregion
		

		
		public void OnItemSelect(object sender, EventArgs e){
			
			if (!base.Suspend) {
				if (ItemSelected != null)
					ItemSelected(sender, e);
			}
			
		}
		
		public void VisualControlClick(object sender, EventArgs e){
			if (!base.Suspend) {
				this.OnSelect();
			}
		}
		
		public void ReportItemSelected(object sender, EventArgs e){
			if (!base.Suspend) {
				this.OnSelect ();
				this.OnItemSelect(sender, e);
			}
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
			AddItem(e.Item);
		}
		
		private void OnItemRemoveded(object sender, CollectionItemEventArgs<IItemRenderer> e){
			//We have to Convert to IDesignable to 
			//get the VisualControl
			SharpReport.Designer.IDesignable iDes = e.Item as SharpReport.Designer.IDesignable;
			if (iDes != null) {
				try {
					this.VisualControl.Body.Controls.Remove (iDes.VisualControl);
				} catch (Exception) {
					throw new SystemException("ReportSection:OnItemRemoveded");
				}
			}
		}
		
		private void AddItem(IItemRenderer item){
			SharpReport.Designer.IDesignable iDesignable = item as SharpReport.Designer.IDesignable;
			if (iDesignable != null) {
				if (this.VisualControl != null) {
					iDesignable.Selected += new SelectedEventHandler(this.ReportItemSelected);
					this.VisualControl.Body.Controls.Add(iDesignable.VisualControl);
					iDesignable.VisualControl.BringToFront();
					iDesignable.VisualControl.Focus();
					iDesignable.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler (OnPropertyChanged);
				}
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
