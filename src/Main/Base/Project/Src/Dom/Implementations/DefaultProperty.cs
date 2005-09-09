// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom {

	[Serializable]
	public class DefaultProperty : AbstractMember, IProperty
	{
		protected DomRegion bodyRegion = DomRegion.Empty;
		
		DomRegion getterRegion = DomRegion.Empty;
		DomRegion setterRegion = DomRegion.Empty;

		protected IMethod     getterMethod;
		protected IMethod     setterMethod;
		List<IParameter> parameters = null;
		bool isIndexer;

		public bool IsIndexer {
			get {
				return isIndexer;
			}
			set {
				isIndexer = value;
			}
		}
		
		public override string DocumentationTag {
			get {
				return "P:" + this.DotNetName;
			}
		}
		
		public virtual DomRegion BodyRegion {
			get {
				return bodyRegion;
			}
		}
		
		public override IMember Clone()
		{
			DefaultProperty p = new DefaultProperty(Name, ReturnType, Modifiers, Region, BodyRegion, DeclaringType);
			p.parameters = DefaultParameter.Clone(this.Parameters);
			p.isIndexer = this.isIndexer;
			return p;
		}
		
		public virtual List<IParameter> Parameters {
			get {
				if (parameters == null) {
					parameters = new List<IParameter>();
				}
				return parameters;
			}
			set {
				parameters = value;
			}
		}

		public DomRegion GetterRegion {
			get {
				return getterRegion;
			}
			set {
				getterRegion = value;
			}
		}

		public DomRegion SetterRegion {
			get {
				return setterRegion;
			}
			set {
				setterRegion = value;
			}
		}

		public IMethod GetterMethod {
			get {
				return getterMethod;
			}
		}

		public IMethod SetterMethod {
			get {
				return setterMethod;
			}
		}

		public virtual bool CanGet {
			get {
				return !getterRegion.IsEmpty;
			}
		}

		public virtual bool CanSet {
			get {
				return !setterRegion.IsEmpty;
			}
		}
		
		public DefaultProperty(IClass declaringType, string name) : base(declaringType, name)
		{
		}
		
		public DefaultProperty(string name, IReturnType type, ModifierEnum m, DomRegion region, DomRegion bodyRegion, IClass declaringType) : base(declaringType, name)
		{
			this.ReturnType = type;
			this.Region = region;
			this.bodyRegion = bodyRegion;
			Modifiers = m;
		}
		
		public virtual int CompareTo(IProperty value)
		{
			int cmp;
			
			if(0 != (cmp = base.CompareTo((IDecoration)value)))
				return cmp;
			
			if (FullyQualifiedName != null) {
				cmp = FullyQualifiedName.CompareTo(value.FullyQualifiedName);
				if (cmp != 0) {
					return cmp;
				}
			}
			
			return DiffUtility.Compare(Parameters, value.Parameters);
		}
		
		int IComparable.CompareTo(object value) {
			return CompareTo((IProperty)value);
		}
	}
}
