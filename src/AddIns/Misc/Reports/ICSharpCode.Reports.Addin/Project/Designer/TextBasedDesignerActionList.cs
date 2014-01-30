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
using System.ComponentModel.Design;

using ICSharpCode.Reports.Addin.Dialogs;

namespace ICSharpCode.Reports.Addin.Designer
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
		
		
		[TypeConverter(typeof(FormatStringConverter))]
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
