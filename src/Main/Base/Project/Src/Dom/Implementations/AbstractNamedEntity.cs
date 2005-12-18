// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;

namespace ICSharpCode.SharpDevelop.Dom
{
	[Serializable]
	public abstract class AbstractNamedEntity : AbstractDecoration
	{
		static char[] nameDelimiters = new char[] { '.', '+' };
		string fullyQualifiedName = null;
		string name               = null;
		string nspace             = null;
		
		public string FullyQualifiedName {
			get {
				if (fullyQualifiedName == null) {
					if (name != null && nspace != null) {
						fullyQualifiedName = nspace + '.' + name;
					} else {
						return String.Empty;
					}
				}
				return fullyQualifiedName;
			}
			set {
				if (fullyQualifiedName == value)
					return;
				fullyQualifiedName = value;
				name   = null;
				nspace = null;
				OnFullyQualifiedNameChanged(EventArgs.Empty);
			}
		}
		
		protected virtual void OnFullyQualifiedNameChanged(EventArgs e)
		{
		}
		
		public virtual string DotNetName {
			get {
				if (this.DeclaringType != null) {
					return this.DeclaringType.DotNetName + "." + this.Name;
				} else {
					return FullyQualifiedName;
				}
			}
		}
		
		public string Name {
			get {
				if (name == null && FullyQualifiedName != null) {
					int lastIndex;
					
					if (CanBeSubclass) {
						lastIndex = FullyQualifiedName.LastIndexOfAny(nameDelimiters);
					} else {
						lastIndex = FullyQualifiedName.LastIndexOf('.');
					}
					
					if (lastIndex < 0) {
						name = FullyQualifiedName;
					} else {
						name = FullyQualifiedName.Substring(lastIndex + 1);
					}
				}
				return name;
			}
		}

		public string Namespace {
			get {
				if (nspace == null && FullyQualifiedName != null) {
					int lastIndex = FullyQualifiedName.LastIndexOf('.');
					
					if (lastIndex < 0) {
						nspace = String.Empty;
					} else {
						nspace = FullyQualifiedName.Substring(0, lastIndex);
					}
				}
				return nspace;
			}
		}
		
		protected virtual bool CanBeSubclass {
			get {
				return false;
			}
		}
		
		public AbstractNamedEntity(IClass declaringType) : base(declaringType)
		{
		}
		
		public AbstractNamedEntity(IClass declaringType, string name) : base(declaringType)
		{
			System.Diagnostics.Debug.Assert(declaringType != null);
			this.name = name;
			nspace = declaringType.FullyQualifiedName;
			
			// lazy-computing the fully qualified name for class members saves ~7 MB RAM (when loading the SharpDevelop solution).
			//fullyQualifiedName = nspace + '.' + name;
		}
		
		public override string ToString()
		{
			return String.Format("[{0}: {1}]", GetType().Name, FullyQualifiedName);
		}
	}
}
