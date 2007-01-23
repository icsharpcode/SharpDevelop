// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text.RegularExpressions;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
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
		
		bool isSecondary;
		
		public bool IsSecondary {
			get {
				return isSecondary;
			}
		}
		
		public string Title {
			get {
				string title = codon.Properties["title"];
				if (string.IsNullOrEmpty(title))
					return codon.Id;
				else
					return title;
			}
		}
		
		public DisplayBindingDescriptor(Codon codon)
		{
			isSecondary = codon.Properties["type"] == "Secondary";
			if (!isSecondary && codon.Properties["type"] != "" && codon.Properties["type"] != "Primary")
				MessageService.ShowWarning("Unknown display binding type: " + codon.Properties["type"]);
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
			if (fileName == null) // regex specified but file has no name
				return false;
			return Regex.IsMatch(fileName, fileNameRegex, RegexOptions.IgnoreCase);
		}
	}
}
