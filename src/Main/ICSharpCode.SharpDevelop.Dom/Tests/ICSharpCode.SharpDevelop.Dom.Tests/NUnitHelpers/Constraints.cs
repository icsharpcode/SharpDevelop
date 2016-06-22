// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
