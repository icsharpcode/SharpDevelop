// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Core;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using Mono.Cecil;

namespace ICSharpCode.ILSpyAddIn
{
	/// <summary>
	/// Description of DecompilerService.
	/// </summary>
	public class ILSpyDecompilerService
	{
		public static ILSpyUnresolvedFile DecompileType(DecompiledTypeReference name)
		{
			if (name == null)
				throw new ArgumentNullException("entity");
			return DoDecompile(name);
		}
		
		public static async Task<ILSpyUnresolvedFile> DecompileTypeAsync(DecompiledTypeReference name, CancellationToken cancellationToken)
		{
			return Task.Run(
				delegate() { return DoDecompile(name, cancellationToken); },
				cancellationToken).Result;
		}
		
		static AstBuilder CreateAstBuilder(DecompiledTypeReference name, CancellationToken cancellationToken = default(CancellationToken))
		{
			ReaderParameters readerParameters = new ReaderParameters();
			// Use new assembly resolver instance so that the AssemblyDefinitions
			// can be garbage-collected once the code is decompiled.
			readerParameters.AssemblyResolver = new ILSpyAssemblyResolver(Path.GetDirectoryName(name.AssemblyFile));
			
			ModuleDefinition module = ModuleDefinition.ReadModule(name.AssemblyFile, readerParameters);
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
		
		public static ILSpyFullParseInformation ParseDecompiledType(DecompiledTypeReference name, CancellationToken cancellationToken = default(CancellationToken))
		{
			var astBuilder = CreateAstBuilder(name, cancellationToken);
			return new ILSpyFullParseInformation(ILSpyUnresolvedFile.Create(name, astBuilder), null, astBuilder.SyntaxTree);
		}
	}
	
	public class DecompiledTypeReference
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
			return FileName.Create("ilspy://" + AssemblyFile + "/" + Type.ReflectionName + ".cs");
		}
		
		static readonly Regex nameRegex = new Regex(@"^ilspy\://(.+)/(.+)\.cs$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		
		public static DecompiledTypeReference FromFileName(string filename)
		{
			var match = nameRegex.Match(filename);
			if (!match.Success) return null;
			
			string asm, typeName;
			asm = match.Groups[1].Value;
			typeName = match.Groups[2].Value;
			
			return new DecompiledTypeReference(new FileName(asm), new FullTypeName(typeName));
		}
	}
}
