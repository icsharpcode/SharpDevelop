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
using System.Linq;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeAttribute : CodeElement, global::EnvDTE.CodeAttribute
	{
		protected readonly IAttribute attribute;
		const string AttributeEndName = "Attribute";
		
		public CodeAttribute()
		{
		}
		
		public CodeAttribute(CodeModelContext context, IAttribute attribute)
			: base(context)
		{
			this.attribute = attribute;
		}
		
		public override global::EnvDTE.vsCMElement Kind {
			get { return global::EnvDTE.vsCMElement.vsCMElementAttribute; }
		}
		
		public override string Name {
			get { return GetShortName(attribute.AttributeType.Name); }
		}
		
		string GetShortName(string name)
		{
			if (name.EndsWith(AttributeEndName)) {
				return name.Substring(0, name.Length - AttributeEndName.Length);
			}
			return name;
		}
		
		public virtual string FullName {
			get { return attribute.AttributeType.FullName; }
		}
		
		public virtual string Value {
			get { return GetValue(); }
			set { }
		}
		
		string GetValue()
		{
			return String.Join(", ", GetArgumentValues());
		}
		
		string[] GetArgumentValues()
		{
			return attribute
				.PositionalArguments
				.Select(arg => GetArgumentValue(arg))
				.ToArray();
		}
		
		string GetArgumentValue(ResolveResult argument)
		{
			return new CodeAttributeArgument(String.Empty, argument).Value;
		}
	}
}
