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
using System.Collections.Generic;
using System.IO;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using ICSharpCode.NRefactory.Parser;

namespace NRefactoryToBooConverter
{
	/// <summary>
	/// Class with static methods to parse C# and VB code.
	/// </summary>
	public static class Parser
	{
		public static Module ParseModule(CompileUnit cu, TextReader input, ConverterSettings settings)
		{
			IList<ISpecial> specials;
			return ParseModule(cu, input, settings, out specials);
		}
		
		public static Module ParseModule(CompileUnit cu, TextReader input, ConverterSettings settings, out IList<ISpecial> specials)
		{
			if (cu == null)
				throw new ArgumentNullException("cu");
			if (input == null)
				throw new ArgumentNullException("input");
			if (settings == null)
				throw new ArgumentNullException("settings");
			IParser parser = ParserFactory.CreateParser(settings.IsVisualBasic ? SupportedLanguage.VBNet : SupportedLanguage.CSharp, input);
			ErrorTrap errorTrap = new ErrorTrap(settings);
			parser.Errors.SemErr = errorTrap.DefaultCodeError;
			parser.Errors.SynErr = errorTrap.DefaultCodeError;
			parser.Errors.Error  = errorTrap.DefaultMsgError;
			parser.Parse();
			specials = parser.Lexer.SpecialTracker.CurrentSpecials;
			// abort when file has errors
			if (errorTrap.count > 0)
				return null;
			Module m = Converter.Convert(parser.CompilationUnit, settings);
			if (m != null && cu != null) {
				cu.Modules.Add(m);
			}
			return m;
		}
		
		internal class ErrorTrap
		{
			CompilerErrorCollection errors;
			string fileName;
			internal int count;
			
			internal ErrorTrap(ConverterSettings settings)
			{
				this.errors = settings.Errors;
				this.fileName = settings.FileName;
			}
			
			internal void DefaultCodeError(int line, int col, int n)
			{
				errors.Add(new CompilerError(new LexicalInfo(fileName, line, col), "Error number " + n));
				count++;
			}
			
			internal void DefaultMsgError(int line, int col, string s)
			{
				errors.Add(new CompilerError(new LexicalInfo(fileName, line, col), s));
				count++;
			}
		}
	}
}
