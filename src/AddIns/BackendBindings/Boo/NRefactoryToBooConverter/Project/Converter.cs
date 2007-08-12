// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using Boo.Lang.Compiler.Ast;
using ICSharpCode.NRefactory.Visitors;
using NR = ICSharpCode.NRefactory.Ast;

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
				cu.AcceptVisitor(new VBNetConstructsConvertVisitor { AddDefaultValueInitializerToLocalVariableDeclarations = false }, null);
			else
				cu.AcceptVisitor(new CSharpConstructsConvertVisitor(), null);
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
