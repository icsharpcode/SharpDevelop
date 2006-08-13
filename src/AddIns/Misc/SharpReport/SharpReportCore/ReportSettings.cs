//
// SharpDevelop ReportEditor
//
// Copyright (C) 2005 Peter Forstmeier
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Peter Forstmeier (Peter.Forstmeier@t-online.de)

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace SharpReportCore{
	/// <summary>
	/// This class stores all the basic settings of an Report
	/// </summary>
	/// 

	public class ReportSettings : BaseSettings,SharpReportCore.IStoreable,
								  IBaseRenderer,IDisposable{
		
		private string connectionString;
		private string commandText;
		
		private System.Data.CommandType commandType;
		
		private Font defaultFont = new Font("Microsoft Sans Serif",
		                                    10,
		                                    FontStyle.Regular,
		                                    GraphicsUnit.Point);
		
		
		private GlobalEnums.ReportTypeEnum reportType;
		private GlobalEnums.PushPullModelEnum dataModel;
		
		private SqlParametersCollection reportParametersCollection;
		private ColumnCollection availableFields;
		private ColumnCollection groupingsCollection;
		private ColumnCollection sortingCollection;
		
		
		
		#region Constructor's
		
		public ReportSettings(System.Drawing.Printing.PageSettings defaultPageSettings)
			:this(defaultPageSettings,"",""){
		}
		
		
		public ReportSettings(System.Drawing.Printing.PageSettings pageSettings,
		                      string reportName,
		                      string fileName):base(pageSettings,reportName,fileName){
			
			BaseValues();
		}
		#endregion
		
		#region privates
		
		void BaseValues() {
			connectionString  = String.Empty;
			this.availableFields = new ColumnCollection();
			sortingCollection = new ColumnCollection();
			groupingsCollection = new ColumnCollection();
			reportParametersCollection = new SqlParametersCollection();
			this.reportType = GlobalEnums.ReportTypeEnum.FormSheet;
			this.dataModel = GlobalEnums.PushPullModelEnum.FormSheet;
		}
		

		/// <summary>
		/// Set the values for all Columns that inherit
		/// from <see cref="AbstractColumn"></see> like for sorting etc
		/// </summary>
		/// <param name="reader">See XMLFormReader</param>
		/// <param name="item">AbstractColumn</param>
		/// <param name="ctrlElem">Element witch contains the values</param>
		private  void BuildAbstractColumn (XmlFormReader reader,
		                                   XmlElement ctrlElem,
		                                   AbstractColumn item) {
			
			
			XmlNodeList nodeList = ctrlElem.ChildNodes;
			foreach (XmlNode node in nodeList) {
				XmlElement elem = node as XmlElement;
				if (elem != null) {
					if (elem.HasAttribute("value")) {
						reader.SetValue (item,elem.Name,elem.GetAttribute("value"));
					}
				}
			}
		}
		
		/// <summary>
		/// This Class fills an Reportparameter
		/// </summary>
		/// <param name="reader">See XMLFormReader</param>
		/// <param name="parElement">XmlElement ReportParameter</param>
		/// <param name="item"><see cref="ReportParameter"</param>
		private static void BuildReportParameter(XmlFormReader reader,
		                                  XmlElement parElement,
		                                  SharpReportCore.AbstractParameter item) {
			
			XmlNodeList nodeList = parElement.ChildNodes;
			foreach (XmlNode node in nodeList) {
				XmlElement elem = node as XmlElement;
				if (elem != null) {
					if (elem.HasAttribute("value")) {
						reader.SetValue ((SqlParameter)item,elem.Name,elem.GetAttribute("value"));
					}
				}
			}
		}
		
		#endregion
		
		#region RestoreItems
		
		private void CheckForCollection ( XmlFormReader xmlReader,XmlElement xmlCol) {
			
			if (xmlCol.ChildNodes.Count == 0) {
				return;
			}
			
			switch ((GlobalEnums.enmParamCollectionName)GlobalEnums.StringToEnum(typeof(GlobalEnums.enmParamCollectionName),xmlCol.Name)) {
					
					case GlobalEnums.enmParamCollectionName.AvailableColumns: {
						XmlNodeList nodeList = xmlCol.ChildNodes;
						this.availableFields.Clear();
						foreach (XmlNode node in nodeList) {
							XmlElement elem = node as XmlElement;
							if (elem != null) {
								AbstractColumn abstr = new AbstractColumn();
								BuildAbstractColumn (xmlReader,elem,abstr);
								this.availableFields.Add(abstr);
							}
						}
						break;
					}
					
					case GlobalEnums.enmParamCollectionName.Sortings:{
						
						XmlNodeList nodeList = xmlCol.ChildNodes;
						this.sortingCollection.Clear();
						foreach (XmlNode node in nodeList) {
							XmlElement elem = node as XmlElement;
							if (elem != null) {
								SortColumn sc = new SortColumn();
								BuildAbstractColumn (xmlReader,elem,sc);
								sortingCollection.Add(sc);
							}
						}
						break;
					}
					
					case GlobalEnums.enmParamCollectionName.Groupings:{
						XmlNodeList nodeList = xmlCol.ChildNodes;
						this.groupingsCollection.Clear();
						foreach (XmlNode node in nodeList) {
							XmlElement elem = node as XmlElement;
							if (elem != null) {
								GroupColumn gc = new GroupColumn();
								BuildAbstractColumn (xmlReader,elem,gc);
								groupingsCollection.Add(gc);
							}
						}
						break;
					}
					
					case 	GlobalEnums.enmParamCollectionName.SqlParams:{
						XmlNodeList nodeList = xmlCol.ChildNodes;
						this.reportParametersCollection.Clear();
						foreach( XmlNode node in nodeList) {
							XmlElement elem = node as XmlElement;
							if (elem != null) {
								SqlParameter parameter = new SqlParameter();
								ReportSettings.BuildReportParameter (xmlReader,
								                      elem,
								                      parameter);
								reportParametersCollection.Add(parameter);
							}
						}
						break;
					}					
			}
		}
		
		
		public void SetSettings(XmlElement xmlSettings) {
			if (xmlSettings == null) {
				throw new ArgumentNullException("xmlSettings");
			}
			XmlNodeList nodeList = xmlSettings.ChildNodes;
			XmlFormReader xmlFormReader = new XmlFormReader();
			base.InitDone = false;
			foreach (XmlNode node in nodeList) {
				XmlElement elem = node as XmlElement;
				if (elem != null) {
					CheckForCollection (xmlFormReader,elem);
					
					if (elem.Name == "PageSettings") {
						base.PageSettings = (PageSettings)XmlFormReader.StringToTypedValue(elem.GetAttribute("value"),
						                                                                   typeof(PageSettings),
						                                                                   CultureInfo.InvariantCulture);
						
					}

					else if (elem.HasAttribute("value")) {
						xmlFormReader.SetValue (this,elem.Name,elem.GetAttribute("value"));
					}
				}
			}
//			base.InitDone = true;
		}
		
		#endregion
		
		#region SharpReport.DelegatesInterfaces.IStoreable interface implementation

		private void SectionItemToXml (XmlElement xmlSection) {
			Type type = this.GetType();
			
			PropertyInfo [] prop = type.GetProperties();
			XmlAttribute att = xmlSection.OwnerDocument.CreateAttribute ("name");
			att.InnerText = type.FullName;
			xmlSection.Attributes.Append(att);
			
			XmlElement xmlProperty;

			foreach (PropertyInfo p in prop) {
				
				AttributeCollection attributes = TypeDescriptor.GetProperties(this)[p.Name].Attributes;
				XmlIgnoreAttribute xmlIgnoreAttribute = (XmlIgnoreAttribute)attributes[typeof(XmlIgnoreAttribute)];
				DefaultValueAttribute defaultValue = (DefaultValueAttribute)attributes[typeof(DefaultValueAttribute)];
				
				//CHeck for
				//	[XmlAttribute("titleFont")]
				//public string TestName {
				
//				XmlAttributeAttribute test = (XmlAttributeAttribute)attributes[typeof(XmlAttributeAttribute)];
//				if (test != null) {
//					
//					System.Windows.Forms.MessageBox.Show (test.AttributeName   + "found");
//				}
				
				
				if (xmlIgnoreAttribute == null){
					if (p.CanWrite) {
						if (defaultValue == null) {

							xmlProperty = SaveItem (xmlSection,p);
							xmlSection.AppendChild(xmlProperty);
						} else {
							if (defaultValue.Value.Equals(p.GetValue(this,null)) == false) {

								xmlProperty = SaveItem (xmlSection,p);
								xmlSection.AppendChild(xmlProperty);
							}
						}
					}
				}
			}
//			System.Windows.Forms.MessageBox.Show("fertig");
		}
		
		
		private XmlElement SaveItem (XmlElement section,PropertyInfo prop) {
			XmlElement xmlProperty;
			XmlAttribute attPropValue;
			
			
			xmlProperty = section.OwnerDocument.CreateElement (prop.Name);
			if (prop.PropertyType == typeof(Font)) {
				XmlFormReader.BuildFontElement (this.DefaultFont,xmlProperty);
				
			}else if (prop.PropertyType == typeof(Margins)) {
				XmlAttribute a = xmlProperty.OwnerDocument.CreateAttribute ("value");
				string str = XmlFormReader.TypedValueToString (this.DefaultMargins,
				                                               CultureInfo.InvariantCulture);
				a.InnerText = str ;
				xmlProperty.Attributes.Append(a);
			}
			else {
				attPropValue = section.OwnerDocument.CreateAttribute ("value");
				attPropValue.InnerText = Convert.ToString(prop.GetValue(this,null));
				
				xmlProperty.Attributes.Append(attPropValue);
			}
			return xmlProperty;
		}
		
		
		private static void SaveCollectionItems (XmlElement xmlSaveTo,AbstractColumn column,PropertyInfo [] prop) {
			XmlElement xmlProperty = null;
			XmlAttribute attPropValue;
			foreach (PropertyInfo p in prop) {
				if (p.CanWrite) {
					xmlProperty = xmlSaveTo.OwnerDocument.CreateElement (p.Name);
					attPropValue = xmlSaveTo.OwnerDocument.CreateAttribute ("value");
					attPropValue.InnerText = Convert.ToString(p.GetValue(column,null));
					xmlProperty.Attributes.Append(attPropValue);
					xmlSaveTo.AppendChild(xmlProperty);
				}
			}
		}
		private void SqlParamsToXml (XmlElement xmlParam) {
			XmlElement xmlElem = null;
			try {
				foreach (SqlParameter rPar in this.reportParametersCollection) {
					Type type = rPar.GetType();
					PropertyInfo [] prop = type.GetProperties();
					xmlElem = xmlParam.OwnerDocument.CreateElement ("params");
					XmlElement xmlProperty = null;
					XmlAttribute attPropValue;
					foreach (PropertyInfo p in prop) {
						if (p.CanWrite) {
							xmlProperty = xmlParam.OwnerDocument.CreateElement(p.Name);
							attPropValue = xmlParam.OwnerDocument.CreateAttribute ("value");
							attPropValue.InnerText = Convert.ToString(p.GetValue(rPar,null));
							xmlProperty.Attributes.Append(attPropValue);
							xmlElem.AppendChild(xmlProperty);
						}
					}
					xmlParam.AppendChild(xmlElem);
				}
			} catch (Exception) {
				throw;
			}
		}
		
		private void SortColumnsToXml(XmlElement xmlSection) {
			try {
				foreach (AbstractColumn column in this.sortingCollection) {
					Type type = column.GetType();
					PropertyInfo [] prop = type.GetProperties();
					XmlElement ctrl = xmlSection.OwnerDocument.CreateElement ("sorting");
					ReportSettings.SaveCollectionItems(ctrl,column,prop);
					xmlSection.AppendChild(ctrl);
				}
			} catch (Exception) {
				throw;
			}
		}
		
		private void GroupColumnsToXml (XmlElement xmlSection) {
			try {
				foreach (AbstractColumn column in this.groupingsCollection) {
					Type type = column.GetType();
					PropertyInfo [] prop = type.GetProperties();
					XmlElement ctrl = xmlSection.OwnerDocument.CreateElement ("grouping");
					ReportSettings.SaveCollectionItems(ctrl,column,prop);
					xmlSection.AppendChild(ctrl);
				}
			} catch (Exception) {
				throw;
			}
		}
		
		private void AvailableFieldsToXml (XmlElement xmlSection) {
			try {
				foreach (AbstractColumn column in this.availableFields) {
					Type type = column.GetType();
					PropertyInfo [] prop = type.GetProperties();
					XmlElement ctrl = xmlSection.OwnerDocument.CreateElement ("column");
					ReportSettings.SaveCollectionItems(ctrl,column,prop);
					xmlSection.AppendChild(ctrl);
				}
			} catch (Exception) {
				throw;
			}
		}
		
		public XmlDocument GetXmlData(){
			XmlDocument doc = new XmlDocument();
			XmlDeclaration dec =  doc.CreateXmlDeclaration("1.0",null, "yes");
			doc.PrependChild ( dec );
			XmlElement root = doc.CreateElement ("Sections");
			doc.AppendChild(root);
			
			XmlElement section = doc.CreateElement ("section");
			if (this.availableFields.Count > 0) {
				XmlElement xmlAvailableFields = doc.CreateElement(GlobalEnums.enmParamCollectionName.AvailableColumns.ToString());
				AvailableFieldsToXml (xmlAvailableFields);
				section.AppendChild(xmlAvailableFields);
			}
			if (this.sortingCollection.Count > 0) {
				XmlElement xmlSortColumns = doc.CreateElement (GlobalEnums.enmParamCollectionName.Sortings.ToString());
				SortColumnsToXml (xmlSortColumns);
				section.AppendChild(xmlSortColumns);
			}
			
			if (this.groupingsCollection.Count > 0){
				XmlElement xmlGroupColumns = doc.CreateElement (GlobalEnums.enmParamCollectionName.Groupings.ToString());
				GroupColumnsToXml(xmlGroupColumns);
				section.AppendChild(xmlGroupColumns);
			}

			if (reportParametersCollection.Count > 0) {
				XmlElement xmlSqlParams = doc.CreateElement (GlobalEnums.enmParamCollectionName.SqlParams.ToString());
				SqlParamsToXml(xmlSqlParams);
				section.AppendChild(xmlSqlParams);
			}
			SectionItemToXml (section);
			root.AppendChild(section);
			doc.AppendChild(root);
			return doc;
		}
		
		#endregion
		
		#region SharpReport.DelegatesInterfaces.IRender interface implementation
		public void Render(ReportPageEventArgs rpea) {
			/*
			Font headFont = new Font("Courier New", 20,FontStyle.Bold);
			Font printFont = this.DefaultFont;
			Brush blackBrush = new  SolidBrush(Color.Black);
			
			float i = startAt;
			
			rpea.PrintPageEventArgs.Graphics.DrawString(this.ToString(),
			                                            headFont,
			                                            blackBrush,
			                                            this.DefaultMargins.Left,i);
			
			i += headFont.GetHeight() + 4F;
			
			SizeF size;
			Rectangle rect;
			Type type = this.GetType();
			PropertyInfo [] prop = type.GetProperties();
			foreach (PropertyInfo p in prop) {
				
				try {
					string s = Convert.ToString(p.GetValue(this,null));
					PointF point = new PointF (rpea.PrintPageEventArgs.MarginBounds.Left,i);
					rpea.PrintPageEventArgs.Graphics.DrawString(p.Name + " :",
					                                            printFont,
					                                            Brushes.Black,
					                                            point);
					
					size = rpea.PrintPageEventArgs.Graphics.MeasureString (s,
					                                                       printFont,
					                                                       rpea.PrintPageEventArgs.PageSettings.PaperSize.Width /2);
					
					if (s.Length != 0) {
						rect = new Rectangle(rpea.PrintPageEventArgs.PageSettings.PaperSize.Width /2,
						                     (int)i,
						                     (int)size.Width,
						                     (int)size.Height);
						rpea.PrintPageEventArgs.Graphics.DrawString(s,printFont,Brushes.Black,rect);
						i += size.Height + 10;
					} else {
						i += printFont.GetHeight();
					}
					rpea.LocationAfterDraw = new PointF(rpea.PrintPageEventArgs.PageSettings.PaperSize.Width /2 + size.Width,i);
				} catch (Exception e) {
					MessageBox.Show (e.ToString() + " / " + e.Message );
					
				}
				
			}
			this.areaHeight = i;
			
			headFont.Dispose();
			printFont.Dispose();
			blackBrush.Dispose();
			return;
			 */
		}
		
//		public float DrawAreaHeight (ReportPageEventArgs rpea){
//			return 0;
//		}
		
		#endregion
		
		
		
		
		
		[Browsable(true), Category("Base Settings")]
		public GlobalEnums.ReportTypeEnum ReportType {
			get {
				return reportType;
			}
			set {
				if (reportType != value) {
					reportType = value;
					this.NotifyPropertyChanged("ReportType");
				}
			}
		}
		
		
		#region Sorting,grouping and ReportParameters
		
		[Browsable(false)]
		[XmlIgnoreAttribute]
		public ColumnCollection AvailableFieldsCollection {
			get {
				return this.availableFields;
			}
			set {
				this.availableFields = value;
			}
		}
		/// <summary>
		/// Get/Set a Collection of <see cref="SortColumn">SortColumn</see>
		/// </summary>
		
		[Browsable(false)]
		[XmlIgnoreAttribute]
		public ColumnCollection SortColumnCollection {
			get {
				return sortingCollection;
			}
		}
		
		
		[Browsable(false)]
		[XmlIgnoreAttribute]
		public ColumnCollection GroupColumnsCollection {
			get {
				if (this.groupingsCollection == null) {
					groupingsCollection = new ColumnCollection();
				}
				return groupingsCollection;
			}
		}
		
		[Browsable(false)]
		[XmlIgnoreAttribute]
		public SqlParametersCollection SqlParametersCollection
		{
			get{
				if (reportParametersCollection == null) {
					reportParametersCollection = new SqlParametersCollection();
				}
				return reportParametersCollection;
			}
			set {
				if (reportParametersCollection == null) {
					reportParametersCollection = new SqlParametersCollection();
				}
				if (reportParametersCollection != value) {
					reportParametersCollection = value;
					this.NotifyPropertyChanged("SqlParametersCollection");
				}
			}
		}
		#endregion
		
		#region DataRelated
		
		[Category("Data")]
		[DefaultValueAttribute ("")]
		public string ConnectionString {
			get {
				return connectionString;
			}
			set {
				if (connectionString != value) {
					connectionString = value;
					this.NotifyPropertyChanged("ConnectionString");
				}
			}
		}
		
		
		[Category("Data")]
		[DefaultValueAttribute ("")]
		public string CommandText {
			get {
				return commandText;
			}
			set {
				if (commandText != value) {
					commandText = value;
					this.NotifyPropertyChanged("CommandText");
				}
			}
		}
		
		
		[Category("Data")]
		public System.Data.CommandType CommandType {
			get {
				return commandType;
			}
			set {
				if (commandType != value) {
					commandType = value;
					this.NotifyPropertyChanged("CommandType");
				}
			}
		}
		
		
		[Category("Data")]
		public GlobalEnums.PushPullModelEnum DataModel {
			get {
				return dataModel;
			}
			set {
				if (dataModel != value) {
					dataModel = value;

					if (this.dataModel != GlobalEnums.PushPullModelEnum.FormSheet) {
						this.reportType = GlobalEnums.ReportTypeEnum.DataReport;
					} else {
						this.reportType = GlobalEnums.ReportTypeEnum.FormSheet;
					}
					this.NotifyPropertyChanged("DataModel");
				}
			}
		}
		
		
		
		#endregion
		
		#region OutPut Settings
//		string bla;
//		
//		[XmlAttributeAttribute("titleFont")]
//		public string TestName {
//			get {
//				return "Name = testName";
//			}
//			set { bla = value;}
//		}
		
		[Category("Output Settings")]
		public Font DefaultFont {
			get {
				return defaultFont;
			}
			set {
				if (defaultFont != value) {
					defaultFont = value;
					this.NotifyPropertyChanged("DefaultFont");
				}
			}
		}
		
		#endregion
		
		#region IDisposable
		public void Dispose(){
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		~ReportSettings()
		{
			Dispose(false);
		}
		
		protected virtual void Dispose(bool disposing){
			if (disposing) {
				// Free other state (managed objects).
				if (this.defaultFont != null) {
					this.defaultFont.Dispose();
				}
			}
			
			// Release unmanaged resources.
			// Set large fields to null.
			// Call Dispose on your base class.
		}
		#endregion
		
	}
}
