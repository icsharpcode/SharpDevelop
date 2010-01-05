/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 01.11.2008
 * Zeit: 22:05
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of TextBasedDesignerActionList.
	/// </summary>
	internal class TextBasedDesignerActionList :DesignerActionList
	{
		private IComponent component;
		private DesignerActionUIService designerActionUISvc;

		public TextBasedDesignerActionList(IComponent component):base(component)
		{
			this.component = component;
			this.designerActionUISvc =
            GetService(typeof(DesignerActionUIService))
            as DesignerActionUIService;

		}
		
		
		public override DesignerActionItemCollection GetSortedActionItems()
		{
			DesignerActionItemCollection actions = new DesignerActionItemCollection();
			actions.Add (new DesignerActionPropertyItem ("DrawBorder","DrawBorder","Appearance"));
			actions.Add (new DesignerActionPropertyItem ("Font","Font","Appearance"));
			actions.Add (new DesignerActionPropertyItem ("ContentAlignment","ContentAlignment","Appearance"));
			actions.Add (new DesignerActionPropertyItem ("StringTrimming","StringTrimming","Appearance"));
			actions.Add (new DesignerActionPropertyItem ("FormatString","FormatString","Appearance"));
			return actions;
		}
		
		
		public bool DrawBorder
		{
			get {return this.Item.DrawBorder;}
			set {
				SmartTagTransactions t = new SmartTagTransactions("border",this,this.Item);
				this.SetProperty ("DrawBorder",value); 
				t.Commit();
			}
		}
		
		public System.Drawing.Font Font
		{
			get {return this.Item.Font;}
			set {SmartTagTransactions t = new SmartTagTransactions("font",this,this.Item);
				this.SetProperty ("Font",value);
				t.Commit();
			}
		}
		
		
		public System.Drawing.ContentAlignment ContentAlignment
		{
			get {return this.Item.ContentAlignment;}
			set {SmartTagTransactions t = new SmartTagTransactions("contentalignment",this,this.Item);
				this.SetProperty ("ContentAlignment",value);
				t.Commit();
			}
		}
		
		public System.Drawing.StringTrimming StringTrimming
		{
			get {return this.Item.StringTrimming;}
			set {SmartTagTransactions t = new SmartTagTransactions("trimming",this,this.Item);
				this.SetProperty ("StringTrimming",value);
				t.Commit();}
		}
		
		public string FormatString 
		{
			get {return this.Item.FormatString;}
			set {SmartTagTransactions t = new SmartTagTransactions("format",this,this.Item);
				this.SetProperty("FormatString",value);
				t.Commit();}
		}
			
		
		private BaseTextItem Item
		{
			get {return (BaseTextItem) this.component;}
		}
		
		
		private void SetProperty (string propName, object value)
		{
			PropertyDescriptor property = TypeDescriptor.GetProperties(this.Item)[propName];
			if (property == null) {
				throw new ArgumentException (this.Item.Text);
			} else {
				property.SetValue (this.Item,value);
			}
		}
	}
}
