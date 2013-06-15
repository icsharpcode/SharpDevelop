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
		IEntityModel entityModel;
		
		public CodeElement()
		{
		}
		
		public CodeElement(IEntityModel entityModel)
		{
			this.entityModel = entityModel;
			this.Language = entityModel.ParentProject.GetCodeModelLanguage();
		}
		
		public virtual string Name {
			get { return entityModel.Name; }
		}
		
		public virtual string Language { get; protected set; }
		
		// default is vsCMPart.vsCMPartWholeWithAttributes
		public virtual global::EnvDTE.TextPoint GetStartPoint()
		{
			return null;
		}
		
		public virtual global::EnvDTE.TextPoint GetEndPoint()
		{
			return null;
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
		
		protected override bool GetIsDerivedFrom(string fullName)
		{
			return false;
		}
	}
}
