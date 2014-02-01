// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
