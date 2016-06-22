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
