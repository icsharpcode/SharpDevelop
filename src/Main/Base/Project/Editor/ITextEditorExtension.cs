// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Editor
{
	/// <summary>
	/// Allows add-ins to attach language/project-specific features to ITextEditor (one instance per ITextEditor is created).
	/// </summary>
	public interface ITextEditorExtension
	{
		/// <summary>
		/// Callback function for backend bindings to add services to ITextEditor.
		/// This is called when the file name of an ITextEditor changes.
		/// </summary>
		void Attach(ITextEditor editor);
		
		/// <summary>
		/// Callback function for backend bindings to remove all added services from ITextEditor.
		/// This is called when the file name of an ITextEditor changes, to unload all added
		/// features properly.
		/// </summary>
		void Detach();
	}
	
	/// <summary>
	/// Creates ITextEditorExtension objects for the code editor.
	/// </summary>
	/// <attribute name="extensions" use="required">
	/// Semicolon-separated list of file extensions that are handled by the text editor extension (e.g. .xaml)
	/// </attribute>
	/// <attribute name="class" use="required">
	/// Name of the ITextEditorExtension class.
	/// </attribute>
	/// <usage>Only in /SharpDevelop/ViewContent/TextEditor/Extensions</usage>
	/// <returns>
	/// The ILanguageBinding object.
	/// </returns>
	public class TextEditorExtensionDoozer : IDoozer
	{
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		public object BuildItem(BuildItemArgs args)
		{
			ITextEditor editor = (ITextEditor)args.Parameter;
			string[] extensions = args.Codon["extensions"].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			if (CanAttach(extensions, editor.FileName)) {
				return args.AddIn.CreateObject(args.Codon["class"]);
			} else {
				return null;
			}
		}
		
		static bool CanAttach(string[] extensions, string fileName)
		{
			// always attach when no extensions were given
			if (extensions.Length == 0)
				return true;
			if (string.IsNullOrEmpty(fileName))
				return false;
			string fileExtension = Path.GetExtension(fileName);
			foreach (string ext in extensions) {
				if (string.Equals(ext, fileExtension, StringComparison.OrdinalIgnoreCase))
					return true;
			}
			return false;
		}
	}
}
