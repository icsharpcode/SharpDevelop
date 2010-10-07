
using System;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace ICSharpCode.NRefactory.TypeSystem
{
	/// <summary>
	/// Represents an ambiguous type (same type found in multiple locations).
	/// </summary>
	public class AmbiguousType : AbstractType
	{
		readonly string name;
		
		public AmbiguousType(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			this.name = name;
		}
		
		public override string Name {
			get { return name; }
		}
		
		public override bool? IsReferenceType {
			get { return null; }
		}
		
		public override int GetHashCode()
		{
			return name.GetHashCode() ^ 12661218;
		}
		
		public override bool Equals(IType other)
		{
			AmbiguousType at = other as AmbiguousType;
			return at != null && name == at.name;
		}
	}
}
