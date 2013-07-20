// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.ILSpyAddIn
{
	/// <summary>
	/// Description of ILSpyUnresolvedFile.
	/// </summary>
	public class ILSpyUnresolvedFile : IUnresolvedFile
	{
		DecompiledTypeReference name;
		DebuggerTextOutput textOutput;
		StringWriter writer;
		IList<Error> errors;
		IList<IUnresolvedTypeDefinition> topLevel;
		
		public static ILSpyUnresolvedFile Create(DecompiledTypeReference name, AstBuilder builder)
		{
			var writer = new StringWriter();
			var output = new DebuggerTextOutput(new PlainTextOutput(writer));
			builder.GenerateCode(output);
			ILSpyUnresolvedFile file = new ILSpyUnresolvedFile(name, builder.SyntaxTree.Errors);
			builder.SyntaxTree.FileName = name.ToFileName();
			var ts = builder.SyntaxTree.ToTypeSystem();
			file.topLevel = ts.TopLevelTypeDefinitions;
			file.textOutput = output;
			file.writer = writer;
			
			return file;
		}
		
		ILSpyUnresolvedFile(DecompiledTypeReference name, IList<Error> errors)
		{
			this.name = name;
			this.errors = errors;
		}
		
		public DebuggerTextOutput TextOutput {
			get { return textOutput; }
		}
		
		public StringWriter Writer {
			get { return writer; }
		}

		public FileName AssemblyFile {
			get { return name.AssemblyFile; }
		}
		
		public Dictionary<string, MethodDebugSymbols> DebugSymbols { get; private set; }
		
		public IUnresolvedTypeDefinition GetTopLevelTypeDefinition(TextLocation location)
		{
			throw new NotImplementedException();
		}
		public IUnresolvedTypeDefinition GetInnermostTypeDefinition(TextLocation location)
		{
			throw new NotImplementedException();
		}
		public IUnresolvedMember GetMember(TextLocation location)
		{
			throw new NotImplementedException();
		}
		
		public string FileName {
			get { return name.ToFileName(); }
		}
		
		public DateTime? LastWriteTime {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public IList<IUnresolvedTypeDefinition> TopLevelTypeDefinitions {
			get {
				return topLevel;
			}
		}
		
		public IList<IUnresolvedAttribute> AssemblyAttributes {
			get {
				throw new NotImplementedException();
			}
		}
		public IList<IUnresolvedAttribute> ModuleAttributes {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IList<Error> Errors {
			get { return errors; }
		}
	}
}
