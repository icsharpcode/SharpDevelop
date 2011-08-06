// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ICSharpCode.NRefactory.TypeSystem
{
	/// <summary>
	/// Mutable container of all classes in an assembly.
	/// </summary>
	#if WITH_CONTRACTS
	[ContractClass(typeof(IProjectContentContract))]
	#endif
	public interface IProjectContent : ITypeResolveContext, IAnnotatable
	{
		/// <summary>
		/// Gets the list of all assembly attributes in the project.
		/// </summary>
		IList<IAttribute> AssemblyAttributes { get; }
		
		/// <summary>
		/// Gets the list of all module attributes in the project.
		/// </summary>
		IList<IAttribute> ModuleAttributes { get; }
		
		/// <summary>
		/// Gets a parsed file by its file name.
		/// </summary>
		IParsedFile GetFile(string fileName);
		
		/// <summary>
		/// Gets the list of all parsed files in the project content.
		/// </summary>
		IEnumerable<IParsedFile> Files { get; }
		
		/// <summary>
		/// Removes types and attributes from oldFile from the project, and adds those from newFile.
		/// </summary>
		/// <remarks>
		/// It is not allowed to call this method from within a <c>using (var ctx = context.Synchronize())</c> block
		/// that involves this project content: this method is a write operation and might (if implemented using locks)
		/// wait until all read operations have finished, causing deadlocks within Synchronize blocks.
		/// </remarks>
		void UpdateProjectContent(IParsedFile oldFile, IParsedFile newFile);
	}
	
	#if WITH_CONTRACTS
	[ContractClassFor(typeof(IProjectContent))]
	abstract class IProjectContentContract : ITypeResolveContextContract, IProjectContent
	{
		IList<IAttribute> IProjectContent.AssemblyAttributes {
			get {
				Contract.Ensures(Contract.Result<IList<IAttribute>>() != null);
				return null;
			}
		}
		
		IParsedFile IProjectContent.GetFile(string fileName)
		{
			Contract.Requires(fileName != null);
			return;
		}
	}
	#endif
}
