using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Input;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Converts <see cref="PartialKeyGesture"/> instance to and from other types
	/// </summary>
	public class PartialKeyGestureConverter : TypeConverter
	{
		private readonly KeyConverter keyConverter = new KeyConverter();
		private readonly ModifierKeysConverter modidierKeysConverter = new ModifierKeysConverter();
		
		/// <summary>
		/// Determines whether object of specified type can be converted to instance of <see cref="PartialKeyGesture"/>
		/// </summary>
		/// <param name="context">A format context that provides information about the environment from which this converter is being invoked. </param>
		/// <param name="sourceType">The type being evaluated for conversion. </param>
		/// <returns>True if this converter can perform the operation; otherwise, false. </returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if(sourceType == typeof(string)) {
				return true;
			}
			
			return base.CanConvertFrom(context, sourceType);
		}
		
		/// <summary>
		/// Determines whether an instance of <see cref="PartialKeyGesture"/> can be converted to the specified type, using the specified context.
		/// </summary>
		/// <param name="context">A format context that provides information about the environment from which this converter is being invoked.</param>
		/// <param name="destinationType">The type being evaluated for conversion. </param>
		/// <returns>True if this converter can perform the operation; otherwise, false. </returns>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string)) {
				return true;
			}
			
			return base.CanConvertFrom(context, destinationType);
		}
		
		/// <summary>
		/// Attempts to convert a <see cref="PartialKeyGesture"/> to the specified type, using the specified context.
		/// </summary>
		/// <param name="context">A format context that provides information about the environment from which this converter is being invoked.</param>
		/// <param name="culture">Culture specific information. </param>
		/// <param name="value">The object to convert</param>
		/// <param name="destinationType">The type wich instance of <see cref="PartialKeyGesture"/> is being converted to.</param>
		/// <returns>The converted object, or an empty string if value is a null reference</returns>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value == null && destinationType == typeof(string)) {
				return String.Empty;
			}
			
			if(value is PartialKeyGesture && destinationType == typeof(string)) {
				var partialKeyGesture = value as PartialKeyGesture;
				
				var modifierString = (string)new ModifierKeysConverter().ConvertTo(context, culture, partialKeyGesture.Modifiers, typeof(string));
				var keyString = (string)new KeyConverter().ConvertTo(context, culture, partialKeyGesture.Key, typeof(string));
				
				var sb = new StringBuilder();
				sb.Append(modifierString);
				if(!string.IsNullOrEmpty(keyString) && !string.IsNullOrEmpty(modifierString)) {
					sb.Append("+");
				}
				sb.Append(keyString);
				
				return sb.ToString();
			}
			
			return base.ConvertTo(context, culture, value, destinationType);
		}
		
		/// <summary>
		/// Attempts to convert the specified object to a <see cref="PartialKeyGesture"/>, using the specified context.
		/// </summary>
		/// <param name="context">A format context that provides information about the environment from which this converter is being invoked.</param>
		/// <param name="culture">Culture specific information.</param>
		/// <param name="value">The object to convert</param>
		/// <returns>The converted object.</returns>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			var gestureString = value as string;
			if (gestureString != null) {
				var keysStart = gestureString.LastIndexOf('+');
				if (keysStart > 0) {
					var modifierKeysString = gestureString.Substring(0, keysStart);
					var keyString = gestureString.Substring(keysStart + 1);
					
					var modifierKeys = (ModifierKeys)modidierKeysConverter.ConvertFrom(context, culture, modifierKeysString);
					var key = (Key)keyConverter.ConvertFrom(context, culture, keyString);
					
					return new PartialKeyGesture(key, modifierKeys);
				}
				
				try {
					var modifierKeys = (ModifierKeys)modidierKeysConverter.ConvertFrom(context, culture, gestureString);
					
					return new PartialKeyGesture(modifierKeys);
				}
				catch (NotSupportedException)
				{ }
				
				
				try
				{
					var key = (Key)keyConverter.ConvertFrom(context, culture, gestureString);
					
					return new PartialKeyGesture(key);
				}
				catch (NotSupportedException)
				{ }
			}
			
			return base.ConvertFrom(context, culture, value);
		}
	}
}
