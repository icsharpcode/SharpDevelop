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
using System.Resources;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop;

namespace ResourceEditor.ViewModels
{
	/// <summary>
	/// Defines the type of resource item supported by editor.
	/// </summary>
	public enum ResourceItemEditorType
	{
		Unknown,
		String,
		Boolean,
		Bitmap,
		Icon,
		Cursor,
		Binary
	}
	
	public class ResourceItem : INotifyPropertyChanged
	{
		string name;
		object resourceValue;
		string comment;
		ResourceItemEditorType resourceType;
		ResourceEditorViewModel resourceEditor;
		
		public ResourceItem(ResourceEditorViewModel resourceEditor, string name, object resourceValue)
		{
			this.resourceEditor = resourceEditor;
			this.name = name;
			this.resourceValue = resourceValue;
			this.resourceType = GetResourceTypeFromValue(resourceValue);
		}
		
		public ResourceItem(ResourceEditorViewModel resourceEditor, string name, object resourceValue, string comment)
		{
			this.resourceEditor = resourceEditor;
			this.name = name;
			this.resourceValue = resourceValue;
			this.resourceType = GetResourceTypeFromValue(resourceValue);
			this.comment = comment;
		}

		#region INotifyPropertyChanged implementation
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		#endregion
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
				OnPropertyChanged("Name");
			}
		}
		
		public object ResourceValue {
			get {
				return resourceValue;
			}
			set {
				resourceValue = value;
				OnPropertyChanged("ResourceValue");
				OnPropertyChanged("ResourceType");
				OnPropertyChanged("Content");
				resourceEditor.MakeDirty();
			}
		}
		
		public string DisplayedResourceType {
			get {
				return ResourceValue == null ? "(Nothing/null)" : ResourceValue.GetType().FullName;
			}
		}
		
		public ResourceItemEditorType ResourceType {
			get {
				return resourceType;
			}
		}
		
		ResourceItemEditorType GetResourceTypeFromValue(object val)
		{
			if (this.ResourceValue == null) {
				return ResourceItemEditorType.Unknown;
			}
			switch (this.ResourceValue.GetType().ToString()) {
				case "System.String":
					return ResourceItemEditorType.String;
				case "System.Drawing.Bitmap":
					return ResourceItemEditorType.Bitmap;
				case "System.Drawing.Icon":
					return ResourceItemEditorType.Icon;
				case "System.Windows.Forms.Cursor":
					return ResourceItemEditorType.Cursor;
				case "System.Byte[]":
					return ResourceItemEditorType.Binary;
				case "System.Boolean":
					return ResourceItemEditorType.Boolean;
				default:
					return ResourceItemEditorType.Unknown;
			}
		}
		
		public string Content {
			get {
				return ToString();
			}
		}
		
		public string Comment {
			get {
				return comment;
			}
			set {
				comment = value;
				OnPropertyChanged("Comment");
			}
		}

		public override string ToString()
		{
			if (ResourceValue == null) {
				return "(Nothing/null)";
			}
			
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
		
		public ResXDataNode ToResXDataNode(Func<Type, string> typeNameConverter = null)
		{
			var node = new ResXDataNode(Name, ResourceValue, typeNameConverter) {
				Comment = Comment
			};
			return node;
		}
		
		public bool UpdateFromFile()
		{
			var fileDialog = new Microsoft.Win32.OpenFileDialog();
			fileDialog.AddExtension = true;
			fileDialog.Filter = "All files (*.*)|*.*";
			fileDialog.CheckFileExists = true;
			
			if (fileDialog.ShowDialog().Value) {
				object newValue = null;
				switch (resourceType) {
					case ResourceItemEditorType.Bitmap:
						try {
							newValue = new Bitmap(fileDialog.FileName);
						} catch {
							SD.MessageService.ShowWarning("Can't load bitmap file.");
							return false;
						}
						break;
				}
					
				if (newValue != null) {
					ResourceValue = newValue;
					return true;
				}
			}
			
			return false;
		}
	}
}
