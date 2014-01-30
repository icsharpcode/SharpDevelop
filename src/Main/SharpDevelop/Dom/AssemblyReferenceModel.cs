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
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Model representing list of assembly references.
	/// </summary>
	class AssemblyReferencesModel : IAssemblyReferencesModel
	{
		IAssemblyModel parentAssemblyModel;
		NullSafeSimpleModelCollection<IAssemblyReferenceModel> assemblyNames;
		
		public AssemblyReferencesModel(IAssemblyModel parentAssemblyModel)
		{
			if (parentAssemblyModel == null)
				throw new ArgumentNullException("parentAssemblyModel");
			
			assemblyNames = new NullSafeSimpleModelCollection<IAssemblyReferenceModel>();
			this.parentAssemblyModel = parentAssemblyModel;
		}
		
		public IModelCollection<IAssemblyReferenceModel> AssemblyNames
		{
			get {
				return assemblyNames;
			}
		}
		
		public IAssemblyModel ParentAssemblyModel {
			get {
				return parentAssemblyModel;
			}
		}
		
		public void Update(IReadOnlyList<DomAssemblyName> references)
		{
			assemblyNames.Clear();
			if (references != null) {
				assemblyNames.AddRange(references.Select(r => new AssemblyReferenceModel(parentAssemblyModel, r)));
			}
		}
	}
	
	/// <summary>
	/// Model representing an assembly reference.
	/// </summary>
	class AssemblyReferenceModel : IAssemblyReferenceModel
	{
		IAssemblyModel parentAssemblyModel;
		DomAssemblyName assemblyName;
		
		public AssemblyReferenceModel(IAssemblyModel parentAssemblyModel, DomAssemblyName assemblyName)
		{
			if (parentAssemblyModel == null)
				throw new ArgumentNullException("parentAssemblyModel");
			if (assemblyName == null)
				throw new ArgumentNullException("assemblyName");
			
			this.parentAssemblyModel = parentAssemblyModel;
			this.assemblyName = assemblyName;
		}

		public DomAssemblyName AssemblyName {
			get {
				return assemblyName;
			}
		}

		public IAssemblyModel ParentAssemblyModel {
			get {
				return parentAssemblyModel;
			}
		}
	}
}
