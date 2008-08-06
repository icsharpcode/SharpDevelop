// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Markup;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Tokenizer for markup extension attributes.
	/// </summary>
	sealed class MarkupExtensionTokenizer
	{
		private MarkupExtensionTokenizer() {}
		
		string text;
		int pos;
		List<MarkupExtensionToken> tokens = new List<MarkupExtensionToken>();
		
		public static List<MarkupExtensionToken> Tokenize(string text)
		{
			MarkupExtensionTokenizer t = new MarkupExtensionTokenizer();
			t.text = text;
			t.Parse();
			return t.tokens;
		}
		
		void AddToken(MarkupExtensionTokenKind kind, string val)
		{
			tokens.Add(new MarkupExtensionToken(kind, val));
		}
		
		void Parse()
		{
			AddToken(MarkupExtensionTokenKind.OpenBrace, "{");
			Expect('{');
			ConsumeWhitespace();
			CheckNotEOF();
			
			StringBuilder b = new StringBuilder();
			while (pos < text.Length && !char.IsWhiteSpace(text, pos) && text[pos] != '}')
				b.Append(text[pos++]);
			AddToken(MarkupExtensionTokenKind.TypeName, b.ToString());
			
			ConsumeWhitespace();
			while (pos < text.Length) {
				switch (text[pos]) {
					case '}':
						AddToken(MarkupExtensionTokenKind.CloseBrace, "}");
						pos++;
						break;
					case '=':
						AddToken(MarkupExtensionTokenKind.Equals, "=");
						pos++;
						break;
					case ',':
						AddToken(MarkupExtensionTokenKind.Comma, ",");
						pos++;
						break;
					case '{':
						AddToken(MarkupExtensionTokenKind.OpenBrace, "{");
						pos++;
						break;
					default:
						MembernameOrString();
						break;
				}
				ConsumeWhitespace();
			}
		}
		
		void MembernameOrString()
		{
			StringBuilder b = new StringBuilder();
			if (text[pos] == '"' || text[pos] == '\'') {
				char quote = text[pos++];
				CheckNotEOF();
				while (!(text[pos] == quote && text[pos-1] != '\\')) {
					char c = text[pos++];
					if (c != '\\')
						b.Append(c);
					CheckNotEOF();
				}
				pos++; // consume closing quote
				ConsumeWhitespace();
			} else {
				int braceTotal = 0;
				while (braceTotal >= 0) {
					CheckNotEOF();
					switch (text[pos]) {
						case '\\':
							pos++;
							CheckNotEOF();
							b.Append(text[pos++]);
							break;
						case '{':
							b.Append(text[pos++]);
							braceTotal++;
							break;
						case '}':
							if (braceTotal != 0) {
								b.Append(text[pos++]);
							}
							braceTotal--;
							break;
						case ',':
						case '=':
							braceTotal = -1;
							break;
						default:
							b.Append(text[pos++]);
							break;
					}
				}
			}
			CheckNotEOF();
			string valueText = b.ToString();
			if (text[pos] == '=') {
				AddToken(MarkupExtensionTokenKind.Membername, valueText.Trim());
			} else {
				AddToken(MarkupExtensionTokenKind.String, valueText);
			}
		}
		
		void Expect(char c)
		{
			CheckNotEOF();
			if (text[pos] != c)
				throw new XamlMarkupExtensionParseException("Expected '" + c + "'");
			pos++;
		}
		
		void ConsumeWhitespace()
		{
			while (pos < text.Length && char.IsWhiteSpace(text, pos))
				pos++;
		}
		
		void CheckNotEOF()
		{
			if (pos >= text.Length)
				throw new XamlMarkupExtensionParseException("Unexpected end of markup extension");
		}
	}
	
	/// <summary>
	/// Exception thrown when XAML loading fails because there is a syntax error in a markup extension.
	/// </summary>
	[Serializable]
	public class XamlMarkupExtensionParseException : XamlLoadException
	{
		/// <summary>
		/// Create a new XamlMarkupExtensionParseException instance.
		/// </summary>
		public XamlMarkupExtensionParseException()
		{
		}
		
		/// <summary>
		/// Create a new XamlMarkupExtensionParseException instance.
		/// </summary>
		public XamlMarkupExtensionParseException(string message) : base(message)
		{
		}
		
		/// <summary>
		/// Create a new XamlMarkupExtensionParseException instance.
		/// </summary>
		public XamlMarkupExtensionParseException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		/// <summary>
		/// Create a new XamlMarkupExtensionParseException instance.
		/// </summary>
		protected XamlMarkupExtensionParseException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
	
	enum MarkupExtensionTokenKind
	{
		OpenBrace,
		CloseBrace,
		Equals,
		Comma,
		TypeName,
		Membername,
		String
	}
	
	sealed class MarkupExtensionToken
	{
		public readonly MarkupExtensionTokenKind Kind;
		public readonly string Value;
		
		public MarkupExtensionToken(MarkupExtensionTokenKind kind, string value)
		{
			this.Kind = kind;
			this.Value = value;
		}
		
		public override string ToString()
		{
			return "[" + Kind + " " + Value + "]";
		}
	}
	
	static class MarkupExtensionParser
	{
		public static MarkupExtension ConstructMarkupExtension(string attributeText, XamlObject containingObject, XamlTypeResolverProvider typeResolver)
		{
			if (containingObject == null)
				throw new ArgumentNullException("containingObject");
			
			Debug.WriteLine("ConstructMarkupExtension " + attributeText);
			
			List<MarkupExtensionToken> markupExtensionTokens = MarkupExtensionTokenizer.Tokenize(attributeText);
			if (markupExtensionTokens.Count < 3
			    || markupExtensionTokens[0].Kind != MarkupExtensionTokenKind.OpenBrace
			    || markupExtensionTokens[1].Kind != MarkupExtensionTokenKind.TypeName
			    || markupExtensionTokens[markupExtensionTokens.Count-1].Kind != MarkupExtensionTokenKind.CloseBrace)
			{
				throw new XamlMarkupExtensionParseException("Invalid markup extension");
			}
			
			string typeName = markupExtensionTokens[1].Value;
			Type extensionType = typeResolver.Resolve(typeName + "Extension");
			if (extensionType == null) extensionType = typeResolver.Resolve(typeName);
			if (extensionType == null || !typeof(MarkupExtension).IsAssignableFrom(extensionType)) {
				throw new XamlMarkupExtensionParseException("Unknown markup extension " + typeName + "Extension");
			}
			if (extensionType == typeof(TypeExtension))
				extensionType = typeof(MyTypeExtension);
			if (extensionType == typeof(System.Windows.StaticResourceExtension))
				extensionType = typeof(MyStaticResourceExtension);
			
			List<string> positionalArgs = new List<string>();
			List<KeyValuePair<string, string>> namedArgs = new List<KeyValuePair<string, string>>();
			for (int i = 2; i < markupExtensionTokens.Count - 1; i++) {
				if (markupExtensionTokens[i].Kind == MarkupExtensionTokenKind.String) {
					positionalArgs.Add(markupExtensionTokens[i].Value);
				} else if (markupExtensionTokens[i].Kind == MarkupExtensionTokenKind.Membername) {
					if (markupExtensionTokens[i+1].Kind != MarkupExtensionTokenKind.Equals
					    || markupExtensionTokens[i+2].Kind != MarkupExtensionTokenKind.String)
					{
						throw new XamlMarkupExtensionParseException("Invalid markup extension");
					}
					namedArgs.Add(new KeyValuePair<string, string>(markupExtensionTokens[i].Value,
					                                               markupExtensionTokens[i+2].Value));
					i += 2;
				}
			}
			// Find the constructor with positionalArgs.Count arguments
			var ctors = extensionType.GetConstructors().Where(c => c.GetParameters().Length == positionalArgs.Count).ToList();
			if (ctors.Count < 1)
				throw new XamlMarkupExtensionParseException("No constructor for " + extensionType.FullName + " found that takes " + positionalArgs.Count + " arguments");
			if (ctors.Count > 1)
				throw new XamlMarkupExtensionParseException("Multiple constructors for " + extensionType.FullName + " found that take " + positionalArgs.Count + " arguments");
			
			var ctorParameters = ctors[0].GetParameters();
			object[] ctorArguments = new object[positionalArgs.Count];
			for (int i = 0; i < ctorArguments.Length; i++) {
				Type parameterType = ctorParameters[i].ParameterType;
				TypeConverter c = XamlNormalPropertyInfo.GetCustomTypeConverter(parameterType)
					?? TypeDescriptor.GetConverter(parameterType);
				ctorArguments[i] = XamlTextValue.AttributeTextToObject(positionalArgs[i],
				                                                       containingObject,
				                                                       c);
			}
			MarkupExtension result = (MarkupExtension)ctors[0].Invoke(ctorArguments);
			foreach (var pair in namedArgs) {
				string memberName = pair.Key;
				if (memberName.Contains(".")) {
					throw new NotImplementedException();
				} else {
					if (memberName.Contains(":"))
						memberName = memberName.Substring(memberName.IndexOf(':') + 1);
					var property = extensionType.GetProperty(memberName);
					if (property == null)
						throw new XamlMarkupExtensionParseException("Property not found: " + extensionType.FullName + "." + memberName);
					TypeConverter c = TypeDescriptor.GetConverter(property.PropertyType);
					object propValue =  XamlTextValue.AttributeTextToObject(pair.Value,
					                                                        containingObject,
					                                                        c);
					property.SetValue(result, propValue, null);
				}
			}
			return result;
		}
		
		sealed class MyTypeExtension : TypeExtension
		{
			public MyTypeExtension() {}
			
			public MyTypeExtension(string typeName) : base(typeName) {}
		}
		
		sealed class MyStaticResourceExtension : System.Windows.StaticResourceExtension
		{
			public MyStaticResourceExtension() {}
			
			public MyStaticResourceExtension(object resourceKey) : base(resourceKey) {}
			
			public override object ProvideValue(IServiceProvider serviceProvider)
			{
				XamlTypeResolverProvider xamlTypeResolver = (XamlTypeResolverProvider)serviceProvider.GetService(typeof(XamlTypeResolverProvider));
				if (xamlTypeResolver == null)
					throw new XamlLoadException("XamlTypeResolverProvider not found.");
				return xamlTypeResolver.FindResource(this.ResourceKey);
			}
		}
	}
}
