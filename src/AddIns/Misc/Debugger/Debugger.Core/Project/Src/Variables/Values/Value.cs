// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	/// <summary>
	/// Delegate that is used to get value. This delegate may be called at any time and should never return null.
	/// </summary>
	delegate ICorDebugValue CorValueGetter();
	
	/// <summary>
	/// Value class holds data necessaty to obtain the value of a given object
	/// even after continue. It provides functions to examine the object.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Expiration: Once value expires it can not be used anymore. 
	/// Expiration is permanet - once value expires it stays expired. 
	/// Value expires when any object specified in constructor expires 
	/// or when process exits.
	/// </para>
	/// <para>
	/// Mutation: As long as any dependecy does not mutate the last
	/// obteined value is still considered up to date. (If continue is
	/// called and internal value is neutred, new copy will be obatined)
	/// </para>
	/// </remarks>
	public partial class Value: DebuggerObject, IExpirable
	{
		Process        process;
		string         name;
		ICorDebugValue rawCorValue;
		
		ICorDebugValue corValue;
		PauseSession   corValue_pauseSession;
		DebugType      type;
		
		/// <summary> Occurs when the Value can not be used </summary>
		public event EventHandler Expired;
		
		bool hasExpired = false;
		
		/// <summary> Gets the name associated with the value </summary>
		public string Name {
			get {
				return name;
			}
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
				if (IsNull)      return "<null>";
				if (IsArray)     return "{" + this.Type.FullName + "}";
				if (IsObject)    return "{" + this.Type.FullName + "}";
				//if (IsObject)    return Eval.InvokeMethod(Process, typeof(object), "ToString", this, new Value[] {}).AsString;
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
				return hasExpired;
			}
		}
		
		internal ICorDebugValue RawCorValue {
			get {
				if (this.HasExpired) throw new CannotGetValueException("Value has expired");
				
				return rawCorValue;
			}
		}
		
		internal ICorDebugValue CorValue {
			get {
				if (this.HasExpired) throw new CannotGetValueException("Value has expired");
				
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
			:this (process, string.Empty, rawCorValue)
		{
			
		}
		
		internal Value(Process process,
		               string name,
		               ICorDebugValue rawCorValue)
		{
			this.process = process;
			this.name = name;
			this.rawCorValue = rawCorValue;
			
			// TODO: clean up
			if (name.StartsWith("<") && name.Contains(">") && name != "<Base class>") {
				string middle = name.TrimStart('<').Split('>')[0]; // Get text between '<' and '>'
				if (middle != "") {
					this.name = middle;
				}
			}
			
			type = DebugType.Create(process, rawCorValue.As<ICorDebugValue2>().ExactType);
			
			if (!rawCorValue.Is<ICorDebugHandleValue>()) {
				process.DebuggingResumed += Process_DebuggingResumed;
			}
		}
		
		void Process_DebuggingResumed(object sender, ProcessEventArgs args)
		{
			process.DebuggingResumed -= Process_DebuggingResumed;
			this.hasExpired = true;
			if (Expired != null) {
				Expired(this, EventArgs.Empty);
			}
		}
		
		/// <summary> Returns the <see cref="Debugger.DebugType"/> of the value </summary>
		[Debugger.Tests.ToStringOnly]
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
	}
	
	public class CannotGetValueException: DebuggerException
	{
		public CannotGetValueException(string message):base(message)
		{
			
		}
	}
}
