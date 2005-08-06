// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Text.RegularExpressions;

using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Core
{
	public class DisplayBindingDescriptor
	{
		object binding = null;
		Codon codon;
		
		public IDisplayBinding Binding {
			get {
				if (binding == null) {
					binding = codon.AddIn.CreateObject(codon.Properties["class"]);
				}
				return binding as IDisplayBinding;
			}
		}
		
		public ISecondaryDisplayBinding SecondaryBinding {
			get {
				if (binding == null) {
					binding = codon.AddIn.CreateObject(codon.Properties["class"]);
				}
				return binding as ISecondaryDisplayBinding;
			}
		}
		
		public bool IsSecondary {
			get {
				return codon.Properties["type"] == "Secondary";
			}
		}
		
		public Codon Codon {
			get {
				return codon;
			}
		}

		public DisplayBindingDescriptor(Codon codon)
		{
			this.codon = codon;
		}
		
		/// <summary>
		/// Gets if the display binding can possibly attach to the file.
		/// If this method returns false, it cannot attach to it; if the method returns
		/// true, it *might* attach to it.
		/// </summary>
		/// <remarks>
		/// This method is used to skip loading addins like the ResourceEditor which cannot
		/// attach to a certain file name for sure.
		/// </remarks>
		public bool CanAttachToFile(string fileName)
		{
			string fileNameRegex = codon.Properties["fileNamePattern"];
			if (fileNameRegex == null || fileNameRegex.Length == 0) // no regex specified
				return true;
			return Regex.IsMatch(fileName, fileNameRegex, RegexOptions.IgnoreCase);
		}
		
		/// <summary>
		/// Gets if the display binding can possibly attach to the language.
		/// If this method returns false, it cannot attach to it; if the method returns
		/// true, it *might* attach to it.
		/// </summary>
		/// <remarks>
		/// This method is used to skip loading addins like the ResourceEditor which cannot
		/// attach to a most languages.
		/// </remarks>
		public bool CanAttachToLanguage(string language)
		{
			string languageRegex = codon.Properties["languagePattern"];
			if (languageRegex == null || languageRegex.Length == 0) // no regex specified
				return true;
			return Regex.IsMatch(language, languageRegex, RegexOptions.IgnoreCase);
		}
	}
}
