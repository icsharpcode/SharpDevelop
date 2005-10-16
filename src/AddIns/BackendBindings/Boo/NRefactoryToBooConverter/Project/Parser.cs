// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
