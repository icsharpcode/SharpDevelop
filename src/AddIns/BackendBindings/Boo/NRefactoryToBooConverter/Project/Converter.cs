// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using NR = ICSharpCode.NRefactory.Parser.AST;
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
			cu.AcceptVisitor(new RefactoryVisitor(), null);
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
