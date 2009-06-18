using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace ICSharpCode.Core.Presentation
{
    /// <summary>
    /// Specifies gesture consisting from multiple key gestures
    /// </summary>
    [TypeConverter(typeof(MultiKeyGestureConverter))]
    public class MultiKeyGesture : KeyGesture
    {
        private static readonly List<MultiKeyGesture> allGestures = new List<MultiKeyGesture>();

        // Set sequence indexes of all sequential key gestures to 0
        private static void ResetAllGestures()
        {
            foreach (var gesture in allGestures)
            {
                gesture.currentGestureIndex = 0;
            }
        }

        private readonly ReadOnlyCollection<PartialKeyGesture> gestures;

        private int currentGestureIndex;
        private DateTime lastGestureInput;
        private static readonly TimeSpan maxDelayBetweenGestureInputs = TimeSpan.FromSeconds(2);

        /// <summary>
        /// Creates instance of <see cref="MultiKeyGesture"/> from list of partial key gestures
        /// </summary>
        /// <param name="gestures">Sequence of partial key gestures</param>
        public MultiKeyGesture(IList<PartialKeyGesture> gestures)
            : base(Key.None, ModifierKeys.None) 
        {
            if(gestures == null || gestures.Count == 0)
            {
                throw new ArgumentException("Must specify atleast one gesture");
            }

            this.gestures = new ReadOnlyCollection<PartialKeyGesture>(gestures);

            allGestures.Add(this);
        }

        /// <summary>
        /// Sequence of partial key gestures which describe this sequential key gesture
        /// </summary>
        public ICollection<PartialKeyGesture> Gestures
        {
            get
            {
                return gestures;
            }
        }

        /// <summary>
        /// Determines whether key event arguments matches this instance of <see cref="MultiKeyGesture"/>
        /// </summary>
        /// <param name="targetElement">The target</param>
        /// <param name="inputEventArgs">Input event arguments</param>
        /// <returns>True if the event data matches this <see cref="MultiKeyGesture"/>; otherwise, false. </returns>
        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            var keyEventArgs = inputEventArgs as KeyEventArgs;
            
            if (keyEventArgs == null)
            {
                return false;
            }
            
            if (keyEventArgs.IsRepeat)
            {
                return false;
            }

            // Checks wheter it is not too long from last match
            var isOnTime = (DateTime.Now - lastGestureInput) <= maxDelayBetweenGestureInputs;
            
            // If strict match is on time
            var strictMatch = gestures[currentGestureIndex].StrictlyMatches(targetElement, inputEventArgs);
            if (strictMatch && (currentGestureIndex == 0 || isOnTime))
            {
                currentGestureIndex++;
                lastGestureInput = DateTime.Now;
                inputEventArgs.Handled = true;

                // Check whether this is last gesture in sequence
                if(currentGestureIndex == gestures.Count)
                {
                    ResetAllGestures();
                    return true;
                }

                return false;
            }

            // If partial match is on time allow continue without reseting current gesture index
            var partialMatch = gestures[currentGestureIndex].Matches(targetElement, inputEventArgs);
            if (partialMatch && currentGestureIndex > 0 && isOnTime)
            {
                return false;
            }

            currentGestureIndex = 0;
            return false;
        }
    }
}
