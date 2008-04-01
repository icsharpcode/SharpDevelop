// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Alpert" email="david@spinthemoose.com"/>
//     <version>$Revision:  $</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Description of ExtractInterfaceDetails.
	/// </summary>
	public class ExtractInterfaceOptions
	{
		IClass c;
		public ExtractInterfaceOptions(IClass c)
		{
			if (null == c) {
				throw new InvalidOperationException("ExtractInterfaceOptions requires a valid IClass");
			}
			this.c = c;
			this.NewInterfaceName = this.ClassEntity.Name.StartsWith("I") ? 
				String.Format("{0}1", this.ClassEntity.Name) :
				String.Format("I{0}", this.ClassEntity.Name);
			this.NewFileName = this.SuggestedFileName;
			this.ChosenMembers = new List<IMember>();
		}
		
		
		public IClass ClassEntity {
			get {
				return this.c;
			}
		}

		public string NewInterfaceName {get; set;}
		public string NewFileName {get;set;}
		public bool IsCancelled {get; set;}
		public bool IncludeComments {get; set;}
		public bool AddInterfaceToClass {get; set;}
		public List<IMember> ChosenMembers;
		
		public string FullyQualifiedName {
			get {
				return String.Format("{0}.{1}",
				                     c.Namespace,
				                     this.NewInterfaceName);
			}
		}
		
		public string SuggestedFileName {
			get {
				return String.Format("{0}{1}",
				                     this.NewInterfaceName,
				                     Path.GetExtension(ClassEntity.CompilationUnit.FileName));
			}
		}
	}
}
