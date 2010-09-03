// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
