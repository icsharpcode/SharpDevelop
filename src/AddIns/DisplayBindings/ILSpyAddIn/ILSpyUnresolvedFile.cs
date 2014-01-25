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
