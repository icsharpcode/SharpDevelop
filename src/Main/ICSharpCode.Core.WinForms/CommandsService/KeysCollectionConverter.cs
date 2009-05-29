using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace ICSharpCode.Core.WinForms
{
	/// <summary>
	/// Converts objects of different types to array of keys and 
	/// convert enumerable back to objects of different type
	/// </summary>
	public class KeysCollectionConverter : TypeConverter
	{
		/// <summary>
		/// Returns a value indicating whether this converter can convert an object in the 
		/// specified source type to the native type of the converter.
		/// </summary>
		/// <param name="context">Type descriptor context</param>
		/// <param name="sourceType">Source type</param>
		/// <returns>Array of keys</returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if (sourceType == typeof(string)) {
				return true;
			} if (sourceType == typeof(string[])) {
				return true;
			}
			
			return base.CanConvertFrom(context, sourceType);
		}
		
		/// <summary>
		/// Returns a value indicating whether this converter can convert a 
		/// specified object to the specified destination type.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that provides a format context, which can be used to extract additional information about the environment this converter is being invoked from. This parameter or properties of this parameter can be a null reference</param>
		/// <param name="destinationType">Convertion destination type</param>
		/// <returns>Destination object</returns>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(string) || destinationType == typeof(string[])) {
				return true;
			}
			
			return base.CanConvertTo(context, destinationType);
		}

		/// <summary>
		/// Convert from source object to array of keys
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that provides a format context, which can be used to extract additional information about the environment this converter is being invoked from. This parameter or properties of this parameter can be a null reference </param>
		/// <param name="culture">Locale information which can influence convertion</param>
		/// <param name="value">Array of Keys</param>
		/// <returns></returns>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value is string || value is string[]) {
				var gestures = new List<Keys>();
				
				string[] serializedGestures;
				if(value is string) {
					serializedGestures = Regex.Split((string)value, @"\s*\|\s*");
				} else {
					serializedGestures = (string[])value;
				}
				
				foreach(var serializedGesture in serializedGestures) {
					if(string.IsNullOrEmpty(serializedGesture)) continue;
					
					gestures.Add((Keys)new KeysConverter().ConvertFromInvariantString(serializedGesture));
				}
				
				return gestures.ToArray();
			}
			
			return base.ConvertFrom(context, culture, value);
		}
		
		/// <summary>
		/// Converts array of keys to the specified destination type
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that provides a format context, which can be used to extract additional information about the environment this converter is being invoked from. This parameter or properties of this parameter can be a null reference </param>
		/// <param name="culture">Locale information which can influence convertion</param>
		/// <param name="value">Array of keys</param>
		/// <param name="destinationType">Convertion destination type</param>
		/// <returns></returns>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {  
			if(destinationType == typeof(string) || destinationType == typeof(string[]))
			{
				if(value == null) return null;
			
				var keysCollection = (IEnumerable<Keys>)value;
				var serializedKeysCollection = new List<string>();
				
				foreach(var gesture in keysCollection) {
					var serializedGesture = new KeysConverter().ConvertToInvariantString(gesture);
					serializedKeysCollection.Add(serializedGesture);
				}
				
				if(destinationType == typeof(string[])) {
					return serializedKeysCollection.ToArray();
				} else if(destinationType == typeof(string)) {
					var sb = new StringBuilder();
					foreach(var serializedGesture in serializedKeysCollection) {
						sb.AppendFormat("{0} | ", serializedGesture);
					}
					
					return sb.Length >= 3 ? sb.ToString(0, sb.Length - 3) : sb.ToString();
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
