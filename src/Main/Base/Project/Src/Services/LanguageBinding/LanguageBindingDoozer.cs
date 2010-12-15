// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Creates LanguageBindingDescriptor objects for the language binding service.
	/// </summary>
	/// <attribute name="extensions" use="required">
	/// Semicolon-separated list of file extensions that are handled by the language binding (e.g. .xaml)
	/// </attribute>
	/// <attribute name="class" use="required">
	/// Name of the ILanguageBinding class.
	/// </attribute>
	/// <usage>Only in /SharpDevelop/Workbench/LanguageBindings</usage>
	/// <returns>
	/// The ILanguageBinding object.
	/// </returns>
	public class LanguageBindingDoozer : IDoozer
	{
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		public object BuildItem(object caller, Codon codon, System.Collections.ArrayList subItems)
		{
			ITextEditor editor = (ITextEditor)caller;
			string[] extensions = codon.Properties["extensions"].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			if (CanAttach(extensions, editor.FileName)) {
				return codon.AddIn.CreateObject(codon.Properties["class"]);
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
