// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
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
		DomRegion bodyRegion = DomRegion.Empty;
		DomRegion getterRegion = DomRegion.Empty;
		DomRegion setterRegion = DomRegion.Empty;
		
		IList<IParameter> parameters = null;
		internal byte accessFlags;
		const byte indexerFlag   = 0x01;
		const byte getterFlag    = 0x02;
		const byte setterFlag    = 0x04;
		const byte extensionFlag = 0x08;
		
		public bool IsIndexer {
			get { return (accessFlags & indexerFlag) == indexerFlag; }
			set { if (value) accessFlags |= indexerFlag; else accessFlags &= 255-indexerFlag; }
		}
		
		public bool CanGet {
			get { return (accessFlags & getterFlag) == getterFlag; }
			set { if (value) accessFlags |= getterFlag; else accessFlags &= 255-getterFlag; }
		}

		public bool CanSet {
			get { return (accessFlags & setterFlag) == setterFlag; }
			set { if (value) accessFlags |= setterFlag; else accessFlags &= 255-setterFlag; }
		}
		
		public bool IsExtensionMethod {
			get { return (accessFlags & extensionFlag) == extensionFlag; }
			set { if (value) accessFlags |= extensionFlag; else accessFlags &= 255-extensionFlag; }
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
			p.accessFlags = this.accessFlags;
			return p;
		}
		
		public virtual IList<IParameter> Parameters {
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
