// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using ICSharpCode.Reports.Addin.Designer;
using ICSharpCode.Reports.Core;

namespace ICSharpCode.Reports.Addin
	
{
	/// <summary>
	/// Description of BaseDataItem.
	/// </summary>
	[Designer(typeof(ICSharpCode.Reports.Addin.Designer.DataItemDesigner))]
	public class BaseDataItem:BaseTextItem
	{
		private string columnName;
		private string baseTableName;
		private string dbValue;
		private string nullValue;
		
		public BaseDataItem():base()
		{
			TypeDescriptor.AddProvider(new DataItemTypeProvider(), typeof(BaseDataItem));
		}
		
		
		
		[System.ComponentModel.EditorBrowsableAttribute()]
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint(e);
			this.Draw(e.Graphics);
		}
		
		
		public override void Draw(Graphics graphics)
		{
			base.Draw(graphics);
		}
		
		[Browsable(true),
		 Category("Databinding"),
		 Description("Datatype of the underlying Column")]
		public string ColumnName {
			get { return columnName; }
			set { columnName = value; }
		}
		
		
		[Browsable(true),
		 Category("Databinding"),
		 Description("TableName")]
		public string BaseTableName {
			get { return baseTableName; }
			set { baseTableName = value; }
		}
		
		public string DbValue {
			get { return dbValue; }
			set { dbValue = value; }
		}
		
		
		public string NullValue {
			get { return nullValue; }
			set { nullValue = value; }
		}
	}
	
	
	
	internal class DataItemTypeProvider : TypeDescriptionProvider
	{
		
		public DataItemTypeProvider() :  base(TypeDescriptor.GetProvider(typeof(AbstractItem)))
		{
		}
		
	
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			ICustomTypeDescriptor td = base.GetTypeDescriptor(objectType,instance);
			return new DataItemTypeDescriptor(td, instance);
		}
	}
	
	
	
	internal class DataItemTypeDescriptor : CustomTypeDescriptor
	{
		
		public DataItemTypeDescriptor(ICustomTypeDescriptor parent, object instance)
			: base(parent)
		{
		}

		
		public override PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(null);
		}
		
		
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection props = base.GetProperties(attributes);
			List<PropertyDescriptor> allProperties = new List<PropertyDescriptor>();
		
			DesignerHelper.AddDefaultProperties(allProperties,props);
			DesignerHelper.AddTextbasedProperties(allProperties,props);
	
			PropertyDescriptor prop = props.Find("Text",true);
			allProperties.Add(prop);
			
			prop = props.Find("DrawBorder",true);
			allProperties.Add(prop);
			
			prop = props.Find("FrameColor",true);
			allProperties.Add(prop);
			
			prop = props.Find("ForeColor",true);
			allProperties.Add(prop);
			
			prop = props.Find("Visible",true);
			allProperties.Add(prop);
			
			prop = props.Find("ColumnName",true);
			allProperties.Add(prop);
			
			prop = props.Find("BaseTableName",true);
			allProperties.Add(prop);
			
			prop = props.Find("DbValue",true);
			allProperties.Add(prop);
			
			prop = props.Find("NullValue",true);
			allProperties.Add(prop);
			
			return new PropertyDescriptorCollection(allProperties.ToArray());
		}
	}
}
