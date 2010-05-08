/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 12.11.2007
 * Zeit: 09:23
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;

using ICSharpCode.Reports.Core;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of Section.
	/// </summary>
	/// 
	[TypeDescriptionProvider(typeof(SectionItemTypeProvider))]
	[Designer(typeof(ICSharpCode.Reports.Addin.SectionDesigner))]
	public class BaseSection:AbstractItem
	{
		private int  sectionOffset;
		private int sectionMargin;
		private bool pageBreakAfter;
		private bool  canGrow ;
		private bool canShrink ;

		public BaseSection():base()
		{
			base.FrameColor = Color.Black;
			TypeDescriptor.AddProvider(new SectionItemTypeProvider(), typeof(BaseSection));
		}
		
		
		[System.ComponentModel.EditorBrowsableAttribute()]
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint(e);
			Draw(e.Graphics);
		}
		
		
		public  override void Draw(Graphics graphics)
		{
			base.DrawControl(graphics,Rectangle.Inflate(this.ClientRectangle,-2,-2));
		}
	
		
		#region Propertys
		
		public int SectionOffset {
			get { return sectionOffset; }
			set { sectionOffset = value; }
		}
		
		public int SectionMargin {
			get { return sectionMargin; }
			set { sectionMargin = value; }
		}
		
		public bool PageBreakAfter {
			get { return pageBreakAfter; }
			set { pageBreakAfter = value; }
		}
		
		public bool CanGrow {
			get { return canGrow; }
			set { canGrow = value; }
		}
		
		public bool CanShrink {
			get { return canShrink; }
			set { canShrink = value; }
		}
		
		#endregion
	}
	
	internal class SectionItemTypeProvider : TypeDescriptionProvider
	{
		public SectionItemTypeProvider() :  base(TypeDescriptor.GetProvider(typeof(AbstractItem)))
		{
		}
		
//		public SectionItemTypeProvider(TypeDescriptionProvider parent): base(parent)
//		{
//		}

	
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			ICustomTypeDescriptor td = base.GetTypeDescriptor(objectType,instance);
			return new SectionItemDescriptor(td, instance);
		}
	}
	
	
	internal class SectionItemDescriptor : CustomTypeDescriptor
	{
//		private BaseTextItem instance;
		
		public SectionItemDescriptor(ICustomTypeDescriptor parent, object instance)
			: base(parent)
		{
//			instance = instance as BaseTextItem;
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
			PropertyDescriptor prop = null;
			
			prop = props.Find("SectionOffset",true);
			allProperties.Add(prop);
			
			prop = props.Find("SectionMargin",true);
			allProperties.Add(prop);
			
			prop = props.Find("DrawBorder",true);
			allProperties.Add(prop);
			
			prop = props.Find("PageBreakAfter",true);
			allProperties.Add(prop);
			
			prop = props.Find("Controls",true);
			allProperties.Add(prop);
			
			prop = props.Find("FrameColor",true);
			allProperties.Add(prop);
			return new PropertyDescriptorCollection(allProperties.ToArray());
		}
	}
}
