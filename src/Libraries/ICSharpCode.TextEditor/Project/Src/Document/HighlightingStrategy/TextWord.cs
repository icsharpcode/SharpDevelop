// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Diagnostics;

namespace ICSharpCode.TextEditor.Document
{
	public enum TextWordType {
		Word,
		Space,
		Tab
	}
	
	/// <summary>
	/// This class represents single words with color information, two special versions of a word are 
	/// spaces and tabs.
	/// </summary>
	public class TextWord
	{
		HighlightColor  color;
		LineSegment     line;
		IDocument       document;
		
		int          offset;
		int          length;
		
		public class SpaceTextWord : TextWord
		{
			public SpaceTextWord()
			{
				length = 1;
			}
			
			public SpaceTextWord(HighlightColor  color)
			{
				length = 1;
				base.color  = color;
			}
			public override TextWordType Type {
				get {
					return TextWordType.Space;
				}
			}
			public override bool IsWhiteSpace {
				get {
					return true;
				}
			}
		}
		
		public class TabTextWord : TextWord
		{
			public TabTextWord()
			{
				length = 1;
			}
			public TabTextWord(HighlightColor  color)
			{
				length = 1;
				base.color  = color;
			}
			
			
			public override TextWordType Type {
				get {
					return TextWordType.Tab;
				}
			}
			public override bool IsWhiteSpace {
				get {
					return true;
				}
			}
		}
		
		static TextWord spaceWord = new SpaceTextWord();
		static TextWord tabWord   = new TabTextWord();
		
		public bool hasDefaultColor;
		
		static public TextWord Space {
			get {
				return spaceWord;
			}
		}
		
		static public TextWord Tab {
			get {
				return tabWord;
			}
		}
		
		public int Offset {
			get {
				return offset;
			}
		}
		
		public int Length {
			get {
				return length;
			}
		}
		
		public bool HasDefaultColor {
			get {
				return hasDefaultColor;
			}
		}
		
		public virtual TextWordType Type {
			get {
				return TextWordType.Word;
			}
		}
		
		public string Word {
			get {
				if (document == null) {
					return String.Empty;
				}
				return document.GetText(line.Offset + offset, length);
			}
		}
		
		public Font Font {
			get {
				return color.Font;
			}
		}
		
		public Color Color {
			get {
				return color.Color;
			}
		}
		
		public HighlightColor SyntaxColor {
			get {
				return color;
			}
			set {
				color = value;
			}
		}
		
		public virtual bool IsWhiteSpace {
			get {
				return false;
			}
		}
		
		protected TextWord()
		{
		}
		
		// TAB
		public TextWord(IDocument document, LineSegment line, int offset, int length, HighlightColor color, bool hasDefaultColor)
		{
			Debug.Assert(document != null);
			Debug.Assert(line != null);
			Debug.Assert(color != null);
			
			this.document = document;
			this.line  = line;
			this.offset = offset;
			this.length = length;
			this.color = color;
			this.hasDefaultColor = hasDefaultColor;
		}
		
		/// <summary>
		/// Converts a <see cref="TextWord"/> instance to string (for debug purposes)
		/// </summary>
		public override string ToString()
		{
			return "[TextWord: Word = " + Word + ", Font = " + Font + ", Color = " + Color + "]";
		}
	}
}
