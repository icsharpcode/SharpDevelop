// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Media;

namespace ICSharpCode.CodeQuality
{
	/// <summary>
	/// Description of IColorizer.
	/// </summary>
	public interface IColorizer<TValue>
	{
		SolidColorBrush GetColorBrush(TValue value);
		SolidColorBrush GetColorBrushMixedWith(Color color, TValue value);
		Color GetColor(TValue value);
		string GetUsesText(TValue value);
		string GetDescription(TValue value);
	}
}
