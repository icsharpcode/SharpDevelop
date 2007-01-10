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
	public partial class Value: DebuggerObject, IExpirable, IMutable
	{
		Process process;
		
		CorValueGetter corValueGetter;
		
		/// <summary> Occurs when the Value can not be used anymore </summary>
		public event EventHandler Expired;
		
		/// <summary> Occurs when the Value have potentialy changed </summary>
		public event EventHandler<ProcessEventArgs> Changed;
		
		bool isExpired = false;
		
		ValueCache cache;
		
		private class ValueCache
		{
			public PauseSession   PauseSession;
			public ICorDebugValue RawCorValue;
			public ICorDebugValue CorValue;
			public DebugType      Type;
			public string         AsString = String.Empty;
		}
		
		/// <summary>
		/// Cache stores expensive or commonly used information about the value
		/// </summary>
		ValueCache Cache {
			get {
				if (this.HasExpired) throw new CannotGetValueException("Value has expired");
				
				if (cache == null || (cache.PauseSession != process.PauseSession && !cache.RawCorValue.Is<ICorDebugHandleValue>())) {
					DateTime startTime = Util.HighPrecisionTimer.Now;
					
					ValueCache newCache = new ValueCache();
					newCache.PauseSession = process.PauseSession;
					newCache.RawCorValue = corValueGetter();
					newCache.CorValue = DereferenceUnbox(newCache.RawCorValue);
					newCache.Type = DebugType.Create(process, newCache.RawCorValue.As<ICorDebugValue2>().ExactType);
					cache = newCache;
					
					// AsString representation
					if (IsNull)      cache.AsString = "<null>";
					if (IsArray)     cache.AsString = "{" + this.Type.FullName + "}";
					if (IsObject)    cache.AsString = "{" + this.Type.FullName + "}";
					//if (IsObject)    cache.AsString = Eval.InvokeMethod(Process, typeof(object), "ToString", this, new Value[] {}).AsString;
					if (IsPrimitive) cache.AsString = PrimitiveValue != null ? PrimitiveValue.ToString() : String.Empty;
					
					TimeSpan totalTime = Util.HighPrecisionTimer.Now - startTime;
					string name = this is NamedValue ? ((NamedValue)this).Name + " = " : String.Empty;
					process.TraceMessage("Obtained value: " + name + cache.AsString + " (" + totalTime.TotalMilliseconds + " ms)");
				}
				return cache;
			}
		}
		
		/// <summary> The process that owns the value </summary>
		[Debugger.Tests.Ignore]
		public Process Process {
			get {
				return process;
			}
		}
		
		/// <summary> Returns true if the Value have expired
		/// and can not be used anymore </summary>
		public bool HasExpired {
			get {
				return isExpired;
			}
		}
		
		internal ICorDebugValue CorValue {
			get {
				return Cache.CorValue;
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
		
		ICorDebugValue RawCorValue {
			get {
				return Cache.RawCorValue;
			}
		}
		
		internal ICorDebugValue SoftReference {
			get {
				if (this.HasExpired) throw new DebuggerException("CorValue has expired");
				
				ICorDebugValue corValue = RawCorValue;
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
		               IExpirable[] expireDependencies,
		               IMutable[] mutateDependencies,
		               CorValueGetter corValueGetter)
		{
			this.process = process;
			
			AddExpireDependency(process);
			foreach(IExpirable exp in expireDependencies) {
				AddExpireDependency(exp);
			}
			
			foreach(IMutable mut in mutateDependencies) {
				AddMutateDependency(mut);
			}
			
			this.corValueGetter = corValueGetter;
		}
		
		void AddExpireDependency(IExpirable dependency)
		{
			if (dependency.HasExpired) {
				MakeExpired();
			} else {
				dependency.Expired += delegate { MakeExpired(); };
			}
		}
		
		void MakeExpired()
		{
			if (!isExpired) {
				isExpired = true;
				OnExpired(new ValueEventArgs(this));
			}
		}
		
		void AddMutateDependency(IMutable dependency)
		{
			dependency.Changed += delegate { NotifyChange(); };
		}
		
		internal void NotifyChange()
		{
			cache = null;
			if (!isExpired) {
				OnChanged(new ValueEventArgs(this));
			}
		}
		
		/// <summary> Is called when the value changes </summary>
		protected virtual void OnChanged(ProcessEventArgs e)
		{
			if (Changed != null) {
				Changed(this, e);
			}
		}
		
		/// <summary> Is called when the value expires and can not be 
		/// used anymore </summary>
		protected virtual void OnExpired(EventArgs e)
		{
			if (Expired != null) {
				Expired(this, e);
			}
		}
		
		/// <summary> Returns the <see cref="Debugger.DebugType"/> of the value </summary>
		[Debugger.Tests.SummaryOnly]
		public DebugType Type {
			get {
				return Cache.Type;
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
