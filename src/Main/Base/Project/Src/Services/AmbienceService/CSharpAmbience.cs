// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// C# ambience.
	/// </summary>
	public class CSharpAmbience : IAmbience
	{
		public ConversionFlags ConversionFlags { get; set; }
		
		public string ConvertEntity(IEntity e)
		{
			using (var ctx = ParserService.CurrentTypeResolveContext.Synchronize()) {
				TypeSystemAstBuilder astBuilder = new TypeSystemAstBuilder(ctx);
				var astNode = astBuilder.ConvertEntity(e);
				CSharpFormattingOptions formatting = new CSharpFormattingOptions();
				StringWriter writer = new StringWriter();
				astNode.AcceptVisitor(new OutputVisitor(writer, formatting), null);
				return writer.ToString().TrimEnd();
			}
		}
		
		public string ConvertVariable(IVariable v)
		{
			using (var ctx = ParserService.CurrentTypeResolveContext.Synchronize()) {
				TypeSystemAstBuilder astBuilder = new TypeSystemAstBuilder(ctx);
				AstNode astNode = astBuilder.ConvertVariable(v);
				CSharpFormattingOptions formatting = new CSharpFormattingOptions();
				StringWriter writer = new StringWriter();
				astNode.AcceptVisitor(new OutputVisitor(writer, formatting), null);
				return writer.ToString();
			}
		}
		
		public string ConvertType(IType type)
		{
			using (var ctx = ParserService.CurrentTypeResolveContext.Synchronize()) {
				TypeSystemAstBuilder astBuilder = new TypeSystemAstBuilder(ctx);
				AstType astType = astBuilder.ConvertType(type);
				CSharpFormattingOptions formatting = new CSharpFormattingOptions();
				StringWriter writer = new StringWriter();
				astType.AcceptVisitor(new OutputVisitor(writer, formatting), null);
				return writer.ToString();
			}
		}
		
		public string WrapAttribute(string attribute)
		{
			return "[" + attribute + "]";
		}
		
		public string WrapComment(string comment)
		{
			return "// " + comment;
		}
	}
}
