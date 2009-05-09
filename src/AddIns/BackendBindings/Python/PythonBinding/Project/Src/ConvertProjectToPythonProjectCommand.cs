// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Text;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Converter;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Converts a C# or VB.NET project to Python.
	/// </summary>
	public class ConvertProjectToPythonProjectCommand : LanguageConverter
	{
		ITextEditorProperties textEditorProperties;
		
		public ConvertProjectToPythonProjectCommand(ITextEditorProperties textEditorProperties)
		{
			this.textEditorProperties = textEditorProperties;
		}
		
		public ConvertProjectToPythonProjectCommand() : this(SharpDevelopTextEditorProperties.Instance)
		{
		}
		
		public override string TargetLanguageName {
			get { return PythonLanguageBinding.LanguageName; }
		}
		
		/// <summary>
		/// Converts C# and VB.NET files to Python and saves the files.
		/// </summary>
		protected override void ConvertFile(FileProjectItem sourceItem, FileProjectItem targetItem)
		{
			NRefactoryToPythonConverter converter = NRefactoryToPythonConverter.Create(sourceItem.Include);
			if (converter != null) {
				targetItem.Include = Path.ChangeExtension(sourceItem.Include, ".py");

				string code = GetParseableFileContent(sourceItem.FileName);
				string pythonCode = converter.Convert(code);
				SaveFile(targetItem.FileName, pythonCode, textEditorProperties.Encoding);
			} else {
				LanguageConverterConvertFile(sourceItem, targetItem);
			}
		}
		
		/// <summary>
		/// Calls the LanguageConverter class method ConvertFile which copies the source file to the target
		/// file without any modifications.
		/// </summary>
		protected virtual void LanguageConverterConvertFile(FileProjectItem sourceItem, FileProjectItem targetItem)
		{
			base.ConvertFile(sourceItem, targetItem);
		}
		
		/// <summary>
		/// Writes the specified file to disk.
		/// </summary>
		protected virtual void SaveFile(string fileName, string content, Encoding encoding)
		{
			File.WriteAllText(fileName, content, encoding);
		}
		
		/// <summary>
		/// Gets the content of the file from the parser service.
		/// </summary>
		protected virtual string GetParseableFileContent(string fileName)
		{
			return ParserService.GetParseableFileContent(fileName);
		}		
	}
}
