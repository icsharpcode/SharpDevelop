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
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.ILSpyAddIn
{
	/// <summary>
	/// Description of ILSpyUnresolvedFile.
	/// </summary>
	public class ILSpyUnresolvedFile : CSharpUnresolvedFile
	{
		DecompiledTypeReference name;
		string output;
		
		public static ILSpyUnresolvedFile Create(DecompiledTypeReference name, AstBuilder builder)
		{
			var writer = new StringWriter();
			var target = new TextWriterTokenWriter(writer) { IndentationString = "\t" };
			var output = new DebugInfoTokenWriterDecorator(TokenWriter.WrapInWriterThatSetsLocationsInAST(target));
			builder.RunTransformations();
			var syntaxTree = builder.SyntaxTree;
			
			syntaxTree.AcceptVisitor(new InsertParenthesesVisitor { InsertParenthesesForReadability = true });
			syntaxTree.AcceptVisitor(new CSharpOutputVisitor(output, FormattingOptionsFactory.CreateSharpDevelop()));
			ILSpyUnresolvedFile file = new ILSpyUnresolvedFile(name);
			var v = new TypeSystemConvertVisitor(file);
			syntaxTree.AcceptVisitor(v);
	
			file.MemberLocations = output.MemberLocations;
			file.DebugSymbols = output.DebugSymbols;
			file.output = writer.ToString();
			
			return file;
		}
		
		ILSpyUnresolvedFile(DecompiledTypeReference name)
		{
			this.name = name;
			FileName = name.ToFileName();
		}
		
		public Dictionary<string, TextLocation> MemberLocations { get; private set; }
		
		public Dictionary<string, MethodDebugSymbols> DebugSymbols { get; private set; }

		public string Output {
			get { return output; }
		}
		
		public FileName AssemblyFile {
			get { return name.AssemblyFile; }
		}
	}
}
