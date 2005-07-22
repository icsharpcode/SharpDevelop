// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	[Serializable]
	public class DefaultIndexer : AbstractMember, IIndexer
	{
		protected IRegion             bodyRegion;
		protected IRegion             getterRegion;
		protected IRegion             setterRegion;
		List<IParameter> parameters;
		
		public override string DocumentationTag {
			get {
				// TODO: We have to specify the parameters here
				return "P:" + this.DotNetName;
			}
		}
		
		public override IMember Clone()
		{
			return new DefaultIndexer(ReturnType, DefaultParameter.Clone(this.Parameters), Modifiers, Region, BodyRegion, DeclaringType);
		}
		
		public virtual IRegion BodyRegion {
			get {
				return bodyRegion;
			}
		}
		
		public IRegion GetterRegion {
			get {
				return getterRegion;
			}
		}

		public IRegion SetterRegion {
			get {
				return setterRegion;
			}
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
		
		public DefaultIndexer(IClass declaringType) : base(declaringType, null)
		{
		}
		
		public DefaultIndexer(IReturnType type, List<IParameter> parameters, ModifierEnum m, IRegion region, IRegion bodyRegion, IClass declaringType) : this(declaringType)
		{
			this.ReturnType = type;
			this.Parameters = parameters;
			this.Region     = region;
			this.bodyRegion = bodyRegion;
			this.Modifiers = m;
		}
		
		public virtual int CompareTo(IIndexer value)
		{
			int cmp;
			cmp = base.CompareTo((IDecoration)value);
			if (cmp != 0) {
				return cmp;
			}
			
			if (FullyQualifiedName != null) {
				cmp = FullyQualifiedName.CompareTo(value.FullyQualifiedName);
				if (cmp != 0) {
					return cmp;
				}
			}
			
			return DiffUtility.Compare(Parameters, value.Parameters);
		}
		
		int IComparable.CompareTo(object value) {
			return CompareTo((IIndexer)value);
		}
	}
}
