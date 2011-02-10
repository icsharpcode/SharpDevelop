using System;

namespace ICSharpCode.NRefactory.CSharp
{
	/// <summary>
	/// A type reference in the C# AST.
	/// </summary>
	public abstract class DomType : DomNode
	{
		#region Null
		public new static readonly DomType Null = new NullDomType ();
		
		sealed class NullDomType : DomType
		{
			public override bool IsNull {
				get {
					return true;
				}
			}
			
			public override S AcceptVisitor<T, S> (DomVisitor<T, S> visitor, T data)
			{
				return default (S);
			}
		}
		#endregion
		
		public override NodeType NodeType {
			get { return NodeType.TypeReference; }
		}
	}
}
