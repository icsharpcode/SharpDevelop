// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.SharpDevelop.Dom
{
	public class DefaultAttribute : AbstractFreezable, IAttribute
	{
		public static readonly IList<IAttribute> EmptyAttributeList = EmptyList<IAttribute>.Instance;
		
		IList<object> positionalArguments;
		IDictionary<string, object> namedArguments;
		
		protected override void FreezeInternal()
		{
			if (positionalArguments.Count == 0)
				positionalArguments = EmptyList<object>.Instance;
			else
				positionalArguments = new ReadOnlyCollection<object>(positionalArguments);
			
			namedArguments = new ReadOnlyDictionary<string, object>(namedArguments);
			
			base.FreezeInternal();
		}
		
		public DefaultAttribute(IReturnType attributeType) : this(attributeType, AttributeTarget.None) {}
		
		public DefaultAttribute(IReturnType attributeType, AttributeTarget attributeTarget)
			: this(attributeType, attributeTarget, null, null)
		{
		}
		
		public DefaultAttribute(IReturnType attributeType, AttributeTarget attributeTarget, IList<object> positionalArguments, IDictionary<string, object> namedArguments)
		{
			if (attributeType == null)
				throw new ArgumentNullException("attributeType");
			this.AttributeType = attributeType;
			this.AttributeTarget = attributeTarget;
			this.positionalArguments = positionalArguments ?? new List<object>();
			this.namedArguments = namedArguments ?? new SortedList<string, object>();
		}
		
		IReturnType attributeType;
		public IReturnType AttributeType {
			get { return attributeType; }
			set {
				CheckBeforeMutation();
				attributeType = value;
			}
		}
		AttributeTarget attributeTarget;
		public AttributeTarget AttributeTarget {
			get { return attributeTarget; }
			set {
				CheckBeforeMutation();
				attributeTarget = value;
			}
		}
		
		public IList<object> PositionalArguments {
			get { return positionalArguments; }
		}
		
		public IDictionary<string, object> NamedArguments {
			get { return namedArguments; }
		}
		
		ICompilationUnit compilationUnit;
		public ICompilationUnit CompilationUnit {
			get { return compilationUnit; }
			set {
				CheckBeforeMutation();
				compilationUnit = value;
			}
		}
		DomRegion region;
		public DomRegion Region {
			get { return region; }
			set {
				CheckBeforeMutation();
				region = value;
			}
		}
	}
}
