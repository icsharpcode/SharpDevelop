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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Core;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Parser;
using Mono.Cecil;

namespace ICSharpCode.ILSpyAddIn
{
	/// <summary>
	/// Description of DecompilerService.
	/// </summary>
	public static class ILSpyDecompilerService
	{
		class ModuleCacheInfo
		{
			public readonly DateTime LastUpdateTime;
			public readonly WeakReference<ModuleDefinition> Module;
			
			public ModuleCacheInfo(DateTime lastUpdateTime, ModuleDefinition assembly)
			{
				if (assembly == null)
					throw new ArgumentNullException("assembly");
				this.LastUpdateTime = lastUpdateTime;
				this.Module = new WeakReference<ModuleDefinition>(assembly);
			}
		}
		
		static readonly Dictionary<FileName, ModuleCacheInfo> moduleCache = new Dictionary<FileName, ModuleCacheInfo>();
		
		static ModuleDefinition GetModuleDefinitionFromCache(FileName file)
		{
			if (file == null) return null;
			ReaderParameters parameters = new ReaderParameters();
			parameters.AssemblyResolver = new ILSpyAssemblyResolver(file);
			var lastUpdateTime = File.GetLastWriteTimeUtc(file);
			lock (moduleCache) {
				ModuleCacheInfo info;
				ModuleDefinition module;
				if (!moduleCache.TryGetValue(file, out info)) {
					module = ModuleDefinition.ReadModule(file, parameters);
					moduleCache.Add(file, new ModuleCacheInfo(lastUpdateTime, module));
					return module;
				} else if (info.LastUpdateTime < lastUpdateTime) {
					moduleCache.Remove(file);
					module = ModuleDefinition.ReadModule(file, parameters);
					moduleCache.Add(file, new ModuleCacheInfo(lastUpdateTime, module));
					return module;
				} else {
					if (info.Module.TryGetTarget(out module))
						return module;
					module = ModuleDefinition.ReadModule(file, parameters);
					info.Module.SetTarget(module);
					return module;
				}
			}
		}
		
		class ILSpyAssemblyResolver : DefaultAssemblySearcher, IAssemblyResolver
		{
			public ISet<AssemblyDefinition> ResolvedAssemblies {
				get { return resolvedAssemblies; }
			}

			public ILSpyAssemblyResolver(FileName fileName)
				: base(fileName)
			{
			}
			
			/// <summary>
			/// Used to remember the referenced assemblies for the WeakReference cache policy.
			/// </summary>
			readonly ISet<AssemblyDefinition> resolvedAssemblies = new HashSet<AssemblyDefinition>();
			
			AssemblyDefinition Resolve(DomAssemblyName name, ReaderParameters parameters)
			{
				var moduleDefinition = GetModuleDefinitionFromCache(FindAssembly(name));
				if (moduleDefinition != null) {
					resolvedAssemblies.Add(moduleDefinition.Assembly);
					return moduleDefinition.Assembly;
				}
				return null;
			}
			
			public AssemblyDefinition Resolve(AssemblyNameReference name)
			{
				return Resolve(name, new ReaderParameters());
			}
			
			public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
			{
				return Resolve(new DomAssemblyName(name.FullName), parameters);
			}
			
			public AssemblyDefinition Resolve(string fullName)
			{
				return Resolve(fullName, new ReaderParameters());
			}
			
			public AssemblyDefinition Resolve(string fullName, ReaderParameters parameters)
			{
				return Resolve(new DomAssemblyName(fullName), parameters);
			}
		}
		
		public static ILSpyFullParseInformation DecompileType(DecompiledTypeReference name, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (name == null)
				throw new ArgumentNullException("name");
			var astBuilder = CreateAstBuilder(name, cancellationToken);
			return new ILSpyFullParseInformation(ILSpyUnresolvedFile.Create(name, astBuilder), null, astBuilder.SyntaxTree);
		}
		
