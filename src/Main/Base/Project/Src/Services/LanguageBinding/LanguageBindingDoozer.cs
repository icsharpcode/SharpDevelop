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
		
		public object BuildItem(BuildItemArgs args)
		{
			return new LanguageBindingDescriptor(args.Codon);
		}
	}
	
	class LanguageBindingDescriptor
	{
		ILanguageBinding binding = null;
		Codon codon;
		
		public ILanguageBinding Binding {
			get {
				if (binding == null) {
					binding = (ILanguageBinding)codon.AddIn.CreateObject(codon.Properties["class"]);
				}
				return binding;
			}
		}
		
		public Codon Codon {
			get {
				return codon;
			}
		}
		
		public LanguageBindingDescriptor(Codon codon)
		{
			this.codon = codon;
		}
		
		string[] extensions;
		
		public string[] Extensions {
			get {
				if (extensions == null) {
					if (codon.Properties["extensions"].Length == 0)
						extensions = new string[0];
					else
						extensions = codon.Properties["extensions"].ToLowerInvariant().Split(';');
				}
				return extensions;
			}
		}
		
		public string Name {
			get {
				return codon.Properties["id"];
			}
		}
		
		public bool CanAttach(string extension)
		{
			// always attach when no extensions were given
			if (Extensions.Length == 0)
				return true;
			if (string.IsNullOrEmpty(extension))
				return false;
			foreach (string ext in Extensions) {
				if (string.Equals(ext, extension, StringComparison.OrdinalIgnoreCase))
					return true;
			}
			return false;
		}
	}
}
