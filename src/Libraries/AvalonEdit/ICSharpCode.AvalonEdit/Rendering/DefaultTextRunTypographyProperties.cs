// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Media.TextFormatting;

namespace ICSharpCode.AvalonEdit.Rendering
{
	/// <summary>
	/// Default implementation for TextRunTypographyProperties.
	/// </summary>
	public class DefaultTextRunTypographyProperties : TextRunTypographyProperties
	{
		public override FontVariants Variants {
			get { return FontVariants.Normal; }
		}
		
		public override bool StylisticSet1 { get { return false; } }
		public override bool StylisticSet2 { get { return false; } }
		public override bool StylisticSet3 { get { return false; } }
		public override bool StylisticSet4 { get { return false; } }
		public override bool StylisticSet5 { get { return false; } }
		public override bool StylisticSet6 { get { return false; } }
		public override bool StylisticSet7 { get { return false; } }
		public override bool StylisticSet8 { get { return false; } }
		public override bool StylisticSet9 { get { return false; } }
		public override bool StylisticSet10 { get { return false; } }
		public override bool StylisticSet11 { get { return false; } }
		public override bool StylisticSet12 { get { return false; } }
		public override bool StylisticSet13 { get { return false; } }
		public override bool StylisticSet14 { get { return false; } }
		public override bool StylisticSet15 { get { return false; } }
		public override bool StylisticSet16 { get { return false; } }
		public override bool StylisticSet17 { get { return false; } }
		public override bool StylisticSet18 { get { return false; } }
		public override bool StylisticSet19 { get { return false; } }
		public override bool StylisticSet20 { get { return false; } }
		
		public override int StylisticAlternates {
			get { return 0; }
		}
		
		public override int StandardSwashes {
			get { return 0; }
		}
		
		public override bool StandardLigatures {
			get { return true; }
		}
		
		public override bool SlashedZero {
			get { return false; }
		}
		
		public override FontNumeralStyle NumeralStyle {
			get { return FontNumeralStyle.Normal; }
		}
		
		public override FontNumeralAlignment NumeralAlignment {
			get { return FontNumeralAlignment.Normal; }
		}
		
		public override bool MathematicalGreek {
			get { return false; }
		}
		
		public override bool Kerning {
			get { return true; }
		}
		
		public override bool HistoricalLigatures {
			get { return false; }
		}
		
		public override bool HistoricalForms {
			get { return false; }
		}
		
		public override FontFraction Fraction {
			get { return FontFraction.Normal; }
		}
		
		public override FontEastAsianWidths EastAsianWidths {
			get { return FontEastAsianWidths.Normal; }
		}
		
		public override FontEastAsianLanguage EastAsianLanguage {
			get { return FontEastAsianLanguage.Normal; }
		}
		
		public override bool EastAsianExpertForms {
			get { return false; }
		}
		
		public override bool DiscretionaryLigatures {
			get { return false; }
		}
		
		public override int ContextualSwashes {
			get { return 0; }
		}
		
		public override bool ContextualLigatures {
			get { return true; }
		}
		
		public override bool ContextualAlternates {
			get { return true; }
		}
		
		public override bool CaseSensitiveForms {
			get { return false; }
		}
		
		public override bool CapitalSpacing {
			get { return false; }
		}
		
		public override FontCapitals Capitals {
			get { return FontCapitals.Normal; }
		}
		
		public override int AnnotationAlternates {
			get { return 0; }
		}
	}
}
