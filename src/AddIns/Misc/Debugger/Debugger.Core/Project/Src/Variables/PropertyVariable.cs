// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

using Debugger.Interop.CorDebug;

namespace Debugger 
{
	/// <summary>
	/// Delegate that is used to get eval. This delegate may be called at any time and should never return null.
	/// </summary>
	public delegate Eval EvalCreator();
	
	public class PropertyVariable: Variable
	{
		EvalCreator evalCreator;
		Eval cachedEval;
		
		internal PropertyVariable(NDebugger debugger, string name, EvalCreator evalCreator):base(debugger, name, null)
		{
			this.evalCreator = evalCreator;
			this.valueGetter = delegate{return Eval.Result;};
		}
		
		public bool IsEvaluated {
			get {
				return Eval.Evaluated;
			}
		}
		
		public Eval Eval {
			get {
				if (cachedEval == null || cachedEval.HasExpired) {
					cachedEval = evalCreator();
					if (cachedEval == null) throw new DebuggerException("EvalGetter returned null");
					cachedEval.EvalStarted += delegate { OnValueChanged(); };
					cachedEval.EvalComplete += delegate { OnValueChanged(); };
				}
				return cachedEval;
			}
		}
	}
}
