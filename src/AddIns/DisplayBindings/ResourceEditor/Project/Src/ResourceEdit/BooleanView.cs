// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ResourceEditor
{
	class BooleanView : Panel, IResourceView
	{
		public event ResourceChangedEventHandler ResourceChanged;
		private ResourceItem resourceItem;
		
		private RadioButton trueRadioButton = new RadioButton();
		private RadioButton falseRadioButton = new RadioButton();
		
		public BooleanView(ResourceItem item)
		{
			trueRadioButton.Location = new Point(4, 4);
			trueRadioButton.Text = "True";
			trueRadioButton.CheckedChanged += new EventHandler(valueChanged);
			Controls.Add(trueRadioButton);
			
			falseRadioButton.Location = new Point(4, 24);
			falseRadioButton.Text = "False";
			falseRadioButton.CheckedChanged += new EventHandler(valueChanged);
			Controls.Add(falseRadioButton);
			
			ResourceItem = item;
		}
		
		public bool WriteProtected
		{
			get {
				return ! trueRadioButton.Enabled;
			}
			set {
				trueRadioButton.Enabled = ! value;
				falseRadioButton.Enabled = ! value;
			}
		}
		
		public ResourceItem ResourceItem
		{
			get {
				return resourceItem;
			}
			set {
				this.resourceItem = value;
				if((bool)resourceItem.ResourceValue == true) {
					trueRadioButton.Checked = true;
				} else {
					falseRadioButton.Checked = true;
				}
			}
		}
		
		protected void OnResourceChanged(string resourceName, object val)
		{
			if(ResourceChanged != null) {
				ResourceChanged(this, new ResourceEventArgs(resourceName, val));
			}
		}
		
		void valueChanged(object sender, EventArgs e)
		{
			OnResourceChanged(resourceItem.Name, trueRadioButton.Checked);
		}
	}
}
