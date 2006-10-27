// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

namespace ResourceEditor
{
	class TextView : TextBox, IResourceView
	{
		public event ResourceChangedEventHandler ResourceChanged;
		private ResourceItem resourceItem;
		
		public TextView(ResourceItem item)
		{
			this.Multiline = true;
			this.ResourceItem = item;
			this.ScrollBars = ScrollBars.Both;
			this.TextChanged += new EventHandler(textChanged);
		}
		
		public bool WriteProtected
		{
			get {
				return ! Enabled;
			}
			set {
				Enabled = ! value;
			}
		}
		
		public ResourceItem ResourceItem
		{
			get {
				return resourceItem;
			}
			set {
				resourceItem = value;
				Text = (string)value.ResourceValue;
			}
		}
		
		protected void OnResourceChanged(string resourceName, object val)
		{
			if(ResourceChanged != null) {
				ResourceChanged(this, new ResourceEventArgs(resourceName, val));
			}
		}
		
		void textChanged(object sender, EventArgs e)
		{
			OnResourceChanged(resourceItem.Name, Text);
		}
	}
}
