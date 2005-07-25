// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;

namespace ICSharpCode.Core
{
	public interface IOwnerState {
		System.Enum InternalState {
			get;
		}
	}
	
	/// <summary>
	/// Description of ClassErbauer.
	/// </summary>
	public class OwnerStateAuswerter : IAuswerter
	{
		public bool IsValid(object caller, Condition condition)
		{
			if (caller is IOwnerState) {
				try {
					System.Enum state         = ((IOwnerState)caller).InternalState;
					System.Enum conditionEnum = (System.Enum)Enum.Parse(state.GetType(), condition.Properties["ownerstate"]);
					
					int stateInt     = Int32.Parse(state.ToString("D"));
					int conditionInt = Int32.Parse(conditionEnum.ToString("D"));
					
					return (stateInt & conditionInt) > 0;
				} catch (Exception) {
					throw new ApplicationException("can't parse '" + condition.Properties["state"] + "'. Not a valid value.");
				}
			}
			return false;
		}
	}
}
