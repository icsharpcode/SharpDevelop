// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;

using System.CodeDom.Compiler;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Internal.Templates;

using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.NRefactory.Parser;

namespace ICSharpCode.SharpDevelop.ProjectImportExporter.Converters
{
	/// <summary>
	/// Description of CSharpToVBNetLanguageConverterOutput.
	/// </summary>
	public class CSharpToVBNetLanguageConverterOutput : AbstractLanguageConverterOutput
	{
		public override string FormatName {
			get {
				return "Convert C# to VB.NET";
			}
		}
		
		protected override string Extension {
			get {
				return ".vb";
			}
		}
		
		protected override IProject CreateProject(string outputPath, IProject originalProject)
		{
			return CreateProject(outputPath, originalProject, "VBNET");
		}
		
		protected override string ConvertFile(string fileName)
		{
			ICSharpCode.NRefactory.Parser.IParser p = ICSharpCode.NRefactory.Parser.ParserFactory.CreateParser(SupportedLanguages.CSharp, new StreamReader(fileName));
			p.Parse();
			
			ICSharpCode.NRefactory.PrettyPrinter.VBNetOutputVisitor vbv = new ICSharpCode.NRefactory.PrettyPrinter.VBNetOutputVisitor();
			vbv.Visit(p.CompilationUnit, null);
			
			return vbv.Text;
		}
	}
}
