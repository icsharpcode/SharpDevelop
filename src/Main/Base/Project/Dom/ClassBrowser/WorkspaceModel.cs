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
		IAssemblyList unpinnedAssemblies;

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
		
		public IAssemblyList UnpinnedAssemblies {
			get {
				return unpinnedAssemblies;
			}
			set {
				if (unpinnedAssemblies != value) {
					unpinnedAssemblies = value;
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
			var modelInMainAssemblyList = mainAssemblyList.Assemblies.FirstOrDefault(m => m.Location == fileName);
			if (modelInMainAssemblyList != null)
				return modelInMainAssemblyList;
			return unpinnedAssemblies.Assemblies.FirstOrDefault(m => m.Location == fileName);
		}
		
		public WorkspaceModel()
		{
			this.assemblyLists = new SimpleModelCollection<IAssemblyList>();
			this.MainAssemblyList = new AssemblyList();
			this.UnpinnedAssemblies = new AssemblyList();
		}
	}
}
