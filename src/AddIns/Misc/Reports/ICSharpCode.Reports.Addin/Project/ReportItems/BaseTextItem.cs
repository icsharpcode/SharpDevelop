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

using ICSharpCode.Reports.Addin.Dialogs;
using ICSharpCode.Reports.Addin.TypeProviders;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Dialogs;
using ICSharpCode.Reports.Core.Globals;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of ReportTextItem.
	/// </summary>

	[Designer(typeof(ICSharpCode.Reports.Addin.Designer.TextItemDesigner))]
	public class BaseTextItem:AbstractItem
	{
		
		private string formatString;
		private StringFormat stringFormat;
		private StringTrimming stringTrimming;
		private ContentAlignment contentAlignment;
		
		
		public BaseTextItem():base()
		{
			base.DefaultSize = GlobalValues.PreferedSize;
			base.Size = GlobalValues.PreferedSize;
			base.BackColor = Color.White;
			this.contentAlignment = ContentAlignment.TopLeft;
			TypeDescriptor.AddProvider(new TextItemTypeProvider(), typeof(BaseTextItem));
		}
		
		
		[System.ComponentModel.EditorBrowsableAttribute()]
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint(e);
			this.Draw(e.Graphics);
		}
		
		
		public override void Draw(Graphics graphics)
		{
			
			
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			using (Brush b = new SolidBrush(this.BackColor)){
				graphics.FillRectangle(b, base.DrawingRectangle);
			}
			
			StringTrimming designTrimmimg = StringTrimming.EllipsisCharacter;
			
			if (this.stringTrimming != StringTrimming.None) {
				designTrimmimg = stringTrimming;
			}
			
			StringFormat stringFormat = TextDrawer.BuildStringFormat(designTrimmimg,contentAlignment);
			
			if (this.RightToLeft == System.Windows.Forms.RightToLeft.Yes) {
				stringFormat.FormatFlags = stringFormat.FormatFlags | StringFormatFlags.DirectionRightToLeft;
			}
			
			
			TextDrawer.DrawString(graphics,this.Text,this.Font,
			                      new SolidBrush(this.ForeColor),
			                      this.ClientRectangle,
			                      stringFormat);
			
			base.DrawControl(graphics,base.DrawingRectangle);
		}
		
		
		
		[EditorAttribute(typeof(DefaultTextEditor),
		                 typeof(System.Drawing.Design.UITypeEditor) )]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value;
				this.Invalidate();
			}
		}
		
		
		#region Format and alignment
		
		[Browsable(true),
		 Category("Appearance"),
		 Description("String to format Number's Date's etc")]
		[DefaultValue("entry1")]
		[TypeConverter(typeof(FormatStringConverter))]

		public string FormatString {
			get { return formatString; }
			set {
				formatString = value;
				Invalidate();
			}
		}

		
		[Browsable(false)]
		public StringFormat StringFormat {
			get { return stringFormat; }
			set {
				stringFormat = value;
				this.Invalidate();
			}
		}
		
		
		[Category("Appearance")]
		public StringTrimming StringTrimming {
			get { return stringTrimming; }
			set {
				stringTrimming = value;
				this.Invalidate();
			}
		}
		
		
		
		[Category("Appearance")]
		[EditorAttribute(typeof(System.Drawing.Design.ContentAlignmentEditor),
		                 typeof(System.Drawing.Design.UITypeEditor) )]
		public ContentAlignment ContentAlignment {
			get { return contentAlignment; }
			set {
				contentAlignment = value;
				this.Invalidate();
			}
		}
		
		#endregion
		
		#region RighToLeft
		
		[Category("Appearance")]
		public  System.Windows.Forms.RightToLeft RTL
		{
			get { return base.RightToLeft; }
			set { base.RightToLeft = value; }
		}

		#endregion
		
		#region DataType
		
		[Browsable(true),
		 Category("Databinding"),
		 Description("Datatype of the underlying Column")]
		[DefaultValue("System.String")]
		[TypeConverter(typeof(DataTypeStringConverter))]

		public string DataType {get;set;}
		
		#endregion
		
		#region Expression
		
		[Browsable(true),
		 Category("Expression"),
		 Description("Enter a valid Expression")]
		[EditorAttribute(typeof(DefaultTextEditor),
		                 typeof(System.Drawing.Design.UITypeEditor) )]
		public string Expression {get;set;}
		
		#endregion
		
		
		#region CanGrow/CanShrink
		
		public bool CanGrow {get;set;}
		
		public bool CanShrink {get;set;}
		
		#endregion
	}
	
}
