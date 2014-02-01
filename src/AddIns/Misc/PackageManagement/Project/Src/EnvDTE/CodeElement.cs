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
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeElement : global::EnvDTE.CodeElementBase, global::EnvDTE.CodeElement
	{
		DTE dte;
		protected CodeModelContext context;
		IEntity entity;
		
		public CodeElement()
		{
		}
		
		public CodeElement(CodeModelContext context)
		{
			this.context = context;
		}
		
		public CodeElement(CodeModelContext context, IEntity entity)
		{
			this.context = context;
			this.entity = entity;
			this.Language = context.CurrentProject.GetCodeModelLanguage();
		}
		
		internal static CodeElement CreateMember(CodeModelContext context, IMember member)
		{
			switch (member.SymbolKind) {
				case SymbolKind.Field:
					return new CodeVariable(context, (IField)member);
				case SymbolKind.Property:
				case SymbolKind.Indexer:
					return new CodeProperty2(context, (IProperty)member);
				case SymbolKind.Event:
					return null;
				case SymbolKind.Method:
				case SymbolKind.Operator:
				case SymbolKind.Constructor:
				case SymbolKind.Destructor:
					return new CodeFunction2(context, (IMethod)member);
				default:
					throw new NotSupportedException("Invalid value for SymbolKind");
			}
		}
		
		public virtual string Name {
			get { return entity.Name; }
		}
		
		public virtual string Language { get; protected set; }
		
		// default is vsCMPart.vsCMPartWholeWithAttributes
		public virtual global::EnvDTE.TextPoint GetStartPoint()
		{
			return TextPoint.CreateStartPoint(context, entity.Region);
		}
		
		public virtual global::EnvDTE.TextPoint GetEndPoint()
		{
			return TextPoint.CreateEndPoint(context, entity.Region);
		}
		
		public virtual global::EnvDTE.vsCMInfoLocation InfoLocation { get; protected set; }
		
		public virtual global::EnvDTE.DTE DTE {
			get {
				if (dte == null) {
					dte = new DTE();
				}
				return dte;
			}
		}
		
		public virtual global::EnvDTE.vsCMElement Kind {
			get { return global::EnvDTE.vsCMElement.vsCMElementOther; }
		}
		
		protected bool IsInFilter(DomRegion region)
		{
			if (context.FilteredFileName == null) {
				return true;
			}
			return context.FilteredFileName == region.FileName;
		}
		
		protected CodeElementsList<CodeAttribute2> GetAttributes(IEntity entity)
		{
			var attributes = new CodeElementsList<CodeAttribute2>();
			foreach (IAttribute attribute in entity.Attributes) {
				if (IsInFilter(attribute.Region)) {
					attributes.Add(new CodeAttribute2(context, attribute));
				}
			}
			return attributes;
		}

		protected override bool GetIsDerivedFrom(string fullName)
		{
			return false;
		}
	}
}
