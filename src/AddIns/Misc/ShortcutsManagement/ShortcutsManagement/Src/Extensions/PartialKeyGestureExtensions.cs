using System.Collections.Generic;
using System.Windows.Input;
using ICSharpCode.Core.Presentation;
using ICSharpCode.ShortcutsManagement.Data;

namespace ICSharpCode.ShortcutsManagement.Extensions
{
    /// <summary>
    /// <see cref="PartialKeyGesture"/> extensions
    /// </summary>
    public static class PartialKeyGestureExtensions
    {
        /// <summary>
        /// Returns true whether compared gesture matches input gesture template
        /// </summary>
        /// <param name="inputGestureTemplate">Input gesture template</param>
        /// <param name="inputGesture">Compared input gesture</param>
        /// <param name="mode">Match mode</param>
        /// <returns>Returns true if template matches compared geture</returns>
        private static bool IsTemplate(this InputGesture inputGestureTemplate, InputGesture inputGesture, GestureFilterMode mode)
        {
            var multiKeyGesture = inputGesture as MultiKeyGesture;
            var multiKeyGestureTemplate = inputGestureTemplate as MultiKeyGesture;
            var partialKeyGesture = inputGesture as PartialKeyGesture;
            var partialKeyGestureTemplate = inputGestureTemplate as PartialKeyGesture;
            var keyGesture = inputGesture as KeyGesture;
            var keyGestureTemplate = inputGestureTemplate as KeyGesture;

            // If multi key gesture is matched to a template
            if (multiKeyGesture != null && keyGestureTemplate != null)
            {
                // Make a multi key gesture template out of single key gesture template
                if(multiKeyGestureTemplate == null)
                {
                    if (partialKeyGestureTemplate != null)
                    {
                        multiKeyGestureTemplate = new MultiKeyGesture(new List<PartialKeyGesture> { partialKeyGestureTemplate });
                    } 
                    else
                    {
                        partialKeyGestureTemplate = new PartialKeyGesture(keyGestureTemplate);
                        multiKeyGestureTemplate = new MultiKeyGesture(new List<PartialKeyGesture> {partialKeyGestureTemplate});
                    }
                }

                // Check for common missmatches
                if(multiKeyGestureTemplate.Gestures == null 
                    || multiKeyGesture.Gestures == null
                    || multiKeyGestureTemplate.Gestures.Count == 0 
                    || multiKeyGesture.Gestures.Count == 0
                    || multiKeyGestureTemplate.Gestures.Count > multiKeyGesture.Gestures.Count)
                {
                    return false;
                }

                // Stores N previous chords
                var previousChords = new LinkedList<PartialKeyGesture>();
                
                // Search whether chord sequece matches template sequenc in any place
                foreach (PartialKeyGesture chord in multiKeyGesture.Gestures)
                {
                    // Accumulates previous chords
                    previousChords.AddLast(chord);
                    if (previousChords.Count > multiKeyGestureTemplate.Gestures.Count)
                    {
                        previousChords.RemoveFirst();
                    }

                    // Only start comparing with template when needed amount of previous chords where collected
                    if (previousChords.Count == multiKeyGestureTemplate.Gestures.Count)
                    {
                        var multiKeyGestureTemplateEnumerator = multiKeyGestureTemplate.Gestures.GetEnumerator();
                        var previousChordsEnumerator = previousChords.GetEnumerator();

                        multiKeyGestureTemplateEnumerator.Reset();

                        // Compare two chord sequences in a loop
                        var multiKeyGesturesMatch = true;
                        while (previousChordsEnumerator.MoveNext() && multiKeyGestureTemplateEnumerator.MoveNext())
                        {
                            if (multiKeyGestureTemplateEnumerator.Current.Modifiers != previousChordsEnumerator.Current.Modifiers
                                || multiKeyGestureTemplateEnumerator.Current.Key != previousChordsEnumerator.Current.Key)
                            {
                                multiKeyGesturesMatch = false;
                                break;
                            }
                        }

                        if(multiKeyGesturesMatch)
                        {
                            return true;
                        }

                        if (mode == GestureFilterMode.StartsWith)
                        {
                            break;
                        }
                    }
                }
            } 
            else if (multiKeyGestureTemplate == null && keyGesture != null && keyGestureTemplate != null)
            {
                var gestureModifiers = partialKeyGesture != null ? partialKeyGesture.Modifiers : keyGesture.Modifiers;
                var gestureKey = partialKeyGesture != null ? partialKeyGesture.Key : keyGesture.Key;

                var templateModifiers = partialKeyGestureTemplate != null ? partialKeyGestureTemplate.Modifiers : keyGestureTemplate.Modifiers;
                var templateKey = partialKeyGestureTemplate != null ? partialKeyGestureTemplate.Key : keyGestureTemplate.Key;
                
                if (mode == GestureFilterMode.PartlyMatches)
                {
                    return (templateModifiers == ModifierKeys.None || templateModifiers == gestureModifiers)
                        && (templateKey == Key.None || templateKey == gestureKey);
                }

                return templateModifiers == gestureModifiers && templateKey == gestureKey;
            }

            return false;
        }

        /// <summary>
        /// Determines whether this instance of <see cref="InputGesture"/> matches any of provided templates
        /// </summary>
        /// <param name="thisGesture">This input gesture</param>
        /// <param name="inputGestureTemplateCollection">Collection of input gestures templates which (at least one of them) should match this input gesture to return true</param>
        /// <param name="mode">Matching mode</param>
        /// <returns>True if this gsture matches templates collection, otherwise false</returns>
        public static bool MatchesCollection(this InputGesture thisGesture, InputGestureCollection inputGestureTemplateCollection, GestureFilterMode mode)
        {
            foreach (InputGesture gesture in inputGestureTemplateCollection) {
                if (thisGesture.IsTemplate(gesture, mode))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
