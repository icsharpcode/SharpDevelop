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
		StringWriter writer;
		IList<Error> errors;
		IList<IUnresolvedTypeDefinition> topLevel;
		
		public static ILSpyUnresolvedFile Create(DecompiledTypeReference name, AstBuilder builder)
		{
			var writer = new StringWriter();
			var target = new TextWriterTokenWriter(writer) { IndentationString = "\t" };
			var output = new DebugInfoTokenWriterDecorator(target, target);
			builder.RunTransformations();
			var syntaxTree = builder.SyntaxTree;
			
			syntaxTree.AcceptVisitor(new InsertParenthesesVisitor { InsertParenthesesForReadability = true });
			var outputFormatter = TokenWriter.WrapInWriterThatSetsLocationsInAST(output);
			syntaxTree.AcceptVisitor(new CSharpOutputVisitor(outputFormatter, FormattingOptionsFactory.CreateSharpDevelop()));
			ILSpyUnresolvedFile file = new ILSpyUnresolvedFile(name, syntaxTree.Errors);
			builder.SyntaxTree.FileName = name.ToFileName();
			var ts = syntaxTree.ToTypeSystem();
			file.topLevel = ts.TopLevelTypeDefinitions;
			file.MemberLocations = output.MemberLocations;
			file.DebugSymbols = output.DebugSymbols;
			file.writer = writer;
			
			return file;
		}
		
		ILSpyUnresolvedFile(DecompiledTypeReference name, IList<Error> errors)
		{
			this.name = name;
			this.errors = errors;
		}
		
		public Dictionary<string, TextLocation> MemberLocations { get; private set; }
		
		public Dictionary<string, MethodDebugSymbols> DebugSymbols { get; private set; }

		public StringWriter Writer {
			get { return writer; }
		}
		
		public FileName AssemblyFile {
			get { return name.AssemblyFile; }
		}
		
		public IUnresolvedTypeDefinition GetTopLevelTypeDefinition(TextLocation location)
		{
			return FindEntity(topLevel, location);
		}
		
		public IUnresolvedTypeDefinition GetInnermostTypeDefinition(TextLocation location)
		{
			IUnresolvedTypeDefinition parent = null;
			IUnresolvedTypeDefinition type = GetTopLevelTypeDefinition(location);
			while (type != null) {
				parent = type;
				type = FindEntity(parent.NestedTypes, location);
			}
			return parent;
		}
		
		public IUnresolvedMember GetMember(TextLocation location)
		{
			IUnresolvedTypeDefinition type = GetInnermostTypeDefinition(location);
			if (type == null)
				return null;
			return FindEntity(type.Members, location);
		}
		
		static T FindEntity<T>(IList<T> list, TextLocation location) where T : class, IUnresolvedEntity
		{
			// This could be improved using a binary search
			foreach (T entity in list) {
				if (entity.Region.IsInside(location.Line, location.Column))
					return entity;
			}
			return null;
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
			get { return topLevel; }
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
