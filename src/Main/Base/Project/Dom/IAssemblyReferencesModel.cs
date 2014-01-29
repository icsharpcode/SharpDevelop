// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Base interface for list of assembly references.
	/// </summary>
	public interface IAssemblyReferencesModel
	{
		IModelCollection<IAssemblyReferenceModel> AssemblyNames { get; }
		IAssemblyModel ParentAssemblyModel { get; }
	}
	
	/// <summary>
	/// Implements an empty assembly references model.
	/// </summary>
	public class EmptyAssemblyReferencesModel : IAssemblyReferencesModel
	{
		public static readonly IAssemblyReferencesModel Instance = new EmptyAssemblyReferencesModel();

		private static readonly SimpleModelCollection<IAssemblyReferenceModel> EmptyReferenceCollection =
			new SimpleModelCollection<IAssemblyReferenceModel>();
		
		public IModelCollection<IAssemblyReferenceModel> AssemblyNames {
			get {
				return EmptyReferenceCollection;
			}
		}
		
		public IAssemblyModel ParentAssemblyModel
		{
			get {
				return EmptyAssemblyModel.Instance;
			}
		}
	}
}
