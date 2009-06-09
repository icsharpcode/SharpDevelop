using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ICSharpCode.ShortcutsManagement
{
    /// <summary>
    /// Contains <see cref="TextBlock"/> attached properties
    /// </summary>
    public static class TextBlockBehavior
    {
    	/// <summary>
        /// Get "TextBlockBehavior.SearchedText" attached property value
    	/// </summary>
    	/// <param name="textBlock">Attached property host</param>
    	/// <returns>Attached property value</returns>
        public static string GetSearchedText(TextBlock textBlock)
        {
            return (string)textBlock.GetValue(SearchedTextProperty);
        }

        /// <summary>
        /// Set "TextBlockBehavior.SearchedText" attached property value
        /// </summary>
        /// <param name="textBlock">Attached property host</param>
        /// <param name="value">New attached property value</param>
        public static void SetSearchedText(TextBlock textBlock, string value)
        {
            textBlock.SetValue(SearchedTextProperty, value);
        }
        
        /// <summary>
        /// Identifies SearchedText dependency property
        /// </summary>
        public static readonly DependencyProperty SearchedTextProperty =
            DependencyProperty.RegisterAttached(
                "SearchedText",
                typeof(string),
                typeof(TextBlockBehavior),
                new UIPropertyMetadata(null, OnSearchedTextChanged));

        /// <summary>
        /// On SearchedText changed highlight text in TextBlock which matches 
        /// attached property value
        /// </summary>
        /// <param name="depObj"></param>
        /// <param name="e"></param>
        private static void OnSearchedTextChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = (TextBlock) depObj;
            var textBlockText = textBlock.Text;

            // Remove all text from text block
            textBlock.Inlines.Clear();

            // Split text contained in text block to three parts: before the match, the match and after the match
            var matches = Regex.Matches(textBlockText, @"(.*)(" + Regex.Escape((string)e.NewValue) + @")(.*)", RegexOptions.IgnoreCase);
            if (matches.Count > 0) {
                foreach (Match match in matches) {
                    // Add what is found before match back to the block without modifications
                    var matchedTextPrefix = match.Groups[1].Value;
                    if (!string.IsNullOrEmpty(matchedTextPrefix)) {
                        textBlock.Inlines.Add(new Run(matchedTextPrefix));
                    }

                    // Higlight the match and add back to textblock
                    var matchedText = match.Groups[2].Value;
                    if (!string.IsNullOrEmpty(matchedText)) {
                        var matchedRun = new Run(matchedText);
                        
                        matchedRun.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CEDEF7"));
                        matchedRun.Foreground = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#000000"));
                        textBlock.Inlines.Add(matchedRun);
                    }

                    // Add what is found after match back to the block without modifications
                    var matchedTextSuffix = match.Groups[3].Value;
                    if (!string.IsNullOrEmpty(matchedTextSuffix)) {
                        textBlock.Inlines.Add(new Run(matchedTextSuffix));
                    }
                }
            } else {
                // If filter string is not found put the original text back without modifications
                textBlock.Inlines.Add(new Run(textBlockText));
            }
        }
    }
}
