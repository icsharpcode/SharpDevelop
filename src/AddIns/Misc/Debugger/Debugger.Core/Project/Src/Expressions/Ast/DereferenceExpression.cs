// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Expressions
{
	/// <summary>
	/// A placeholder expression which can not be evaluated.
	/// </summary>
	public class DereferenceExpression: Expression
	{
		Expression targetObject;
		
		public Expression TargetObject {
			get { return targetObject; }
		}
		
		public DereferenceExpression(Expression targetObject)
		{
			this.targetObject = targetObject;
		}
		
		public override string Code {
			get { return "*" + targetObject.Code; }
		}
		
		public override string CodeTail {
			get { return base.CodeTail; }
		}
		
		protected override Value EvaluateInternal(StackFrame context)
		{
			Value targetValue = targetObject.Evaluate(context);
			if (!targetValue.Type.IsPointer) throw new GetValueException("Target object is not a pointer");
			return targetValue.Dereference();
		}
		
		#region GetHashCode and Equals
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (targetObject != null) hashCode += 1000000007 * targetObject.GetHashCode(); 
			}
			return hashCode;
		}
		
		public override bool Equals(object obj)
		{
			DereferenceExpression other = obj as DereferenceExpression;
			if (other == null) return false; 
			return object.Equals(this.targetObject, other.targetObject);
		}
		
		#endregion
	}
}
