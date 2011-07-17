// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	public class ExtractInterfaceOptions
	{
		ITypeDefinition c;
		public ExtractInterfaceOptions(ITypeDefinition c)
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
		
		
		public ITypeDefinition ClassEntity {
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
				                     Path.GetExtension(ClassEntity.Region.FileName));
			}
		}
	}
}
