// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.Core
{
	public interface IOwnerState {
		System.Enum InternalState {
			get;
		}
	}
	
	/// <summary>
	/// Condition evaluator that compares the state of the parameter with a specified value.
	/// The parameter has to implement <see cref="IOwnerState"/>.
	/// </summary>
	public class OwnerStateConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object parameter, Condition condition)
		{
			if (parameter is IOwnerState) {
				try {
					System.Enum state         = ((IOwnerState)parameter).InternalState;
					System.Enum conditionEnum = (System.Enum)Enum.Parse(state.GetType(), condition.Properties["ownerstate"]);
					
					int stateInt     = Int32.Parse(state.ToString("D"));
					int conditionInt = Int32.Parse(conditionEnum.ToString("D"));
					
					return (stateInt & conditionInt) > 0;
				} catch (Exception ex) {
					throw new CoreException("can't parse '" + condition.Properties["state"] + "'. Not a valid value.", ex);
				}
			}
			return false;
		}
	}
}
