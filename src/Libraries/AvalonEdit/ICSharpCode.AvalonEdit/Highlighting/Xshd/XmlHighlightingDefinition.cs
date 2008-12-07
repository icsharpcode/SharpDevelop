// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Highlighting.Xshd
{
	sealed class XmlHighlightingDefinition : IHighlightingDefinition
	{
		public XmlHighlightingDefinition(XshdSyntaxDefinition xshd, IHighlightingDefinitionReferenceResolver resolver)
		{
			xshd.AcceptElements(new RegisterNamedElementsVisitor(this));
			TranslateElementVisitor translateVisitor = new TranslateElementVisitor(this, resolver);
			foreach (XshdElement element in xshd.Elements) {
				HighlightingRuleSet rs = element.AcceptVisitor(translateVisitor) as HighlightingRuleSet;
				XshdRuleSet xrs = element as XshdRuleSet;
				if (rs != null && xrs != null && xrs.Name == null) {
					if (MainRuleSet != null)
						throw Error(element, "Duplicate main RuleSet. There must be only one nameless RuleSet!");
					else
						MainRuleSet = rs;
				}
			}
			if (MainRuleSet == null)
				throw new HighlightingDefinitionInvalidException("Could not find main RuleSet.");
		}
		
		#region RegisterNamedElements
		sealed class RegisterNamedElementsVisitor : IXshdVisitor
		{
			XmlHighlightingDefinition def;
			
			public RegisterNamedElementsVisitor(XmlHighlightingDefinition def)
			{
				this.def = def;
			}
			
			public object VisitRuleSet(XshdRuleSet ruleSet)
			{
				if (ruleSet.Name != null) {
					if (ruleSet.Name.Length == 0)
						throw Error(ruleSet, "Name must not be the empty string");
					if (def.ruleSetDict.ContainsKey(ruleSet.Name))
						throw Error(ruleSet, "Duplicate rule set name '" + ruleSet.Name + "'.");
					
					def.ruleSetDict.Add(ruleSet.Name, new HighlightingRuleSet());
				}
				ruleSet.AcceptElements(this);
				return null;
			}
			
			public object VisitColor(XshdColor color)
			{
				if (color.Name != null) {
					if (color.Name.Length == 0)
						throw Error(color, "Name must not be the empty string");
					if (def.colorDict.ContainsKey(color.Name))
						throw Error(color, "Duplicate color name '" + color.Name + "'.");
					
					def.colorDict.Add(color.Name, new HighlightingColor());
				}
				return null;
			}
			
			public object VisitKeywords(XshdKeywords keywords)
			{
				return keywords.ColorReference.AcceptVisitor(this);
			}
			
			public object VisitSpan(XshdSpan span)
			{
				span.BeginColorReference.AcceptVisitor(this);
				span.SpanColorReference.AcceptVisitor(this);
				span.EndColorReference.AcceptVisitor(this);
				return span.RuleSetReference.AcceptVisitor(this);
			}
			
			public object VisitImport(XshdImport import)
			{
				return import.RuleSetReference.AcceptVisitor(this);
			}
			
			public object VisitRule(XshdRule rule)
			{
				return rule.ColorReference.AcceptVisitor(this);
			}
		}
		#endregion
		
		#region TranslateElements
		sealed class TranslateElementVisitor : IXshdVisitor
		{
			XmlHighlightingDefinition def;
			IHighlightingDefinitionReferenceResolver resolver;
			
			public TranslateElementVisitor(XmlHighlightingDefinition def, IHighlightingDefinitionReferenceResolver resolver)
			{
				Debug.Assert(def != null);
				Debug.Assert(resolver != null);
				this.def = def;
				this.resolver = resolver;
			}
			
			bool ignoreCase;
			
			public object VisitRuleSet(XshdRuleSet ruleSet)
			{
				HighlightingRuleSet rs;
				if (ruleSet.Name != null)
					rs = def.ruleSetDict[ruleSet.Name];
				else
					rs = new HighlightingRuleSet();
				
				bool oldIgnoreCase = ignoreCase;
				if (ruleSet.IgnoreCase != null)
					ignoreCase = ruleSet.IgnoreCase.Value;
				
				foreach (XshdElement element in ruleSet.Elements) {
					object o = element.AcceptVisitor(this);
					HighlightingRuleSet elementRuleSet = o as HighlightingRuleSet;
					if (elementRuleSet != null) {
						Merge(rs, elementRuleSet);
					} else {
						HighlightingSpan span = o as HighlightingSpan;
						if (span != null) {
							rs.Spans.Add(span);
						} else {
							HighlightingRule elementRule = o as HighlightingRule;
							if (elementRule != null) {
								rs.Rules.Add(elementRule);
							}
						}
					}
				}
				
				ignoreCase = oldIgnoreCase;
				
				return rs;
			}
			
			static void Merge(HighlightingRuleSet target, HighlightingRuleSet source)
			{
				target.Rules.AddRange(source.Rules);
				target.Spans.AddRange(source.Spans);
			}
			
			public object VisitColor(XshdColor color)
			{
				HighlightingColor c;
				if (color.Name != null)
					c = def.colorDict[color.Name];
				else if (color.Foreground == null && color.FontStyle == null && color.FontWeight == null)
					return null;
				else
					c = new HighlightingColor();
				
				c.Foreground = color.Foreground;
				c.FontStyle = color.FontStyle;
				c.FontWeight = color.FontWeight;
				return c;
			}
			
			public object VisitKeywords(XshdKeywords keywords)
			{
				if (keywords.Words.Count == 0)
					return Error(keywords, "Keyword group must not be empty.");
				foreach (string keyword in keywords.Words) {
					if (string.IsNullOrEmpty(keyword))
						throw Error(keywords, "Cannot use empty string as keyword");
				}
				StringBuilder keyWordRegex = new StringBuilder(@"\b(?>");
				// (?> = atomic group
				// atomic groups increase matching performance, but we
				// must ensure that the keywords are sorted correctly.
				// "\b(?>in|int)\b" does not match "int" because the atomic group captures "in".
				// To solve this, we are sorting the keywords by descending length.
				int i = 0;
				foreach (string keyword in keywords.Words.OrderByDescending(w=>w.Length)) {
					if (i++ > 0)
						keyWordRegex.Append('|');
					keyWordRegex.Append(Regex.Escape(keyword));
				}
				keyWordRegex.Append(@")\b");
				return new HighlightingRule {
					Color = GetColor(keywords, keywords.ColorReference),
					Regex = CreateRegex(keywords, keyWordRegex.ToString(), XshdRegexType.Default)
				};
			}
			
			Regex CreateRegex(XshdElement position, string regex, XshdRegexType regexType)
			{
				if (regex == null)
					throw Error(position, "Regex missing");
				RegexOptions options = RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;
				if (regexType == XshdRegexType.IgnorePatternWhitespace)
					options |= RegexOptions.IgnorePatternWhitespace;
				if (ignoreCase)
					options |= RegexOptions.IgnoreCase;
				try {
					return new Regex(regex, options);
				} catch (ArgumentException ex) {
					throw Error(position, ex.Message);
				}
			}
			
			HighlightingColor GetColor(XshdElement position, XshdReference<XshdColor> colorReference)
			{
				if (colorReference.InlineElement != null) {
					return (HighlightingColor)colorReference.InlineElement.AcceptVisitor(this);
				} else if (colorReference.ReferencedElement != null) {
					IHighlightingDefinition definition = GetDefinition(position, colorReference.ReferencedDefinition);
					HighlightingColor color = definition.GetNamedColor(colorReference.ReferencedElement);
					if (color == null)
						throw Error(position, "Could not find color named '" + colorReference.ReferencedElement + "'.");
					return color;
				} else {
					return null;
				}
			}
			
			IHighlightingDefinition GetDefinition(XshdElement position, string definitionName)
			{
				if (definitionName == null)
					return def;
				IHighlightingDefinition d = resolver.GetDefinition(definitionName);
				if (d == null)
					throw Error(position, "Could not find definition with name '" + definitionName + "'.");
				return d;
			}
			
			HighlightingRuleSet GetRuleSet(XshdElement position, XshdReference<XshdRuleSet> ruleSetReference)
			{
				if (ruleSetReference.InlineElement != null) {
					return (HighlightingRuleSet)ruleSetReference.InlineElement.AcceptVisitor(this);
				} else if (ruleSetReference.ReferencedElement != null) {
					IHighlightingDefinition definition = GetDefinition(position, ruleSetReference.ReferencedDefinition);
					HighlightingRuleSet ruleSet = definition.GetNamedRuleSet(ruleSetReference.ReferencedElement);
					if (ruleSet == null)
						throw Error(position, "Could not find rule set named '" + ruleSetReference.ReferencedElement + "'.");
					return ruleSet;
				} else {
					return null;
				}
			}
			
			public object VisitSpan(XshdSpan span)
			{
				string endRegex = span.EndRegex;
				if (string.IsNullOrEmpty(span.BeginRegex) && string.IsNullOrEmpty(span.EndRegex))
					throw Error(span, "Span has no start/end regex.");
				if (!span.Multiline) {
					if (endRegex == null)
						endRegex = "$";
					else if (span.EndRegexType == XshdRegexType.IgnorePatternWhitespace)
						endRegex = "($|" + endRegex + "\n)";
					else
						endRegex = "($|" + endRegex + ")";
				}
				HighlightingColor wholeSpanColor = GetColor(span, span.SpanColorReference);
				return new HighlightingSpan {
					StartExpression = CreateRegex(span, span.BeginRegex, span.BeginRegexType),
					EndExpression = CreateRegex(span, endRegex, span.EndRegexType),
					RuleSet = GetRuleSet(span, span.RuleSetReference),
					StartColor = MergeColor(wholeSpanColor, GetColor(span, span.BeginColorReference)),
					SpanColor = wholeSpanColor,
					EndColor = MergeColor(wholeSpanColor, GetColor(span, span.EndColorReference)),
				};
			}
			
			static HighlightingColor MergeColor(HighlightingColor baseColor, HighlightingColor newColor)
			{
				if (baseColor == null)
					return newColor;
				if (newColor == null)
					return baseColor;
				return new HighlightingColor {
					Foreground = newColor.Foreground ?? baseColor.Foreground,
					FontWeight = newColor.FontWeight ?? baseColor.FontWeight,
					FontStyle = newColor.FontStyle ?? baseColor.FontStyle,
				};
			}
			
			public object VisitImport(XshdImport import)
			{
				return GetRuleSet(import, import.RuleSetReference);
			}
			
			public object VisitRule(XshdRule rule)
			{
				return new HighlightingRule {
					Color = GetColor(rule, rule.ColorReference),
					Regex = CreateRegex(rule, rule.Regex, rule.RegexType)
				};
			}
		}
		#endregion
		
		static Exception Error(XshdElement element, string message)
		{
			if (element.LineNumber > 0)
				return new HighlightingDefinitionInvalidException(
					"Error at line " + element.LineNumber + ":\n" + message);
			else
				return new HighlightingDefinitionInvalidException(message);
		}
		
		Dictionary<string, HighlightingRuleSet> ruleSetDict = new Dictionary<string, HighlightingRuleSet>();
		Dictionary<string, HighlightingColor> colorDict = new Dictionary<string, HighlightingColor>();
		
		public HighlightingRuleSet MainRuleSet { get; private set; }
		
		public HighlightingRuleSet GetNamedRuleSet(string name)
		{
			if (string.IsNullOrEmpty(name))
				return MainRuleSet;
			HighlightingRuleSet r;
			if (ruleSetDict.TryGetValue(name, out r))
				return r;
			else
				return null;
		}
		
		public HighlightingColor GetNamedColor(string name)
		{
			HighlightingColor c;
			if (colorDict.TryGetValue(name, out c))
				return c;
			else
				return null;
		}
	}
}
