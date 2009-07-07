/*
 * Created by SharpDevelop.
 * User: trecio
 * Date: 2009-05-31
 * Time: 22:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace CppBinding.Parser
{
	/// <summary>
	/// Description of Parser.
	/// </summary>
	public class CppParser : IParser
	{
		public CppParser() {
			LexerTags = new string[0];
		}
		
		public string[] LexerTags {
			get;
			set;
		}
		
		public LanguageProperties Language {
			get {
				return LanguageProperties.None;
			}
		}
		
		public IExpressionFinder CreateExpressionFinder(string fileName) {
			throw new NotImplementedException();
		}
		
		public bool CanParse(string fileName) {
			string extension = Path.GetExtension(fileName).ToLower();
			return Array.Find(SupportedExtensions, x => extension.Equals(x)) != null;
		}
		
		public bool CanParse(IProject project) {
			return project.Language == "C++";
		}
	
		public ICompilationUnit Parse(IProjectContent projectContent, string fileName, string fileContent) {
//            currentParser.Parse(fileContent);

			throw new NotImplementedException();
		}
		
		public IResolver CreateResolver() {
			throw new NotImplementedException();
		}

        private readonly string[] SupportedExtensions = { ".h", ".hpp", ".c", ".cpp" };

//        private IManagedCppParser currentParser = new ManagedCppParser();

    }
}
