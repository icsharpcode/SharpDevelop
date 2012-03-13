// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Media;

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
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		IImage image;
		string name;
		string value;
		string type;
				
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
		
		public bool CanSetName { get; protected set; }
		
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
		
		public bool CanSetValue { get; protected set; }
		
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
		
		public Func<IEnumerable<TreeNode>> GetChildren { get; protected set; }
		
		public bool HasChildren {
			get { return GetChildren != null; }
		}
		
		public IEnumerable<IVisualizerCommand> VisualizerCommands { get; protected set; }
		
		public bool HasVisualizerCommands { get; protected set; }
		
		public TreeNode(string name, Func<IEnumerable<TreeNode>> getChildren)
		{
			this.Name = name;
			this.GetChildren = getChildren;
		}
		
		public TreeNode(string imageName, string name, string value, string type, Func<IEnumerable<TreeNode>> getChildren)
		{
			this.Image = string.IsNullOrEmpty(imageName) ? null : new ResourceServiceImage(imageName);
			this.Name = name;
			this.Value = value;
			this.Type = type;
			this.GetChildren = getChildren;
		}
		
		protected virtual void OnPropertyRead(string name)
		{
			if (PropertyRead != null) {
				PropertyRead(this, new PropertyEventArgs() { Name = name});
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