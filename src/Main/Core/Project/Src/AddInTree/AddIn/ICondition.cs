/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 21.12.2004
 * Time: 19:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Default actions, when a condition is failed.
	/// </summary>
	public enum ConditionFailedAction {
		Nothing,
		Exclude,
		Disable
	}
		
	/// <summary>
	/// Description of ICondition.
	/// </summary>
	public interface ICondition
	{
		string Name {
			get;
		}
		/// <summary>
		/// Returns the action which occurs, when this condition fails.
		/// </summary>
		ConditionFailedAction Action {
			get;
			set;
		}
		
		/// <summary>
		/// Returns true, when the condition is valid otherwise false.
		/// </summary>
		bool IsValid(object caller);
	}
}
