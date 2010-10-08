
using System;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	public class ConstantResolveResult
	{
		public IType Type { get; set; }
		public object Value { get; set; }
		
		public ConstantResolveResult(IType type, object value)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (value == null)
				throw new ArgumentNullException("value");
			this.Type = type;
			this.Value = value;
		}
	}
}
