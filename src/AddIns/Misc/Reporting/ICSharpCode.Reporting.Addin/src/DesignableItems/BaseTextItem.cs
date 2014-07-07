/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.03.2014
 * Time: 20:30
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows;
using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Addin.Designer;
using ICSharpCode.Reporting.Addin.Dialogs;
using ICSharpCode.Reporting.Addin.TypeProvider;

namespace ICSharpCode.Reporting.Addin.DesignableItems
{
	/// <summary>
	/// Description of BaseTextItem.
	/// </summary>
	[Designer(typeof(TextItemDesigner))]
	public class BaseTextItem:AbstractItem
	{
		const string datatypeOfTheUnderlyingColumn = "Datatype of the underlying Column";

		
		string formatString;
		StringFormat stringFormat;
		StringTrimming stringTrimming;
		ContentAlignment contentAlignment;
		TextAlignment textAlignment;
		
		public BaseTextItem(){
			DefaultSize = GlobalValues.PreferedSize;
			Size = GlobalValues.PreferedSize;
			BackColor = Color.White;
			contentAlignment = ContentAlignment.TopLeft;
			textAlignment = TextAlignment.Left;
			TypeDescriptor.AddProvider(new TextItemTypeProvider(), typeof(BaseTextItem));
		}
		
		
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e){
			base.OnPaint(e);
			Draw(e.Graphics);
		}
		
		
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			using (var backgroundBrush = new SolidBrush(this.BackColor)){
				graphics.FillRectangle(backgroundBrush, base.DrawingRectangle);
			}
			
			var designTrimmimg = StringTrimming.EllipsisCharacter;
			
			if (stringTrimming != StringTrimming.None) {
				designTrimmimg = stringTrimming;
			}
			
//			stringFormat = TextDrawer.BuildStringFormat(designTrimmimg, contentAlignment);
			stringFormat = TextDrawer.BuildStringFormat(designTrimmimg, textAlignment);
			using (var textBrush = new SolidBrush(ForeColor)) {
				TextDrawer.DrawString(graphics, Text, Font, textBrush, ClientRectangle, stringFormat);
			}		
			DrawControl(graphics, base.DrawingRectangle);
		}
		
		
		
		[EditorAttribute(typeof(DefaultTextEditor),
		                 typeof(UITypeEditor) )]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value;
				Invalidate();
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
				Invalidate();
			}
		}
		
		
		[Category("Appearance")]
		public StringTrimming StringTrimming {
			get { return stringTrimming; }
			set {
				stringTrimming = value;
				Invalidate();
			}
		}
		
		
		[Category("Appearance")]
		[EditorAttribute(typeof(ContentAlignmentEditor),
		                 typeof(UITypeEditor) )]
		public ContentAlignment ContentAlignment {
			get { return contentAlignment; }
			set {
				contentAlignment = value;
				Invalidate();
			}
		}
		
		
		[Category("Appearance")]
		public TextAlignment TextAlignment {
			get { return textAlignment;}
			set {textAlignment = value;
				Invalidate();}
		}
		
		#endregion
		
		
		#region DataType
		
		
		[Category("Databinding"),Description(datatypeOfTheUnderlyingColumn)]
		[DefaultValue("System.String")]
		[TypeConverter(typeof(DataTypeStringConverter))]
		public string DataType {get;set;}
		
		#endregion
		
		#region Expression
		
		[Browsable(true),
		 Category("Expression"),
		 Description("Enter a valid Expression")]
		[EditorAttribute(typeof(ExpressionEditor),
		                 typeof(UITypeEditor) )]
		public string Expression {get;set;}
		
		#endregion
		
		
		#region CanGrow/CanShrink
		
		public bool CanGrow {get;set;}
		
		public bool CanShrink {get;set;}
		
		#endregion
	}
}
