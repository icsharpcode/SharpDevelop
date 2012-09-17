// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// The context for an entity model.
	/// This may be a reference to a project, or a compilation provider for a single stand-alone code file.
	/// </summary>
	public interface IEntityModelContext
	{
		/// <summary>
		/// Used for <see cref="IEntityModel.ParentProject"/>.
		/// </summary>
		IProject Project { get; }
		
		/// <summary>
		/// Used for <see cref="IEntityModel.Resolve()"/>.
		/// </summary>
		/// <param name="solutionSnapshot">
		/// The solution snapshot provided to <see cref="IEntityModel.Resolve(ISolutionSnapshotWithProjectMapping)"/>,
		/// or null if the <see cref="IEntityModel.Resolve()"/> overload was used.
		/// </param>
		ICompilation GetCompilation(ISolutionSnapshotWithProjectMapping solutionSnapshot);
		
		/// <summary>
		/// Returns true if part1 is considered a better candidate for the primary part than part2.
		/// </summary>
		bool IsBetterPart(IUnresolvedTypeDefinition part1, IUnresolvedTypeDefinition part2);
	}
	
	public class ProjectEntityModelContext : IEntityModelContext
	{
		readonly IProject project;
		readonly string primaryCodeFileExtension;
		
		public ProjectEntityModelContext(IProject project, string primaryCodeFileExtension)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			this.project = project;
			this.primaryCodeFileExtension = primaryCodeFileExtension;
		}
		
		public IProject Project {
			get { return project; }
		}
		
		public ICompilation GetCompilation(ISolutionSnapshotWithProjectMapping solutionSnapshot)
		{
			if (solutionSnapshot != null)
				return solutionSnapshot.GetCompilation(project);
			else
				return SD.ParserService.GetCompilation(project);
		}
		
		public bool IsBetterPart(IUnresolvedTypeDefinition part1, IUnresolvedTypeDefinition part2)
		{
			IUnresolvedFile file1 = part1.UnresolvedFile;
			IUnresolvedFile file2 = part2.UnresolvedFile;
			if (file1 != null && file2 == null)
				return true;
			if (file1 == null)
				return false;
			bool file1HasExtension = file1.FileName.EndsWith(primaryCodeFileExtension, StringComparison.OrdinalIgnoreCase);
			bool file2HasExtension = file2.FileName.EndsWith(primaryCodeFileExtension, StringComparison.OrdinalIgnoreCase);
			if (file1HasExtension && !file2HasExtension)
				return true;
			if (!file1HasExtension && file2HasExtension)
				return false;
			return file1.FileName.Length < file2.FileName.Length;
		}
	}
}
