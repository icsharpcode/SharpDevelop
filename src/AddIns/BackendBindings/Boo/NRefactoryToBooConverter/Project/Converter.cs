#region license
// Copyright (c) 2005, Daniel Grunwald (daniel@danielgrunwald.de)
// All rights reserved.
//
// NRefactoryToBoo is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// NRefactoryToBoo is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with NRefactoryToBoo; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion

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
