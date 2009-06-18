using System;
using System.ComponentModel;
using System.Windows.Input;

namespace ICSharpCode.Core.Presentation
{
    /// <summary>
    /// Represents input binding which can be invoked by <see cref="MultiKeyGesture"/>
    /// </summary>
    public class MultiKeyBinding : InputBinding
    {
        /// <summary>
        /// Instance of <see cref="MultiKeyGesture"/>
        /// </summary>
        [TypeConverter(typeof(MultiKeyGestureConverter))]
        public override InputGesture Gesture
        {
            get { return base.Gesture as MultiKeyGesture; }
            set
            {
                if (!(value is MultiKeyGesture))
                {
                    throw new ArgumentException();
                }

                base.Gesture = value;
            }
        }
    }
}
