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
		ICorDebugValue rawCorValue;
		PauseSession   rawCorValue_pauseSession;
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
				return CorValue == null;
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
		
		/// <summary> Returns true if the Value has expired
		/// and can not be used anymore </summary>
		public bool HasExpired {
			get {
				return rawCorValue_pauseSession != process.PauseSession &&
				       !rawCorValue.Is<ICorDebugHandleValue>();
			}
		}
		
		internal ICorDebugValue RawCorValue {
			get {
				if (this.HasExpired) throw new GetValueException("Value has expired");
				
				return rawCorValue;
			}
		}
		
		internal ICorDebugValue CorValue {
			get {
				if (this.HasExpired) throw new GetValueException("Value has expired");
				
				if (corValue_pauseSession != process.PauseSession) {
					corValue = DereferenceUnbox(rawCorValue);
					corValue_pauseSession = process.PauseSession;
				}
				return corValue;
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
		
		internal ICorDebugValue SoftReference {
			get {
				ICorDebugValue corValue = this.RawCorValue;
				if (corValue != null && corValue.Is<ICorDebugHandleValue>()) {
					return corValue;
				}
				corValue = DereferenceUnbox(corValue);
				if (corValue != null && corValue.Is<ICorDebugHeapValue2>()) {
					return corValue.As<ICorDebugHeapValue2>().CreateHandle(CorDebugHandleType.HANDLE_WEAK_TRACK_RESURRECTION).CastTo<ICorDebugValue>();
				} else {
					return corValue; // Value type - return value type
				}
			}
		}
		
		internal Value(Process process,
		               ICorDebugValue rawCorValue)
			:this (process, new EmptyExpression(), rawCorValue)
		{
			
		}
		
		internal Value(Process process,
		               Expression expression,
		               ICorDebugValue rawCorValue)
		{
			this.process = process;
			this.expression = expression;
			this.rawCorValue = rawCorValue;
			this.rawCorValue_pauseSession = process.PauseSession;
			
			if (this.CorValue == null) {
				type = DebugType.GetType(this.Process, null, "System.Object");
			} else {
				ICorDebugType exactType = this.CorValue.CastTo<ICorDebugValue2>().ExactType;
				type = DebugType.Create(this.Process, exactType);
			}
		}
		
		/// <summary> Returns the <see cref="Debugger.DebugType"/> of the value </summary>
		public DebugType Type {
			get {
				return type;
			}
		}
		
		/// <summary> Copy the acutal value from some other Value object </summary>
		public bool SetValue(Value newValue)
		{
			ICorDebugValue corValue = this.RawCorValue;
			ICorDebugValue newCorValue = newValue.RawCorValue;
			if (newCorValue.Type == (uint)CorElementType.BYREF) {
				newCorValue = newCorValue.As<ICorDebugReferenceValue>().Dereference();
			}
			
			if (corValue.Is<ICorDebugReferenceValue>()) {
				if (newCorValue.Is<ICorDebugObjectValue>()) {
					ICorDebugClass corClass = newCorValue.As<ICorDebugObjectValue>().Class;
					ICorDebugValue box = Eval.NewObject(process, corClass).RawCorValue;
					newCorValue = box;
				}
				corValue.CastTo<ICorDebugReferenceValue>().SetValue(newCorValue.CastTo<ICorDebugReferenceValue>().Value);
				return true;
			} else {
				return false;
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
