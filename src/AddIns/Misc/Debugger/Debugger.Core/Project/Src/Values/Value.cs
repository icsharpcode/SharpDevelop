// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using Debugger.MetaData;
using Debugger.Expressions;
using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	/// <summary>
	/// Value class provides functions to examine value in the debuggee.
	/// It has very life-time.  In general, value dies whenever debugger is
	/// resumed (this includes method invocation and property evaluation).
	/// You can use Expressions to reobtain the value.
	/// </summary>
	public partial class Value: DebuggerObject
	{
		Process        process;
		Expression     expression;
		ICorDebugValue corValue;
		PauseSession   corValue_pauseSession;
		DebugType      type;
		
		/// <summary> Expression which can be used to reobtain this value. </summary>
		public Expression Expression {
			get { return expression; }
		}
		
		/// <summary> Returns true if the value is null </summary>
		public bool IsNull {
			get {
				return 
					(this.Type.IsClass || this.Type.IsValueType || this.Type.IsArray || this.CorType == CorElementType.STRING) &&
					this.CorValue.Is<ICorDebugReferenceValue>() &&
					this.CorReferenceValue.IsNull != 0;
			}
		}
		
		/// <summary> Gets a string representation of the value </summary>
		public string AsString {
			get {
				if (IsNull)      return "null";
				if (IsArray)     return "{" + this.Type.FullName + "}";
				if (IsObject)    return "{" + this.Type.FullName + "}";
				if (IsPrimitive) return PrimitiveValue != null ? PrimitiveValue.ToString() : String.Empty;
				throw new DebuggerException("Unknown value type");
			}
		}
		
		/// <summary> The process that owns the value </summary>
		[Debugger.Tests.Ignore]
		public Process Process {
			get {
				return process;
			}
		}
		
		/// <summary> Returns true if the Value can not be used anymore.
		/// Value is valid only until the debuggee is resummed. </summary>
		public bool IsInvalid {
			get {
				return corValue_pauseSession != process.PauseSession &&
				       !corValue.Is<ICorDebugHandleValue>();
			}
		}
		
		[Tests.Ignore]
		public ICorDebugValue CorValue {
			get {
				if (this.IsInvalid) throw new GetValueException("Value is no longer valid");
				
				return corValue;
			}
		}
		
		ICorDebugReferenceValue CorReferenceValue {
			get {
				if (this.CorValue.Is<ICorDebugReferenceValue>()) {
					return this.CorValue.CastTo<ICorDebugReferenceValue>();
				} else {
					throw new GetValueException("Reference value was expected");
				}
			}
		}
		
		internal CorElementType CorType {
			get {
				ICorDebugValue corValue = this.CorValue;
				if (corValue == null) {
					return (CorElementType)0;
				}
				return (CorElementType)corValue.Type;
			}
		}
		
		public Value GetPermanentReference()
		{
			ICorDebugValue corValue = this.CorValue;
			if (this.Type.IsClass) {
				corValue = this.CorObjectValue.CastTo<ICorDebugHeapValue2>().CreateHandle(CorDebugHandleType.HANDLE_STRONG).CastTo<ICorDebugValue>();
			}
			return new Value(process, expression, corValue);
		}
		
		internal Value(Process process, ICorDebugValue corValue)
			:this (process, new EmptyExpression(), corValue)
		{
			
		}
		
		internal Value(Process process, Expression expression, ICorDebugValue corValue)
		{
			if (corValue == null) {
				throw new ArgumentNullException("corValue");
			}
			this.process = process;
			this.expression = expression;
			this.corValue = corValue;
			this.corValue_pauseSession = process.PauseSession;
			
			ICorDebugType exactType = this.CorValue.CastTo<ICorDebugValue2>().ExactType;
			type = DebugType.Create(this.Process, exactType);
		}
		
		/// <summary> Returns the <see cref="Debugger.DebugType"/> of the value </summary>
		public DebugType Type {
			get {
				return type;
			}
		}
		
		/// <summary> Copy the acutal value from some other Value object </summary>
		public void SetValue(Value newValue)
		{
			ICorDebugValue corValue = this.CorValue;
			ICorDebugValue newCorValue = newValue.CorValue;
			
			if (corValue.Is<ICorDebugReferenceValue>()) {
				if (newCorValue.Is<ICorDebugObjectValue>()) {
					ICorDebugValue box = Eval.NewObjectNoConstructor(newValue.Type).CorValue;
					newCorValue = box;
				}
				corValue.CastTo<ICorDebugReferenceValue>().SetValue(newCorValue.CastTo<ICorDebugReferenceValue>().Value);
			} else {
				corValue.CastTo<ICorDebugGenericValue>().RawValue =
					newCorValue.CastTo<ICorDebugGenericValue>().RawValue;
			}
		}
		
		public override string ToString()
		{
			return this.Expression.Code + " = " + this.AsString;
		}
	}
	
	public class GetValueException: DebuggerException
	{
		Expression expression;
		string error;
		
		/// <summary> Expression that has caused this exception to occur </summary>
		public Expression Expression {
			get { return expression; }
			set { expression = value; }
		}
		
		public string Error {
			get { return error; }
		}
		
		public override string Message {
			get {
				if (expression == null) {
					return error;
				} else {
					return String.Format("Error evaluating \"{0}\": {1}", expression.Code, error);
				}
			}
		}
		
		public GetValueException(string error):base(error)
		{
			this.error = error;
		}
	}
}
