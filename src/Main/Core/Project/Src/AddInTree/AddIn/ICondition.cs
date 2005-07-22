// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
