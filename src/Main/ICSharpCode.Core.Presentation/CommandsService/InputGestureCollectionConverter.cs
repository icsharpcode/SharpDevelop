using System;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Converts input gesture from text representation to object and vice versa
	/// 
	/// Will be able to convert mouse and key gestures and handle gestures collections
	/// </summary>
	public class InputGestureCollectionConverter : TypeConverter
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
			if (destinationType == typeof(string)) {
				return true;
			}
			
			return base.CanConvertTo(context, destinationType);
		}

		/// <summary>
		/// Convert from object to input gesture collection
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that provides a format context, which can be used to extract additional information about the environment this converter is being invoked from. This parameter or properties of this parameter can be a null reference </param>
		/// <param name="culture">Locale information which can influence convertion</param>
		/// <param name="value">Input gesture collection</param>
		/// <returns></returns>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if (value is string) {
				var gestures = new InputGestureCollection();
				var serializedGestures = Regex.Split((string)value, @"\s*\;\s*");
				
				foreach(var serializedGesture in serializedGestures) {
					if(string.IsNullOrEmpty(serializedGesture)) continue;
					
					if(serializedGesture.IndexOf(",") < 0) {
						var keyGesture = (InputGesture)new KeyGestureConverter().ConvertFromString(context, culture, serializedGesture);
						gestures.Add(keyGesture);
					} else {
						var multiKeyGestyre = (InputGesture)new MultiKeyGestureConverter().ConvertFromString(context, culture, serializedGesture);
						gestures.Add(multiKeyGestyre);
					}
				}
				
				return gestures;
			}
			
			return base.ConvertFrom(context, culture, value);
		}
		
		/// <summary>
		/// Converts the input gesture collection to the specified destination type
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that provides a format context, which can be used to extract additional information about the environment this converter is being invoked from. This parameter or properties of this parameter can be a null reference </param>
		/// <param name="culture">Locale information which can influence convertion</param>
		/// <param name="value">Input gesture collection</param>
		/// <param name="destinationType">Convertion destination type</param>
		/// <returns></returns>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {  
			if(destinationType == typeof(string)) {
				var sb = new StringBuilder();
				var gesturesCollection = (InputGestureCollection)value;
				foreach(var gesture in gesturesCollection) {
					string serializedGesture;
					if (gesture is MultiKeyGesture) {
						serializedGesture = (string)new MultiKeyGestureConverter().ConvertTo(context, culture, gesture, typeof(string));
                    } else if (gesture is PartialKeyGesture) {
                        serializedGesture = (string)new PartialKeyGestureConverter().ConvertTo(context, culture, gesture, typeof(string));
                    } else if (gesture is KeyGesture) {
						serializedGesture = (string)new KeyGestureConverter().ConvertTo(context, culture, gesture, typeof(string));
					} else if (gesture is MouseGesture) {
						serializedGesture = (string)new MouseGestureConverter().ConvertTo(context, culture, gesture, typeof(string));
					} else {
						continue;
					}
					
					sb.AppendFormat("{0};", serializedGesture);
				}
				
				return sb.Length >= 3 ? sb.ToString(0, sb.Length - 1) : sb.ToString();
			}
			
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}