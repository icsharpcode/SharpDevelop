// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeElement : global::EnvDTE.CodeElementBase, global::EnvDTE.CodeElement
	{
		DTE dte;
		protected CodeModelContext context;
		readonly ISymbolModel symbolModel;
		IEntity entity;
		
		public CodeElement()
		{
		}
		
		public CodeElement(CodeModelContext context)
		{
			this.context = context;
		}
		
		public CodeElement(CodeModelContext context, ISymbolModel symbolModel)
		{
			this.context = context;
			this.symbolModel = symbolModel;
			if (symbolModel.ParentProject != null) {
				this.Language = symbolModel.ParentProject.GetCodeModelLanguage();
			}
		}
		
		public CodeElement(CodeModelContext context, IEntity entity)
		{
			this.context = context;
			this.entity = entity;
			if (context.CurrentProject != null) {
				this.Language = context.CurrentProject.GetCodeModelLanguage();
			}
		}
		
		internal static CodeElement CreateMember(CodeModelContext context, IMemberModel m)
		{
			switch (m.SymbolKind) {
				case SymbolKind.Field:
					return new CodeVariable(context, m.Resolve() as IField);
				case SymbolKind.Property:
				case SymbolKind.Indexer:
//					return new CodeProperty2(m);
					throw new NotImplementedException();
				case SymbolKind.Event:
					return null; // events are not supported in EnvDTE?
				case SymbolKind.Method:
				case SymbolKind.Operator:
				case SymbolKind.Constructor:
				case SymbolKind.Destructor:
					return new CodeFunction2(context, (IMethodModel)m);
				default:
					throw new NotSupportedException("Invalid value for SymbolKind");
			}
		}
		
		public virtual string Name {
			get {
				if (symbolModel != null)
					return symbolModel.Name;
				return entity.Name;
			}
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
		
		protected CodeElementsList<CodeAttribute2> GetAttributes(IEntityModel entityModel)
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
