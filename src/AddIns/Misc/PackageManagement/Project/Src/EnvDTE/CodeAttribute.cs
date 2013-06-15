// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
