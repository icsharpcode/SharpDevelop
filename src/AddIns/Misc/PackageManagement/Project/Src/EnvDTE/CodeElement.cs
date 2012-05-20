// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeElement : MarshalByRefObject
	{
		public CodeElement(IEntity entity)
		{
			this.Entity = entity;
		}
		
		public CodeElement()
		{
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
		
		public virtual string Language { get; private set; }
		
		// default is vsCMPart.vsCMPartWholeWithAttributes
		public virtual TextPoint GetStartPoint()
		{
			throw new NotImplementedException();
		}
		
		public virtual TextPoint GetEndPoint()
		{
			throw new NotImplementedException();
		}
		
		public virtual vsCMInfoLocation InfoLocation { get; private set; }
		public virtual DTE DTE { get; private set; }
	}
}
