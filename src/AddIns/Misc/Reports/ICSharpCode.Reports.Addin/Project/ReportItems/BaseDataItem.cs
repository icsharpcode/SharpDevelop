// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Drawing;

using ICSharpCode.Reports.Addin.TypeProviders;

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
	
}
