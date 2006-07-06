// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	/// <summary>
	/// PersistentValue is a container used to obtain the value of a given object even after continue.
	/// </summary>
	public class PersistentCorValue
	{
		/// <summary>
		/// Delegate that is used to get value. This delegate may be called at any time and should never return null.
		/// </summary>
		public delegate ICorDebugValue CorValueGetter();
		
		
		NDebugger debugger;
		
		public ICorDebugValue CorValue;
		// ICorDebugHandleValue can be used to get corValue back after Continue()
		public ICorDebugHandleValue corHandleValue;
		public PauseSession pauseSessionAtCreation;
		public DebugeeState debugeeStateAtCreation;
		
		public bool IsExpired {
			get {
				if (corHandleValue == null) {
					return pauseSessionAtCreation != debugger.PauseSession;
				} else {
					return debugeeStateAtCreation != debugger.DebugeeState;
				}
			}
		}
		
		public ICorDebugValue CorValueProp {
			get {
				if (this.IsExpired) throw new DebuggerException("CorValue has expired");
				
				if (pauseSessionAtCreation == debugger.PauseSession) {
					return CorValue;
				} else {
					if (corHandleValue == null) {
						throw new DebuggerException("CorValue has expired");
					} else {
						CorValue = Value.DereferenceUnbox(corHandleValue.As<ICorDebugValue>());
						pauseSessionAtCreation = debugger.PauseSession;
						return CorValue;
					}
				}
			}
		}
		
		public ICorDebugHandleValue SoftReference {
			get {
				if (this.IsExpired) throw new DebuggerException("CorValue has expired");
				
				if (corHandleValue != null) return corHandleValue;
				
				ICorDebugHeapValue2 heapValue = this.CorValueProp.As<ICorDebugHeapValue2>();
				if (heapValue == null) { // TODO: Investigate - hmmm, value types are not at heap?
					return null;
				} else {
					return heapValue.CreateHandle(CorDebugHandleType.HANDLE_WEAK_TRACK_RESURRECTION);
				}
			}
		}
		
		public PersistentCorValue(NDebugger debugger, ICorDebugValue corValue)
		{
			this.debugger = debugger;
			if (corValue != null) {
				if (corValue.Is<ICorDebugHandleValue>()) {
					corHandleValue = corValue.As<ICorDebugHandleValue>();
				}
				this.CorValue = Value.DereferenceUnbox(corValue);
			}
			this.pauseSessionAtCreation = debugger.PauseSession;
			this.debugeeStateAtCreation = debugger.DebugeeState;
		}
	}
}
