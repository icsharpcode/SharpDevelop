using System;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace ResourceEditor
{
	public class ResourceItem
	{
		string name;
		object resourceValue;
		
		public ResourceItem(string name, object resourceValue)
		{
			this.name = name;
			this.resourceValue = resourceValue;
		}
		
		public string Name
		{
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public object ResourceValue
		{
			get {
				return resourceValue;
			}
			set {
				resourceValue = value;
			}
		}
		
		public int ImageIndex
		{
			get {
				switch(this.resourceValue.GetType().ToString()) {
					case "System.String":
						return 0;
					case "System.Drawing.Bitmap":
						return 1;
					case "System.Drawing.Icon":
						return 2;
					case "System.Windows.Forms.Cursor":
						return 3;
					case "System.Byte[]":
						return 4;
					default:
						return 5;
				}
			}
		}
		
		public override string ToString()
		{
			string type = ResourceValue.GetType().FullName;
			string tmp = String.Empty;
			
			switch (type) {
				case "System.String":
					tmp = ResourceValue.ToString();
					break;
				case "System.Byte[]":
					tmp = "[Size = " + ((byte[])ResourceValue).Length + "]";
					break;
				case "System.Drawing.Bitmap":
					Bitmap bmp = ResourceValue as Bitmap;
					tmp = "[Width = " + bmp.Size.Width + ", Height = " + bmp.Size.Height + "]";
					break;
				case "System.Drawing.Icon":
					Icon icon = ResourceValue as Icon;
					tmp = "[Width = " + icon.Size.Width + ", Height = " + icon.Size.Height + "]";
					break;
				case "System.Windows.Forms.Cursor":
					Cursor c = ResourceValue as Cursor;
					tmp = "[Width = " + c.Size.Width + ", Height = " + c.Size.Height + "]";
					break;
				case "System.Boolean":
					tmp = ResourceValue.ToString();
					break;
				default:
					tmp = ResourceValue.ToString();
					break;
			}
			return tmp;
		}
	}
}
