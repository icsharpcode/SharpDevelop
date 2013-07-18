// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.TreeView;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	/// <summary>
	/// Description of WorkspaceModel.
	/// </summary>
	public class WorkspaceModel : System.ComponentModel.INotifyPropertyChanged
	{
		IMutableModelCollection<SharpTreeNode> specialNodes;
		public IMutableModelCollection<SharpTreeNode> SpecialNodes {
			get { return specialNodes; }
		}
		
		AssemblyList assemblyList;

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		public AssemblyList AssemblyList {
			get {
				return assemblyList;
			}
			set {
				if (assemblyList != value) {
					assemblyList = value;
					OnPropertyChanged("AssemblyList");
				}
			}
		}
		public WorkspaceModel()
		{
			this.specialNodes = new SimpleModelCollection<SharpTreeNode>();
			this.AssemblyList = new AssemblyList();
		}
	}
}
