/*
 * Created by SharpDevelop.
 * User: Daniel
 * Date: 10/30/2011
 * Time: 01:02
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp
{
	public class CSharpProjectContent : IProjectContent
	{
		public IEnumerable<IParsedFile> Files {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IEnumerable<IAssemblyReference> AssemblyReferences {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string AssemblyName {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IEnumerable<IUnresolvedAttribute> AssemblyAttributes {
			get {
				return this.Files.SelectMany(f => f.AssemblyAttributes);
			}
		}
		
		public IEnumerable<IUnresolvedAttribute> ModuleAttributes {
			get {
				return this.Files.SelectMany(f => f.ModuleAttributes);
			}
		}
		
		public IEnumerable<IUnresolvedTypeDefinition> TopLevelTypeDefinitions {
			get {
				return this.Files.SelectMany(f => f.TopLevelTypeDefinitions);
			}
		}
		
		public IParsedFile GetFile(string fileName)
		{
			throw new NotImplementedException();
		}
		
		public ICompilation CreateCompilation()
		{
			throw new NotImplementedException();
		}
		
		public IProjectContent AddAssemblyReferences(IEnumerable<IAssemblyReference> references)
		{
			throw new NotImplementedException();
		}
		
		public IProjectContent RemoveAssemblyReferences(IEnumerable<IAssemblyReference> references)
		{
			throw new NotImplementedException();
		}
		
		public IProjectContent UpdateProjectContent(IParsedFile oldFile, IParsedFile newFile)
		{
			throw new NotImplementedException();
		}
		
		public IProjectContent UpdateProjectContent(IEnumerable<IParsedFile> oldFiles, IEnumerable<IParsedFile> newFiles)
		{
			throw new NotImplementedException();
		}
		
		IAssembly IAssemblyReference.Resolve(ITypeResolveContext context)
		{
			throw new NotImplementedException();
		}
	}
}
