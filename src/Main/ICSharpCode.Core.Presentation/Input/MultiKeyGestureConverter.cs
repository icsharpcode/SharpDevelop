using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace ICSharpCode.Core.Presentation
{
    /// <summary>
    /// Converts <see cref="MultiKeyGesture"/> instance to and from other types
    /// </summary>
    public class MultiKeyGestureConverter : TypeConverter
    {
        private readonly PartialKeyGestureConverter partialKeyGestureConverter = new PartialKeyGestureConverter();

        /// <summary>
        /// Determines whether object of specified type can be converted to instance of <see cref="MultiKeyGesture"/>
        /// </summary>
        /// <param name="context">A format context that provides information about the environment from which this converter is being invoked.</param>
        /// <param name="sourceType">The type being evaluated for conversion.</param>
        /// <returns>True if this converter can perform the operation; otherwise, false.</returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if(sourceType == typeof(string))
			{
			    return true;
			}

		    return base.CanConvertFrom(context, sourceType);
		}

        /// <summary>
        /// Determines whether an instance of <see cref="MultiKeyGesture"/> can be converted to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">A format context that provides information about the environment from which this converter is being invoked.</param>
        /// <param name="destinationType">The type being evaluated for conversion. </param>
        /// <returns>True if this converter can perform the operation; otherwise, false. </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, destinationType);
        }

        /// <summary>
        /// Attempts to convert a <see cref="MultiKeyGesture"/> to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">A format context that provides information about the environment from which this converter is being invoked.</param>
        /// <param name="culture">Culture specific information. </param>
        /// <param name="value">The object to convert</param>
        /// <param name="destinationType">The type wich instance of <see cref="PartialKeyGesture"/> is being converted to.</param>
        /// <returns>The converted object, or an empty string if value is a null reference</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value == null && destinationType == typeof(string))
            {
                return String.Empty;
            }

            var MultiKeyGesture = value as MultiKeyGesture;
            if (MultiKeyGesture != null && destinationType == typeof(string))
            {
                var partialKeyGestureStrings =  MultiKeyGesture.Chords.Select(gesture => (string)partialKeyGestureConverter.ConvertTo(context, culture, gesture, typeof(string))).ToArray();
                var MultiKeyGestureString = string.Join(",", partialKeyGestureStrings);

                return MultiKeyGestureString;
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Attempts to convert the specified object to a <see cref="MultiKeyGesture"/>, using the specified context.
        /// </summary>
        /// <param name="context">A format context that provides information about the environment from which this converter is being invoked.</param>
        /// <param name="culture">Culture specific information.</param>
        /// <param name="value">The object to convert</param>
        /// <returns>The converted object.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
		    var gestureString = value as string;
            if (gestureString != null) {
                var keyGestures = new List<PartialKeyGesture>();

                var keyGestureStrings = gestureString.Split(',');
                foreach (var keyGestureString in keyGestureStrings) {
                    var partialKeyGesture = (PartialKeyGesture)new PartialKeyGestureConverter().ConvertFrom(context, culture, keyGestureString);
                    keyGestures.Add(partialKeyGesture);
                }

                return new MultiKeyGesture(keyGestures);
            }

		    return base.ConvertFrom(context, culture, value);
		}
    }
}
