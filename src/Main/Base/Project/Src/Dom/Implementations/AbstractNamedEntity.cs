// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="?" email="?"/>
//     <version value="$version"/>
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
					return String.Empty;
				}
				
				return fullyQualifiedName;
			}
			set {
				fullyQualifiedName = value;
				name   = null;
				nspace = null;
			}
		}
		
		public override string DocumentationTag {
			get {
				return FullyQualifiedName;
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
	}
}
