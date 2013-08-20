// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ICSharpCode.Core;
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

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
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
					OnPropertyChanged();
				}
			}
		}
		
		public IAssemblyModel FindAssemblyModel(FileName fileName)
		{
			foreach (var list in assemblyLists) {
				var model = list.Assemblies.FirstOrDefault(m => m.Location == fileName);
				if (model != null)
					return model;
			}
			return mainAssemblyList.Assemblies.FirstOrDefault(m => m.Location == fileName);
		}
		
		public WorkspaceModel()
		{
			this.assemblyLists = new SimpleModelCollection<IAssemblyList>();
			this.MainAssemblyList = new AssemblyList();
		}
	}
}