		static AstBuilder CreateAstBuilder(DecompiledTypeReference name, CancellationToken cancellationToken = default(CancellationToken))
		{
			ModuleDefinition module = GetModuleDefinitionFromCache(name.AssemblyFile);
			if (module == null)
				throw new InvalidOperationException("Could not find assembly file");
			TypeDefinition typeDefinition = module.GetType(name.Type.ReflectionName);
			if (typeDefinition == null)
				throw new InvalidOperationException("Could not find type");
			DecompilerContext context = new DecompilerContext(module);
			context.CancellationToken = cancellationToken;
			AstBuilder astBuilder = new AstBuilder(context);
			astBuilder.AddType(typeDefinition);
			return astBuilder;
		}
		
		static ILSpyUnresolvedFile DoDecompile(DecompiledTypeReference name, CancellationToken cancellationToken = default(CancellationToken))
		{
			return ILSpyUnresolvedFile.Create(name, CreateAstBuilder(name, cancellationToken));
		}
	}
	
	public class DecompiledTypeReference : IEquatable<DecompiledTypeReference>
	{
		public FileName AssemblyFile { get; private set; }
		public TopLevelTypeName Type { get; private set; }
		
		public DecompiledTypeReference(FileName assemblyFile, TopLevelTypeName type)
		{
			this.AssemblyFile = assemblyFile;
			this.Type = type;
		}
		
		public FileName ToFileName()
		{
			return FileName.Create("ilspy://" + AssemblyFile + "/" + EscapeTypeName(Type.ReflectionName) + ".cs");
		}
		
		static readonly Regex nameRegex = new Regex(@"^ilspy\://(.+)/(.+)\.cs$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		
		public static DecompiledTypeReference FromFileName(string filename)
		{
			var match = nameRegex.Match(filename);
			if (!match.Success) return null;
			
			string asm, typeName;
			asm = match.Groups[1].Value;
			typeName = UnescapeTypeName(match.Groups[2].Value);
			
			return new DecompiledTypeReference(new FileName(asm), new TopLevelTypeName(typeName));
		}
		
		public static DecompiledTypeReference FromTypeDefinition(ITypeDefinition definition)
		{
			FileName assemblyLocation = definition.ParentAssembly.GetRuntimeAssemblyLocation();
			if (assemblyLocation != null && SD.FileSystem.FileExists(assemblyLocation)) {
				return new DecompiledTypeReference(assemblyLocation, definition.FullTypeName.TopLevelTypeName);
			}
			return null;
		}
		
		public static string EscapeTypeName(string typeName)
		{
			if (typeName == null)
				throw new ArgumentNullException("typeName");
			foreach (var ch in new[] { '_' }.Concat(Path.GetInvalidFileNameChars())) {
				typeName = typeName.Replace(ch.ToString(), string.Format("_{0:X4}", (int)ch));
			}
			return typeName;
		}
		
		static readonly Regex unescapeRegex = new Regex(@"_([0-9A-F]{4})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		
		public static string UnescapeTypeName(string typeName)
		{
			if (typeName == null)
				throw new ArgumentNullException("typeName");
			typeName = unescapeRegex.Replace(typeName, m => ((char)int.Parse(m.Groups[1].Value, System.Globalization.NumberStyles.HexNumber)).ToString());
			return typeName;
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			DecompiledTypeReference other = (DecompiledTypeReference)obj;
			if (other == null)
				return false;
			return Equals(other);
		}
		
		public bool Equals(DecompiledTypeReference other)
		{
			return object.Equals(this.AssemblyFile, other.AssemblyFile) && this.Type == other.Type;
		}

		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (AssemblyFile != null)
					hashCode += 1000000007 * AssemblyFile.GetHashCode();
				hashCode += 1000000009 * Type.GetHashCode();
			}
			return hashCode;
		}

		public static bool operator ==(DecompiledTypeReference lhs, DecompiledTypeReference rhs) {
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}

		public static bool operator !=(DecompiledTypeReference lhs, DecompiledTypeReference rhs) {
			return !(lhs == rhs);
		}
		#endregion
	}
}
