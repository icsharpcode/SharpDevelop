// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Alpert" email="david@spinthemoose.com"/>
//     <version>$Revision:  $</version>
// </file>

using System;

using ICSharpCode.SharpDevelop.Dom;

namespace NUnit.Framework.Constraints
{
	#region IProperty constraints
	/// <summary>
	/// Abstract NUnit Constraint to refactor the test for
	/// emptiness of an IProperty's get or set accessor region.
	/// </summary>
	public abstract class PropertyAccessorIsEmptyConstraint : NUnit.Framework.Constraints.Constraint
	{
		protected IProperty p;
		protected abstract string accessorText {get;}
		protected abstract DomRegion RegionToTest {get;}
		
		public override bool Matches(object actual)
		{
			this.actual = actual;
			this.p = actual as IProperty;
			if (actual is IProperty) {
				// test to ensure that the DomRegion encapsulates either "get;" or "set;"
				return AccessorDomRegionIsEmpty(this.RegionToTest);
			}
			// not an IProperty
			return false;
		}
		
		public bool AccessorDomRegionIsEmpty(DomRegion region) {
			return region.EndLine == region.BeginLine
				&& region.EndColumn - region.BeginColumn == 4;
		}
		
		public override void WriteMessageTo(MessageWriter writer) {
			if (this.actual is IProperty) {
				writer.WriteMessageLine("Expected {0} to have an empty {1} accessor but it was not empty {2}.", this.actual, this.accessorText, this.RegionToTest);
			} else {
				writer.WriteMessageLine("{0} is not an IProperty; cannot test for the emptiness of a {1} accessor.", this.actual, this.accessorText);
			}
		}
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate(String.Format("An IProperty with an empty {0} region", this.accessorText));
		}
	}

	public class PropertyGetIsEmptyConstraint : PropertyAccessorIsEmptyConstraint
	{
		protected override string accessorText { get { return "get"; } }
		protected override DomRegion RegionToTest { get { return this.p.GetterRegion; } }
	}
	
	public class PropertySetIsEmptyConstraint : PropertyAccessorIsEmptyConstraint
	{
		protected override string accessorText { get { return "set"; } }
		protected override DomRegion RegionToTest { get { return this.p.SetterRegion; } }
	}
	
	#endregion
	
	#region IMethod Constraints

	public class MethodBodyIsEmptyConstraint : NUnit.Framework.Constraints.Constraint
	{
		IMethod m;
		public override bool Matches(object actual)
		{
			this.actual = actual;
			this.m = actual as IMethod;
			if (this.m == null) {
				return false;
			}
			return m.BodyRegion.EndLine == 0 && m.BodyRegion.EndColumn == 0;
		}
		public override void WriteMessageTo(MessageWriter writer)
		{
			if (this.actual is IMethod) {
				writer.WriteMessageLine("Expected {0} to have an empty BodyRegion but it was not empty {1}.", this.m, this.m.BodyRegion);
			} else {
				writer.WriteMessageLine("{0} is not an IMethod; cannot test for the emptiness of a it's BodyRegion.", this.actual);
			}
		}
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("A method with an empty BodyRegion");
		}
	}
	

	#endregion
}
