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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Debugging;

namespace Debugger.AddIn.TreeModel
{
	/// <summary>
	/// A node in the variable tree.
	/// </summary>
	public class TreeNode : INotifyPropertyChanged
	{
		public event EventHandler<PropertyEventArgs> PropertyRead;
		public event PropertyChangedEventHandler PropertyChanged;
		
		IImage image;
		string name;
		string value;
		string type;
		
		public bool CanDelete { get; set; }
		
		public IImage Image {
			get {
				OnPropertyRead("Image");
				return this.image;
			}
			set {
				if (this.image != value) {
					this.image = value;
					OnPropertyChanged("Image");
				}
			}
		}
		
		public string Name {
			get {
				OnPropertyRead("Name");
				return this.name;
			}
			set {
				if (this.name != value) {
					this.name = value;
					OnPropertyChanged("Name");
				}
			}
		}
		
		public bool CanSetName { get; set; }
		
		public string Value {
			get {
				OnPropertyRead("Value");
				return this.value;
			}
			set {
				if (this.value != value) {
					this.value = value;
					OnPropertyChanged("Value");
				}
			}
		}
		
		public bool CanSetValue { get; set; }
		
		public string Type {
			get {
				OnPropertyRead("Type");
				return this.type;
			}
			set {
				if (this.type != value) {
					this.type = value;
					OnPropertyChanged("Type");
				}
			}
		}
		
		string contextMenuAddInTreeEntry = "/AddIns/Debugger/Tooltips/ContextMenu/TreeNode";
		public string ContextMenuAddInTreeEntry {
			get { return contextMenuAddInTreeEntry; }
			set {
				if (this.contextMenuAddInTreeEntry != value) {
					contextMenuAddInTreeEntry = value;
					OnPropertyChanged("ContextMenuAddInTreeEntry");
				}
			}
		}
		
		public Func<IEnumerable<TreeNode>> GetChildren { get; protected set; }
		
		public bool HasChildren {
			get { return GetChildren != null; }
		}
		
		public IEnumerable<IVisualizerCommand> VisualizerCommands { get; protected set; }
		
		public bool HasVisualizerCommands {
			get {
				return (VisualizerCommands != null) && VisualizerCommands.Any();
			}
		}
		
		public TreeNode(string name, Func<IEnumerable<TreeNode>> getChildren)
		{
			this.Name = name;
			this.GetChildren = getChildren;
		}
		
		public TreeNode(IImage image, string name, string value, string type, Func<IEnumerable<TreeNode>> getChildren)
		{
			this.Image = image;
			this.Name = name;
			this.Value = value;
			this.Type = type;
			this.GetChildren = getChildren;
		}
		
		protected virtual void OnPropertyRead(string name)
		{
			if (PropertyRead != null) {
				PropertyRead(this, new PropertyEventArgs() { Name = name });
			}
		}
		
		protected virtual void OnPropertyChanged(string name)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}
	}
	
	public class PropertyEventArgs: EventArgs
	{
		public string Name { get; set; }
	}
}
