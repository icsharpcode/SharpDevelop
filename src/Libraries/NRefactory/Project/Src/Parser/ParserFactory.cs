// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.IO;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Parser
{
	public enum SupportedLanguages {
		CSharp,
		VBNet
	}
	
	/// <summary>
	/// Description of IParser.
	/// </summary>
	public class ParserFactory
	{
		public static ILexer CreateLexer(SupportedLanguages language, TextReader textReader)
		{
			switch (language) {
				case SupportedLanguages.CSharp:
					return new ICSharpCode.NRefactory.Parser.CSharp.Lexer(textReader);
				case SupportedLanguages.VBNet:
					return new ICSharpCode.NRefactory.Parser.VB.Lexer(textReader);
			}
			throw new System.NotSupportedException(language + " not supported.");
		}
		
		public static IParser CreateParser(SupportedLanguages language, TextReader textReader)
		{
			ILexer lexer = CreateLexer(language, textReader);
			switch (language) {
				case SupportedLanguages.CSharp:
					return new ICSharpCode.NRefactory.Parser.CSharp.Parser(lexer);
				case SupportedLanguages.VBNet:
					return new ICSharpCode.NRefactory.Parser.VB.Parser(lexer);
			}
			throw new System.NotSupportedException(language + " not supported.");
		}
		
		public static IParser CreateParser(string fileName)
		{
			return CreateParser(fileName, Encoding.UTF8);
		}
		public static IParser CreateParser(string fileName, Encoding encoding)
		{
			switch (Path.GetExtension(fileName).ToUpper()) {
				case ".CS":
					return CreateParser(SupportedLanguages.CSharp, new StreamReader(fileName, encoding));
				case ".VB":
					return CreateParser(SupportedLanguages.VBNet, new StreamReader(fileName, encoding));
			}
			return null;
		}
		
		
	}
}
