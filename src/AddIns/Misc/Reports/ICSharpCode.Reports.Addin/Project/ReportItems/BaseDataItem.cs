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
