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
		NDebugger debugger;
		
		ICorDebugValue corValue;
		// ICorDebugHandleValue can be used to get corValue back after Continue()
		public ICorDebugHandleValue corHandleValue;
		PauseSession pauseSessionAtCreation;
		DebugeeState debugeeStateAtCreation;
		
		public NDebugger Debugger {
			get {
				return debugger;
			}
		}
		
		public bool IsExpired {
			get {
				if (corHandleValue == null) {
					return pauseSessionAtCreation != debugger.PauseSession;
				} else {
					return debugeeStateAtCreation != debugger.DebugeeState;
				}
			}
		}
		
		public ICorDebugValue CorValue {
			get {
				if (this.IsExpired) throw new DebuggerException("CorValue has expired");
				
				if (pauseSessionAtCreation == debugger.PauseSession) {
					return corValue;
				} else {
					if (corHandleValue == null) {
						throw new DebuggerException("CorValue has expired");
					} else {
						corValue = PersistentValue.DereferenceUnbox(corHandleValue.As<ICorDebugValue>());
						pauseSessionAtCreation = debugger.PauseSession;
						return corValue;
					}
				}
			}
		}
		
		public ICorDebugHandleValue SoftReference {
			get {
				if (this.IsExpired) throw new DebuggerException("CorValue has expired");
				
				if (corHandleValue != null) return corHandleValue;
				
				ICorDebugHeapValue2 heapValue = this.CorValue.As<ICorDebugHeapValue2>();
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
				this.corHandleValue = corValue.As<ICorDebugHandleValue>();
				this.corValue = PersistentValue.DereferenceUnbox(corValue);
			}
			this.pauseSessionAtCreation = debugger.PauseSession;
			this.debugeeStateAtCreation = debugger.DebugeeState;
		}
	}
}
