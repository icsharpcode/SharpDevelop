// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Alpert" email="david@spinthemoose.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Tests the <see cref="NavigationService"/> for the presence of points
	/// to jump back to.
	/// </summary>
	/// <example title="Test if the NavigationService can jump back.">
	/// &lt;Condition name = "CanNavigateBack" &gt;
	/// </example>
	public class CanNavigateBackConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			return NavigationService.CanNavigateBack || NavigationService.CanNavigateForwards;
		}
	}

	/// <summary>
	/// Tests the <see cref="NavigationService"/> for the presence of points
	/// to jump forward to.
	/// </summary>
	/// <example title="Test if the NavigationService can jump forward.">
	/// &lt;Condition name = "CanNavigateForward" &gt;
	/// </example>
	public class CanNavigateForwardConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			return NavigationService.CanNavigateForwards;
		}
	}
}
