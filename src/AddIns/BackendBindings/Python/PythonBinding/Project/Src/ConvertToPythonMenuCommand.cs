// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Converts VB.NET or C# code to Python.
	/// </summary>
	public class ConvertToPythonMenuCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			Run(WorkbenchSingleton.Workbench);
		}
		
		protected void Run(IWorkbench workbench)
		{
			// Get the code to convert.
			IViewContent viewContent = workbench.ActiveWorkbenchWindow.ActiveViewContent;
			IEditable editable = viewContent as IEditable;
			
			// Generate the python code.
			SupportedLanguage language = GetSupportedLanguage(viewContent.PrimaryFileName);
			NRefactoryToPythonConverter converter = CreateConverter(language);
			string pythonCode = converter.Convert(editable.Text, language);
			
			// Show the python code in a new window.
			NewFile("Generated.py", "Python", pythonCode);
		}
		
		/// <summary>
		/// Creates either a VBNet converter or C# converted.
		/// </summary>
		NRefactoryToPythonConverter CreateConverter(SupportedLanguage language)
		{
			if (language == SupportedLanguage.VBNet) {
				return new VBNetToPythonConverter();
			}
			return new CSharpToPythonConverter();
		}
				
		/// <summary>
		/// Creates a new file using the FileService by default.
		/// </summary>
		protected virtual void NewFile(string defaultName, string language, string content)
		{
			FileService.NewFile(defaultName, content);
		}		
		
		/// <summary>
		/// Gets the supported language either C# or VB.NET
		/// </summary>
		SupportedLanguage GetSupportedLanguage(string fileName)
		{
			string extension = Path.GetExtension(fileName.ToLower());
			if (extension == ".vb") {
				Console.WriteLine("VBNet");
				return SupportedLanguage.VBNet;
			}
			Console.WriteLine("C#");
			return SupportedLanguage.CSharp;
		}		
	}
}
