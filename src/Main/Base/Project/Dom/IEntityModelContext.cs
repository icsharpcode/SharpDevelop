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
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;

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
		ICompilation GetCompilation();
		
		/// <summary>
		/// Returns true if part1 is considered a better candidate for the primary part than part2.
		/// </summary>
		bool IsBetterPart(IUnresolvedTypeDefinition part1, IUnresolvedTypeDefinition part2);
		
		/// <summary>
		/// Short name of current assembly.
		/// </summary>
		string AssemblyName { get; }
		
		/// <summary>
		/// Gets the full assembly name (including public key token etc.)
		/// </summary>
		string FullAssemblyName { get; }
		
		/// <summary>
		/// Full path and file name of the assembly. Output assembly for projects.
		/// </summary>
		string Location { get; }
		
		/// <summary>
		/// Returns whether this is a valid context (based on a existing and readable definition).
		/// </summary>
		bool IsValid { get; }
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
		
		public string AssemblyName {
			get { return project.AssemblyName; }
		}
		
		public string FullAssemblyName {
			get {
				if (project.ProjectContent != null) {
					return project.ProjectContent.FullAssemblyName;
				}
				
				// TODO Better fallback for the case we can't get a FullAssemblyName?
				return project.AssemblyName;
			}
		}
		
		public string Location {
			get { return project.OutputAssemblyFullPath; }
		}
		
		public IProject Project {
			get { return project; }
		}
		
		public ICompilation GetCompilation()
		{
			return SD.ParserService.GetCompilation(project);
		}
		
		public bool IsBetterPart(IUnresolvedTypeDefinition part1, IUnresolvedTypeDefinition part2)
		{
			return EntityModelContextUtils.IsBetterPart(part1, part2, primaryCodeFileExtension);
		}
		
		public bool IsValid {
			get { return true; }
		}
	}
	
	public class AssemblyEntityModelContext : IEntityModelContext
	{
		Lazy<ICompilation> compilation;
		IUnresolvedAssembly mainAssembly;
		IAssemblyReference[] references;
		
		public AssemblyEntityModelContext(IUnresolvedAssembly mainAssembly, IAssemblyReference[] references)
		{
			if (mainAssembly == null)
				throw new ArgumentNullException("mainAssembly");
			this.mainAssembly = mainAssembly;
			this.references = references;
			this.compilation = new Lazy<ICompilation>(() => new SimpleCompilation(mainAssembly, references));
		}
		
		public string AssemblyName {
			get { return mainAssembly.AssemblyName; }
		}
		
		public string FullAssemblyName {
			get { return mainAssembly.FullAssemblyName; }
		}
		
		public string Location {
			get { return mainAssembly.Location; }
		}
		
		public ICompilation GetCompilation()
		{
			return compilation.Value;
		}
		
		public bool IsBetterPart(IUnresolvedTypeDefinition part1, IUnresolvedTypeDefinition part2)
		{
			return false;
		}
		
		public IProject Project {
			get { return null; }
		}
		
		public bool IsValid {
			get { return true; }
		}
	}
	
	public static class EntityModelContextUtils
	{
		public static bool IsBetterPart(IUnresolvedTypeDefinition part1, IUnresolvedTypeDefinition part2, string codeFileExtension)
		{
			IUnresolvedFile file1 = part1.UnresolvedFile;
			IUnresolvedFile file2 = part2.UnresolvedFile;
			if (file1 != null && file2 == null)
				return true;
			if (file1 == null)
				return false;
			bool file1HasExtension = file1.FileName.EndsWith(codeFileExtension, StringComparison.OrdinalIgnoreCase);
			bool file2HasExtension = file2.FileName.EndsWith(codeFileExtension, StringComparison.OrdinalIgnoreCase);
			if (file1HasExtension && !file2HasExtension)
				return true;
			if (!file1HasExtension && file2HasExtension)
				return false;
			return file1.FileName.Length < file2.FileName.Length;
		}
	}
}
