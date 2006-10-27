// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml;

namespace ICSharpCode.TextEditor.Document
{
	public class Span
	{
		bool        stopEOL;
		HighlightColor color;
		HighlightColor beginColor = null;
		HighlightColor endColor = null;
		char[]      begin = null;
		char[]      end   = null;
		string      name  = null;
		string      rule  = null;
		HighlightRuleSet ruleSet = null;
		bool        noEscapeSequences = false;
		bool		ignoreCase = false;
		bool        isBeginSingleWord = false;
		bool        isEndSingleWord = false;
		
		internal HighlightRuleSet RuleSet {
			get {
				return ruleSet;
			}
			set {
				ruleSet = value;
			}
		}

		public bool IgnoreCase	{
			get	{
				return ignoreCase;
			}
			set	{
				ignoreCase = value;
			}
		}

		public bool StopEOL {
			get {
				return stopEOL;
			}
		}
		
		public bool IsBeginSingleWord {
			get {
				return isBeginSingleWord;
			}
		}
		
		public bool IsEndSingleWord {
			get {
				return isEndSingleWord;
			}
		}
		
		public HighlightColor Color {
			get {
				return color;
			}
		}
		
		public HighlightColor BeginColor {
			get {
				if(beginColor != null) {
					return beginColor;
				} else {
					return color;
				}
			}
		}
		
		public HighlightColor EndColor {
			get {
				return endColor!=null ? endColor : color;
			}
		}
		
		public char[] Begin {
			get {
				return begin;
			}
		}
		
		public char[] End {
			get {
				return end;
			}
		}
		
		public string Name {
			get {
				return name;
			}
		}
		
		public string Rule {
			get {
				return rule;
			}
		}
		
		public bool NoEscapeSequences {
			get {
				return noEscapeSequences;
			}
		}
		
		public Span(XmlElement span)
		{
			color   = new HighlightColor(span);
			
			if (span.HasAttribute("rule")) {
				rule = span.GetAttribute("rule");
			}
			
			if (span.HasAttribute("noescapesequences")) {
				noEscapeSequences = Boolean.Parse(span.GetAttribute("noescapesequences"));
			}
			
			name = span.GetAttribute("name");
			if (span.HasAttribute("stopateol")) {
				stopEOL = Boolean.Parse(span.GetAttribute("stopateol"));
			}
			
			begin   = span["Begin"].InnerText.ToCharArray();
			beginColor = new HighlightColor(span["Begin"], color);
			
			if (span["Begin"].HasAttribute("singleword")) {
				this.isBeginSingleWord = Boolean.Parse(span["Begin"].GetAttribute("singleword"));
			}
			
			
			if (span["End"] != null) {
				end  = span["End"].InnerText.ToCharArray();
				endColor = new HighlightColor(span["End"], color);
				if (span["End"].HasAttribute("singleword")) {
					this.isEndSingleWord = Boolean.Parse(span["End"].GetAttribute("singleword"));
				}

			}
		}
	}
}
