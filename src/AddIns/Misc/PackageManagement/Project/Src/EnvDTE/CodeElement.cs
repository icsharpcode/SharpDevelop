// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeElement : MarshalByRefObject
	{
		DTE dte;
			
		public CodeElement()
		{
		}
		
		public CodeElement(IEntity entity)
		{
			this.Entity = entity;
			this.Language = GetLanguage(entity.ProjectContent);
		}
		
		protected string GetLanguage(IProjectContent projectContent)
		{
			if (projectContent.Project != null) {
				var projectType = new ProjectType(projectContent.Project as MSBuildBasedProject);
				if (projectType.Type == ProjectType.VBNet) {
					return CodeModelLanguageConstants.vsCMLanguageVB;
				}
			}
			return CodeModelLanguageConstants.vsCMLanguageCSharp;
		}
		
		protected IEntity Entity { get; private set; }
		
		public virtual string Name {
			get { return GetName(); }
		}
		
		string GetName()
		{
			int index = Entity.FullyQualifiedName.LastIndexOf('.');
			return Entity.FullyQualifiedName.Substring(index + 1);
		}
		
		public virtual string Language { get; protected set; }
		
		// default is vsCMPart.vsCMPartWholeWithAttributes
		public virtual TextPoint GetStartPoint()
		{
			return null;
		}
		
		public virtual TextPoint GetEndPoint()
		{
			return null;
		}
		
		public virtual vsCMInfoLocation InfoLocation { get; protected set; }
		
		public virtual DTE DTE {
			get {
				if (dte == null) {
					dte = new DTE();
				}
				return dte;
			}
		}
		
		protected vsCMAccess GetAccess()
		{
			if (Entity.IsPublic) {
				return vsCMAccess.vsCMAccessPublic;
			}
			return vsCMAccess.vsCMAccessPrivate;
		}
		
		public virtual vsCMElement Kind {
			get { return vsCMElement.vsCMElementOther; }
		}
	}
}
