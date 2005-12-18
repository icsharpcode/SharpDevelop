// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

using Debugger.Interop.CorDebug;

namespace Debugger 
{
	/// <summary>
	/// Delegate that is used to get eval. This delegate may be called at any time and may return null.
	/// </summary>
	public delegate Eval EvalCreator();
	
	public class PropertyVariable: ClassVariable
	{
		EvalCreator evalCreator;
		Eval cachedEval;
		
		internal PropertyVariable(NDebugger debugger, string name, bool isStatic, bool isPublic, EvalCreator evalCreator):base(debugger, name, isStatic, isPublic, null)
		{
			this.evalCreator = evalCreator;
			this.valueGetter = delegate {
			                   	if (Eval != null) {
			                   		return Eval.Result;
			                   	} else {
			                   		return new UnavailableValue(debugger, "Property has expired");
			                   	}
			                   };
		}
		
		public bool IsEvaluated {
			get {
				if (Eval != null) {
					return Eval.Evaluated;
				} else {
					return true;
				}
			}
		}
		
		public Eval Eval {
			get {
				if (cachedEval == null || cachedEval.HasExpired) {
					cachedEval = evalCreator();
					if (cachedEval != null) {
						cachedEval.EvalStarted += delegate { OnValueChanged(); };
						cachedEval.EvalComplete += delegate { OnValueChanged(); };
					}
				}
				return cachedEval;
			}
		}
	}
}
