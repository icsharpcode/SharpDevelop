// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media;

using ICSharpCode.CodeQualityAnalysis.Utility;

namespace ICSharpCode.CodeQualityAnalysis.Controls
{
	/// <summary>
	/// Description of DependencyColorizer.
	/// </summary>
	public class DependencyColorizer : IColorizer<Relationship>
	{
		private Dictionary<Color, SolidColorBrush> cache;
		
		public DependencyColorizer()
		{
			cache = new Dictionary<Color, SolidColorBrush>();
		}
		
		public SolidColorBrush GetColorBrush(Relationship relationship)
		{
			var color = GetColor(relationship);
			if (cache.ContainsKey(color))
				return cache[color];
			
			var brush = new SolidColorBrush(color);
			brush.Freeze();
			
			cache[color] = brush;
			
			return brush;
		}
		
		public Color GetColor(Relationship relationship)
		{
			if (relationship == null)
				return Colors.Transparent;
			
			if (relationship.Relationships.Any(r => r == RelationshipType.UseThis))
				return Colors.LightBlue;
			if (relationship.Relationships.Any(r => r == RelationshipType.UsedBy))
				return Colors.Violet;
			if (relationship.Relationships.Any(r => r == RelationshipType.Same))
				return Colors.Gray;
			
			return Colors.Transparent;
		}
		
		public SolidColorBrush GetColorBrushMixedWith(Color color, Relationship relationship)
		{
			var mixedColor = GetColor(relationship);
			mixedColor = mixedColor.MixedWith(color);
			
			if (cache.ContainsKey(mixedColor))
				return cache[mixedColor];
			
			var brush = new SolidColorBrush(mixedColor);
			brush.Freeze();
			
			cache[mixedColor] = brush;
			
			return brush;
		}
 		                                      
	}
}
