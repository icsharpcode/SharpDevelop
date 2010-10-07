// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Default implementation of <see cref="IAttribute"/>.
	/// </summary>
	public sealed class DefaultAttribute : AbstractFreezable, IAttribute
	{
		DomRegion region;
		ITypeReference attributeType = SharedTypes.UnknownType;
		IList<IConstantValue> positionalArguments;
		IList<KeyValuePair<string, IConstantValue>> namedArguments;
		
		protected override void FreezeInternal()
		{
			attributeType.Freeze();
			positionalArguments = FreezeList(positionalArguments);
			
			if (namedArguments == null || namedArguments.Count == 0) {
				namedArguments = EmptyList<KeyValuePair<string, IConstantValue>>.Instance;
			} else {
				namedArguments = Array.AsReadOnly(namedArguments.ToArray());
				foreach (var pair in namedArguments) {
					pair.Value.Freeze();
				}
			}
			
			base.FreezeInternal();
		}
		
		public DomRegion Region {
			get { return region; }
			set {
				CheckBeforeMutation();
				region = value;
			}
		}
		
		public ITypeReference AttributeType {
			get { return attributeType; }
			set {
				CheckBeforeMutation();
				attributeType = value;
			}
		}
		
		public IList<IConstantValue> PositionalArguments {
			get {
				if (positionalArguments == null)
					positionalArguments = new List<IConstantValue>();
				return positionalArguments;
			}
		}
		
		public IList<KeyValuePair<string, IConstantValue>> NamedArguments {
			get {
				if (namedArguments == null)
					namedArguments = new List<KeyValuePair<string, IConstantValue>>();
				return namedArguments;
			}
		}
	}
}
