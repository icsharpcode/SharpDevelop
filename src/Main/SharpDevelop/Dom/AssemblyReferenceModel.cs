// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
