// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.NRefactory.Parser;
using NR = ICSharpCode.NRefactory.Parser.Ast;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;

namespace NRefactoryToBooConverter
{
	/// <summary>
	/// Class with static conversion methods.
	/// </summary>
	public static class Converter
	{
		public static Module Convert(NR.CompilationUnit cu, ConverterSettings settings)
		{
			if (cu == null)
				throw new ArgumentNullException("cu");
			if (settings == null)
				throw new ArgumentNullException("settings");
			if (settings.IsVisualBasic)
				cu.AcceptVisitor(new VBNetConstructsConvertVisitor(), null);
			else
				cu.AcceptVisitor(new CSharpConstructsVisitor(), null);
			return (Module)cu.AcceptVisitor(new ConvertVisitor(settings), null);
		}
		
		public static Expression Convert(NR.Expression expression, ConverterSettings settings)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");
			if (settings == null)
				throw new ArgumentNullException("settings");
			return (Expression)expression.AcceptVisitor(new ConvertVisitor(settings), null);
		}
	}
}
