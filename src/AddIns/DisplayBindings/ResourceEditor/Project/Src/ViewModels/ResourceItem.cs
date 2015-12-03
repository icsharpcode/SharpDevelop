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
using System.Windows;
using System.Windows.Documents;
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
	
	public class ResourceItem : DependencyObject, INotifyPropertyChanged
	{
		ResourceItemEditorType resourceType;
		ResourceEditorViewModel resourceEditor;
		string nameBeforeEditing;
		string highlightText;
		
		public ResourceItem(ResourceEditorViewModel resourceEditor, string name, object resourceValue)
		{
			this.resourceEditor = resourceEditor;
			this.Name = name;
			this.SortingCriteria = name;
			this.ResourceValue = resourceValue;
			this.resourceType = GetResourceTypeFromValue(resourceValue);
		}
		
		public ResourceItem(ResourceEditorViewModel resourceEditor, string name, object resourceValue, string comment)
		{
			this.resourceEditor = resourceEditor;
			this.Name = name;
			this.SortingCriteria = name;
			this.ResourceValue = resourceValue;
			this.resourceType = GetResourceTypeFromValue(resourceValue);
			this.Comment = comment;
			this.RichComment = comment;
		}

		#region INotifyPropertyChanged implementation
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		void RaisePropertyChanged(string name)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}
		
		#endregion
		
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(ResourceItem),
				new FrameworkPropertyMetadata(NamePropertyChanged));
		
		static void NamePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			((ResourceItem)obj).Highlight(((ResourceItem)obj).highlightText);
		}
		
		public string Name {
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		
		public static readonly DependencyProperty DisplayNameProperty =
			DependencyProperty.Register("DisplayName", typeof(object), typeof(ResourceItem),
			                            new FrameworkPropertyMetadata());
		
		public object DisplayName {
			get { return (object)GetValue(DisplayNameProperty); }
			set { SetValue(DisplayNameProperty, value); }
		}
		
		public static readonly DependencyProperty SortingCriteriaProperty =
			DependencyProperty.Register("SortingCriteria", typeof(string), typeof(ResourceItem),
				new FrameworkPropertyMetadata());
		
		public string SortingCriteria {
			get { return (string)GetValue(SortingCriteriaProperty); }
			set { SetValue(SortingCriteriaProperty, value); }
		}
		
		public static readonly DependencyProperty ResourceValueProperty =
			DependencyProperty.Register("ResourceValue", typeof(object), typeof(ResourceItem),
				new FrameworkPropertyMetadata());
		
		public object ResourceValue {
			get { return (object)GetValue(ResourceValueProperty); }
			set { SetValue(ResourceValueProperty, value); Highlight(highlightText); }
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
		
		public static readonly DependencyProperty IsEditingProperty =
			DependencyProperty.Register("IsEditing", typeof(bool), typeof(ResourceItem),
				new FrameworkPropertyMetadata());
		
		public bool IsEditing {
			get { return (bool)GetValue(IsEditingProperty); }
			set { SetValue(IsEditingProperty, value); }
		}
		
		public bool IsNew {
			get;
			set;
		}
		
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			
			if (e.Property.Name == DisplayNameProperty.Name
			    || e.Property.Name == RichContentProperty.Name
			    || e.Property.Name == RichCommentProperty.Name) {
				return;
			}
			
			if (e.Property.Name == ResourceValueProperty.Name) {
				// Update content property as well
				RaisePropertyChanged("Content");
				Highlight(highlightText);
			}
			if (e.Property.Name == IsEditingProperty.Name) {
				bool previouslyEditing = (bool)e.OldValue;
				bool isEditing = (bool)e.NewValue;
				if (!previouslyEditing && isEditing) {
					// Save initial name to compare it later on cancellation
					nameBeforeEditing = Name;
				} else if (previouslyEditing && !isEditing) {
					// Make dirty, if name has changed after finishing edit
					if (nameBeforeEditing != Name) {
						// New name
						if (String.IsNullOrEmpty(Name)) {
							// Empty name is not valid -> revert silently
							Name = nameBeforeEditing;
						} else if (resourceEditor.ContainsResourceName(Name)) {
							// Resource names must be unique -> revert and show message
							SD.MessageService.ShowWarning("${res:ResourceEditor.ResourceList.KeyAlreadyDefinedWarning}");
							Name = nameBeforeEditing;
						} else {
							// New name seems to be valid
							SortingCriteria = Name;
							resourceEditor.MakeDirty();
						}
					}
					IsNew = false;
				}
			} else {
				if (e.Property.Name == NameProperty.Name)
					SortingCriteria = Name;
				resourceEditor.MakeDirty();
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
		
		public static readonly DependencyProperty RichContentProperty =
			DependencyProperty.Register("RichContent", typeof(object), typeof(ResourceItem),
			                            new FrameworkPropertyMetadata());
		
		public object RichContent {
			get { return (object)GetValue(RichContentProperty); }
			set { SetValue(RichContentProperty, value); }
		}
		
		public static readonly DependencyProperty CommentProperty =
			DependencyProperty.Register("Comment", typeof(string), typeof(ResourceItem),
				new FrameworkPropertyMetadata(""));
		
		public string Comment {
			get { return (string)GetValue(CommentProperty); }
			set { SetValue(CommentProperty, value); }
		}
		
		public static readonly DependencyProperty RichCommentProperty =
			DependencyProperty.Register("RichComment", typeof(object), typeof(ResourceItem),
			                            new FrameworkPropertyMetadata());
		
		public object RichComment {
			get { return (object)GetValue(RichCommentProperty); }
			set { SetValue(RichCommentProperty, value); }
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
						} catch (Exception ex) {
							SD.MessageService.ShowWarningFormatted("${res:ResourceEditor.Messages.CantLoadResourceFromFile}", ex.Message);
							return false;
						}
						break;
					case ResourceItemEditorType.Icon:
						try {
							newValue = new Icon(fileDialog.FileName);
						} catch (Exception ex) {
							SD.MessageService.ShowWarningFormatted("${res:ResourceEditor.Messages.CantLoadResourceFromFile}", ex.Message);
							return false;
						}
						break;
					case ResourceItemEditorType.Cursor:
						try {
							newValue = new Cursor(fileDialog.FileName);
						} catch (Exception ex) {
							SD.MessageService.ShowWarningFormatted("${res:ResourceEditor.Messages.CantLoadResourceFromFile}", ex.Message);
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
		
		public void Highlight(string text)
		{
			this.highlightText = text;
			if (string.IsNullOrEmpty(text)) {
				DisplayName = Name;
				RichContent = Content;
				RichComment = Comment;
			} else {
				DisplayName = CreateSpan(Name, text);
				RichContent = CreateSpan(Content ?? "", text);
				RichComment = CreateSpan(Comment ?? "", text);
			}
			RaisePropertyChanged("DisplayName");
			RaisePropertyChanged("RichContent");
			RaisePropertyChanged("RichComment");
		}

		Span CreateSpan(string text, string matchText)
		{
			int startIndex = 0;
			int match;
			var span = new Span();
			do {
				match = text.IndexOf(matchText, startIndex, StringComparison.OrdinalIgnoreCase);
				if (match > -1) {
					span.Inlines.Add(new Run(text.Substring(startIndex, match - startIndex)));
					span.Inlines.Add(new Span(new Run(text.Substring(match, matchText.Length))) {
						Background = System.Windows.Media.Brushes.Yellow
					});
				} else {
					span.Inlines.Add(new Run(text.Substring(startIndex, text.Length - startIndex)));
				}
				startIndex = match + matchText.Length;
			} while (match > -1);
			return span;
		}
	}
}
