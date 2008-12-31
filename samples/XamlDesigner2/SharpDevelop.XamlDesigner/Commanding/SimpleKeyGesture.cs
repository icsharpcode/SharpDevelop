using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.ComponentModel;
using System.Globalization;

namespace SharpDevelop.XamlDesigner.Commanding
{
	[TypeConverter(typeof(SimpleKeyGestureConverter))]
	public class SimpleKeyGesture : InputGesture
	{
		public Key Key;
		public ModifierKeys Modifiers;

		static KeyConverter KeyConverter = new KeyConverter();
		static ModifierKeysConverter ModifierKeysConverter = new ModifierKeysConverter();

		public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
		{
			KeyEventArgs e = inputEventArgs as KeyEventArgs;
			return e != null && e.Key == Key && Keyboard.Modifiers == Modifiers;
		}

		public static SimpleKeyGesture FromString(string s)
		{
			var result = new SimpleKeyGesture();
			var index = s.LastIndexOf('+');

			if (index >= 0) {
				result.Modifiers = (ModifierKeys)ModifierKeysConverter.ConvertFromString(s.Substring(0, index));
				result.Key = (Key)KeyConverter.ConvertFromString(s.Substring(index + 1));
			}
			else {
				result.Key = (Key)KeyConverter.ConvertFromString(s);
			}

			return result;
		}

		public override string ToString()
		{
			if (Modifiers == ModifierKeys.None) return KeyConverter.ConvertToString(Key);
			return ModifierKeysConverter.ConvertToString(Modifiers) + "+" + KeyConverter.ConvertToString(Key);
		}
	}

	class SimpleKeyGestureConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return SimpleKeyGesture.FromString((string)value);
		}
	}
}
