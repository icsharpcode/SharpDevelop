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
		IMutableModelCollection<IAssemblyList> assemblyLists;
		public IMutableModelCollection<IAssemblyList> AssemblyLists {
			get { return assemblyLists; }
		}
		
		IAssemblyList mainAssemblyList;

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		public IAssemblyList MainAssemblyList {
			get {
				return mainAssemblyList;
			}
			set {
				if (mainAssemblyList != value) {
					mainAssemblyList = value;
					OnPropertyChanged("AssemblyList");
				}
			}
		}
		public WorkspaceModel()
		{
			this.assemblyLists = new SimpleModelCollection<IAssemblyList>();
			this.MainAssemblyList = new AssemblyList();
		}
	}
}
