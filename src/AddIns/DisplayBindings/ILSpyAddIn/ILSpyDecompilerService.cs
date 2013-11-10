// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
using ICSharpCode.SharpDevelop.Parser;
using Mono.Cecil;

namespace ICSharpCode.ILSpyAddIn
{
	/// <summary>
	/// Description of DecompilerService.
	/// </summary>
	public static class ILSpyDecompilerService
	{
		class AssemblyCacheInfo
		{
			public readonly DateTime LastUpdateTime;
			public readonly WeakReference<AssemblyDefinition> Assembly;
			
			public AssemblyCacheInfo(DateTime lastUpdateTime, AssemblyDefinition assembly)
			{
				if (assembly == null)
					throw new ArgumentNullException("assembly");
				this.LastUpdateTime = lastUpdateTime;
				this.Assembly = new WeakReference<AssemblyDefinition>(assembly);
			}
		}
		
		static readonly Dictionary<FileName, AssemblyCacheInfo> assemblyCache = new Dictionary<FileName, AssemblyCacheInfo>();
		
		static AssemblyDefinition GetAssemblyDefinitionFromCache(FileName file, ReaderParameters parameters = null)
		{
			if (file == null) return null;
			if (parameters == null) parameters = new ReaderParameters();
			var lastUpdateTime = File.GetLastWriteTimeUtc(file);
			lock (assemblyCache) {
				AssemblyCacheInfo info;
				AssemblyDefinition asm;
				if (!assemblyCache.TryGetValue(file, out info)) {
					asm = AssemblyDefinition.ReadAssembly(file, parameters);
					assemblyCache.Add(file, new AssemblyCacheInfo(lastUpdateTime, asm));
					return asm;
				} else if (info.LastUpdateTime < lastUpdateTime) {
					assemblyCache.Remove(file);
					asm = AssemblyDefinition.ReadAssembly(file, parameters);
					assemblyCache.Add(file, new AssemblyCacheInfo(lastUpdateTime, asm));
					return asm;
				} else {
					if (info.Assembly.TryGetTarget(out asm))
						return asm;
					asm = AssemblyDefinition.ReadAssembly(file, parameters);
					info.Assembly.SetTarget(asm);
					return asm;
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
			
			readonly ISet<AssemblyDefinition> resolvedAssemblies = new HashSet<AssemblyDefinition>();
			
			AssemblyDefinition Resolve(DomAssemblyName name, ReaderParameters parameters)
			{
				var assemblyDefinition = GetAssemblyDefinitionFromCache(FindAssembly(name), parameters);
				if (assemblyDefinition != null)
					resolvedAssemblies.Add(assemblyDefinition);
				return assemblyDefinition;
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
		
		public static ILSpyUnresolvedFile DecompileType(DecompiledTypeReference name)
		{
			if (name == null)
				throw new ArgumentNullException("entity");
			return DoDecompile(name);
		}
		
		public static async Task<ILSpyUnresolvedFile> DecompileTypeAsync(DecompiledTypeReference name, CancellationToken cancellationToken)
		{
			return await Task.Run(
				delegate() { return DoDecompile(name, cancellationToken); },
				cancellationToken);
		}
		
		static AstBuilder CreateAstBuilder(DecompiledTypeReference name, out AssemblyDefinition[] neededAssemblies, CancellationToken cancellationToken = default(CancellationToken))
		{
			ReaderParameters readerParameters = new ReaderParameters();
			// Use new assembly resolver instance so that the AssemblyDefinitions
			// can be garbage-collected once the code is decompiled.
			var resolver = new ILSpyAssemblyResolver(name.AssemblyFile);
			readerParameters.AssemblyResolver = resolver;
			AssemblyDefinition asm = GetAssemblyDefinitionFromCache(name.AssemblyFile, readerParameters);
			if (asm == null)
				throw new InvalidOperationException("Could not find assembly file");
			ModuleDefinition module = asm.MainModule;
			TypeDefinition typeDefinition = module.GetType(name.Type.ReflectionName);
			if (typeDefinition == null)
				throw new InvalidOperationException("Could not find type");
			DecompilerContext context = new DecompilerContext(module);
			context.CancellationToken = cancellationToken;
			AstBuilder astBuilder = new AstBuilder(context);
			astBuilder.AddType(typeDefinition);
			neededAssemblies = resolver.ResolvedAssemblies.ToArray();
			return astBuilder;
		}
		
		static ILSpyUnresolvedFile DoDecompile(DecompiledTypeReference name, CancellationToken cancellationToken = default(CancellationToken))
		{
			AssemblyDefinition[] assemblies;
			return ILSpyUnresolvedFile.Create(name, CreateAstBuilder(name, out assemblies, cancellationToken));
		}
		
		public static ILSpyFullParseInformation ParseDecompiledType(DecompiledTypeReference name, CancellationToken cancellationToken = default(CancellationToken))
		{
			AssemblyDefinition[] assemblies;
			var astBuilder = CreateAstBuilder(name, out assemblies, cancellationToken);
			return new ILSpyFullParseInformation(ILSpyUnresolvedFile.Create(name, astBuilder), null, astBuilder.SyntaxTree, assemblies);
		}
	}
	
	public class DecompiledTypeReference : IEquatable<DecompiledTypeReference>
	{
		public FileName AssemblyFile { get; private set; }
		public FullTypeName Type { get; private set; }
		
		public DecompiledTypeReference(FileName assemblyFile, FullTypeName type)
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
			
			return new DecompiledTypeReference(new FileName(asm), new FullTypeName(typeName));
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
			foreach (var ch in Path.GetInvalidFileNameChars().Concat(new[] { '_' })) {
				typeName = unescapeRegex.Replace(typeName, m => ((char)int.Parse(m.Groups[1].Value, System.Globalization.NumberStyles.HexNumber)).ToString());
			}
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
