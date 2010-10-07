// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Default implementation of <see cref="IProperty"/>.
	/// </summary>
	public class DefaultProperty : AbstractMember, IProperty
	{
		Accessibility getterAccessibility;
		Accessibility setterAccessibility;
		IList<IParameter> parameters;
		
		const ushort FlagIsIndexer = 0x1000;
		const ushort FlagCanGet    = 0x2000;
		const ushort FlagCanSet    = 0x4000;
		
		protected override void FreezeInternal()
		{
			parameters = FreezeList(parameters);
			base.FreezeInternal();
		}
		
		public DefaultProperty(ITypeDefinition declaringTypeDefinition, string name)
			: base(declaringTypeDefinition, name, EntityType.Property)
		{
		}
		
		public bool IsIndexer {
			get { return flags[FlagIsIndexer]; }
			set {
				CheckBeforeMutation();
				flags[FlagIsIndexer] = value;
			}
		}
		
		public IList<IParameter> Parameters {
			get {
				if (parameters == null)
					parameters = new List<IParameter>();
				return parameters;
			}
		}
		
		public bool CanGet {
			get { return flags[FlagCanGet]; }
			set {
				CheckBeforeMutation();
				flags[FlagCanGet] = value;
			}
		}
		
		public bool CanSet {
			get { return flags[FlagCanSet]; }
			set {
				CheckBeforeMutation();
				flags[FlagCanSet] = value;
			}
		}
		
		public Accessibility GetterAccessibility {
			get { return getterAccessibility; }
			set {
				CheckBeforeMutation();
				getterAccessibility = value;
			}
		}
		
		public Accessibility SetterAccessibility {
			get { return setterAccessibility; }
			set {
				CheckBeforeMutation();
				setterAccessibility = value;
			}
		}
	}
}
