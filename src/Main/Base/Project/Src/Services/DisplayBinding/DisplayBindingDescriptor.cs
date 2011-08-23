// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text.RegularExpressions;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	public class DisplayBindingDescriptor
	{
		object binding;
		bool isSecondary;
		Codon codon;
		
		/// <summary>
		/// Gets the IDisplayBinding or ISecondaryDisplayBinding if it is already loaded,
		/// otherwise returns null.
		/// </summary>
		internal object GetLoadedBinding()
		{
			return binding;
		}
		
		public IDisplayBinding Binding {
			get {
				if (codon != null && binding == null) {
					binding = codon.AddIn.CreateObject(codon.Properties["class"]);
				}
				return binding as IDisplayBinding;
			}
		}
		
		public ISecondaryDisplayBinding SecondaryBinding {
			get {
				if (codon != null && binding == null) {
					binding = codon.AddIn.CreateObject(codon.Properties["class"]);
				}
				return binding as ISecondaryDisplayBinding;
			}
		}
		
		public bool IsSecondary {
			get { return isSecondary; }
		}
		
		public string Id { get; set; }
		public string Title { get; set; }
		public string FileNameRegex { get; set; }
		
		public DisplayBindingDescriptor(Codon codon)
		{
			if (codon == null)
				throw new ArgumentNullException("codon");
			
			isSecondary = codon.Properties["type"] == "Secondary";
			if (!isSecondary && codon.Properties["type"] != "" && codon.Properties["type"] != "Primary")
				MessageService.ShowWarning("Unknown display binding type: " + codon.Properties["type"]);
			
			this.codon = codon;
			this.Id = codon.Id;
			
			string title = codon.Properties["title"];
			if (string.IsNullOrEmpty(title))
				this.Title = codon.Id;
			else
				this.Title = title;
			
			this.FileNameRegex = codon.Properties["fileNamePattern"];
		}
		
		public DisplayBindingDescriptor(IDisplayBinding binding)
		{
			if (binding == null)
				throw new ArgumentNullException("binding");
			
			this.isSecondary = false;
			this.binding = binding;
		}
		
		public DisplayBindingDescriptor(ISecondaryDisplayBinding binding)
		{
			if (binding == null)
				throw new ArgumentNullException("binding");
			
			this.isSecondary = true;
			this.binding = binding;
		}
		
		/// <summary>
		/// Gets if the display binding can possibly open the file.
		/// If this method returns false, it cannot open it; if the method returns
		/// true, it *might* open it.
		/// Call Binding.CanCreateContentForFile() to know for sure if the binding
		/// will open the file.
		/// </summary>
		/// <remarks>
		/// This method is used to skip loading addins like the ResourceEditor which cannot
		/// attach to a certain file name for sure.
		/// </remarks>
		public bool CanOpenFile(string fileName)
		{
			string fileNameRegex = StringParser.Parse(this.FileNameRegex);
			if (fileNameRegex == null || fileNameRegex.Length == 0) // no regex specified
				return true;
			if (fileName == null) // regex specified but file has no name
				return false;
			return Regex.IsMatch(fileName, fileNameRegex, RegexOptions.IgnoreCase);
		}
		
		public override string ToString()
		{
			return string.Format("[DisplayBindingDescriptor Id={1} Binding={0}]", binding, Id);
		}

	}
}
