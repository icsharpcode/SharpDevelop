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
	public class TreeNode : ITreeNode
	{
		public IImage IconImage { get; protected set; } 
		public string ImageName { get; set; } 
		public string Name { get; set; }
		public virtual string Text { get; set; }
		public virtual string Type { get; protected set; }
		public virtual Func<IEnumerable<TreeNode>> GetChildren { get; protected set; }
		
		/// <summary>
		/// System.Windows.Media.ImageSource version of <see cref="IconImage"/>.
		/// </summary>
		public ImageSource ImageSource {
			get { 
				return this.IconImage == null ? null : this.IconImage.ImageSource;
			}
		}
		
		/// <summary>
		/// System.Drawing.Image version of <see cref="IconImage"/>.
		/// </summary>
		public Image Image {
			get { 
				return this.IconImage == null ? null : this.IconImage.Bitmap;
			}
		}
		
		public virtual bool CanSetText { 
			get { return false; }
		}
		
		Func<IEnumerable<ITreeNode>> ITreeNode.GetChildren {
			get {
				if (this.GetChildren == null)
					return null;
				return () => this.GetChildren();
			}
		}
		
		public virtual bool HasChildNodes {
			get { return this.GetChildren != null; }
		}

		public virtual IEnumerable<IVisualizerCommand> VisualizerCommands {
			get {
				return null;
			}
		}
		
		public virtual bool HasVisualizerCommands {
			get {
				return (VisualizerCommands != null) && (VisualizerCommands.Count() > 0);
			}
		}
		
		public bool IsPinned { get; set; }
		
		public TreeNode(string name, Func<IEnumerable<TreeNode>> getChildren)
		{
			this.Name = name;
			this.GetChildren = getChildren;
		}
		
		public TreeNode(string imageName, string name, string text, string type, Func<IEnumerable<TreeNode>> getChildren)
		{
			this.ImageName = imageName;
			if (imageName != null)
				this.IconImage = new ResourceServiceImage(imageName);
			this.Name = name;
			this.Text = text;
			this.Type = type;
			this.GetChildren = getChildren;
		}
		
		public virtual bool SetText(string newValue) { 
			return false;
		}
	}
}