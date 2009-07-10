using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;

namespace ICSharpCode.ShortcutsManagement.Dialogs
{
    /// <summary>
    /// Notification type enumeration
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// A valid gesture is entered so far
        /// </summary>
        Valid,

        /// <summary>
        /// Gesture is being entered, but is not valid yet
        /// </summary>
        Invalid,

        /// <summary>
        /// Gesture was successfully added to gestures collection
        /// </summary>
        Added,

        /// <summary>
        /// Failed to add gesture to gestures collection
        /// </summary>
        Failed,

        /// <summary>
        /// Notification message is not displayed
        /// </summary>
        None
    }

    /// <summary>
    /// Represents a textbox suited for entering key gestures
    /// </summary>
    public partial class MultiKeyGestureTextBox : UserControl
    {
        /// <summary>
        /// Identifies <see cref="TextBoxBorderThickness"/> dependency property
        /// </summary>
        public static readonly DependencyProperty TextBoxBorderThicknessProperty = DependencyProperty.Register(
            "TextBoxBorderThickness",
            typeof(int),
            typeof(MultiKeyGestureTextBox),
            new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Specifies text box border thickness
        /// </summary>
        public int TextBoxBorderThickness
        {
            get {
                return (int)GetValue(TextBoxBorderThicknessProperty);
            }
            set {
                SetValue(TextBoxBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Key gesture entered in text box
        /// </summary>
        public KeyGesture Gesture
        {
            get {
                if (enteredKeyGestureSequence == null || enteredKeyGestureSequence.Count == 0) {
                    return null;
                }

                if (enteredKeyGestureSequence.Count == 1) {
                    return new PartialKeyGesture(enteredKeyGestureSequence.First());
                }

                return new MultiKeyGesture(enteredKeyGestureSequence);
            }
        }

        /// <summary>
        /// Event which is raised when gesture entered in text box changes
        /// </summary>
        public event EventHandler GestureChanged;

        /// <summary>
        /// Identifies <see cref="NotificationVisibility"/> dependency property
        /// </summary>
        public static readonly DependencyProperty NotificationVisibilityProperty = DependencyProperty.Register(
            "NotificationVisibility",
            typeof(Visibility),
            typeof(MultiKeyGestureTextBox),
            new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Whether notifications are displayed under textbox control
        /// </summary>
        public Visibility NotificationVisibility
        {
            get {
                return (Visibility)GetValue(NotificationVisibilityProperty);
            }
            set {
                SetValue(NotificationVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Identifies <see cref="NotificationType"/> dependency property
        /// </summary>
        public static readonly DependencyProperty NotificationTypeProperty = DependencyProperty.Register(
            "NotificationType",
            typeof(NotificationType),
            typeof(MultiKeyGestureTextBox),
            new FrameworkPropertyMetadata(NotificationType.None, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Specifies displayed notification type
        /// </summary>
        public NotificationType NotificationType
        {
            get {
                return (NotificationType)GetValue(NotificationTypeProperty);
            }
            set {
                SetValue(NotificationTypeProperty, value);
            }
        }

        /// <summary>
        /// Identifies <see cref="NotificationText"/> dependency property
        /// </summary>
        public static readonly DependencyProperty NotificationTextProperty = DependencyProperty.Register(
            "NotificationText",
            typeof(string),
            typeof(MultiKeyGestureTextBox),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Specifies displayed notification text
        /// </summary>
        public string NotificationText
        {
            get {
                return (string)GetValue(NotificationTextProperty);
            }
            set {
                SetValue(NotificationTextProperty, value);
            }
        }

        /// <summary>
        /// Last entered chords
        /// </summary>
        private List<PartialKeyGesture> enteredKeyGestureSequence = new List<PartialKeyGesture>();

        /// <summary>
        /// Time when last successfull chord was entered
        /// </summary>
        private DateTime lastEnterTime = DateTime.Now;

        /// <summary>
        /// Creates instance of <see cref="MultiKeyGestureTextBox"/>
        /// </summary>
        public MultiKeyGestureTextBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Displays notifications under gesture text box
        /// </summary>
        /// <param name="notificationText">Notification text</param>
        /// <param name="type">Notification type</param>
        public void DisplayNotification(string notificationText, NotificationType type)
        {
            NotificationText = notificationText;
            NotificationType = type;
        }

        /// <summary>
        /// Clears all text area, chords and hides notification
        /// </summary>
        public void Clear()
        {
            enteredKeyGestureSequence = new List<PartialKeyGesture>();
            shortcutTextBox.Text = "";
            DisplayNotification("", NotificationType.None);
        }

        /// <summary>
        /// Raised when clicked on "Clear" button to the right from gesture text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearTextBox_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }


        /// <summary>
        /// Raised when text inside textbox changes
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event arguments</param>
        void shortcutTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (GestureChanged != null) {
                GestureChanged.Invoke(sender, new EventArgs());
            }
        }

        /// <summary>
        /// Raised before user presses any key inside text box
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event argument</param>
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            // If a key is holded for a longer time key event is raised repeatedly.
            // We don't want to handle this kind of events
            if (e.IsRepeat) {
                return;
            }

            // If delete or backspace button is pressed
            if (e.Key == Key.Back || e.Key == Key.Delete) {
                Clear();
                return;
            }

            // Check whether time given for chord entry haven't expired yet
            if (DateTime.Now - lastEnterTime > MultiKeyGesture.DelayBetweenChords) {
                if (enteredKeyGestureSequence.Count > 0) {
                    DisplayNotification(StringParser.Parse("${res:ShortcutsManagement.GestureTextBox.TimeExpired}"), NotificationType.Invalid);
                }

                Clear();
                lastEnterTime = DateTime.Now;
            }

            var partialKeyGesture = new PartialKeyGesture(e);

            var lastGesture = enteredKeyGestureSequence.Count > 0 ? enteredKeyGestureSequence.LastOrDefault() : null;
            var isLastGestureSpecialKey = lastGesture != null && (lastGesture.Key >= Key.F1) && (lastGesture.Key <= Key.F24);

            var isLastGestureComplete = lastGesture != null && (lastGesture.Key != Key.None || isLastGestureSpecialKey);
            var isContinuedGesture = lastGesture != null && partialKeyGesture.Modifiers - (partialKeyGesture.Modifiers ^ lastGesture.Modifiers) >= 0;
            
            // If continuing previous chord
            if (!isLastGestureComplete && isContinuedGesture)
            {
                enteredKeyGestureSequence.RemoveAt(enteredKeyGestureSequence.Count - 1);
            }
            
            // If previous chord is unfinished and second chord is already entered
            // start from scratch. 
            else if (!isLastGestureComplete)
            {
                DisplayNotification(StringParser.Parse("${res:ShortcutsManagement.GestureTextBox.SequenceIsNotCoherent}"), NotificationType.Invalid);
                Clear();
            }

            // If successfully finished another chord give more time
            if (partialKeyGesture.Key != Key.None)
            {
                lastEnterTime = DateTime.Now;
            }

            enteredKeyGestureSequence.Add(partialKeyGesture);

            // Create a multi key gesture if entered more than one chord
            if (enteredKeyGestureSequence.Count > 0)
            {
                var multiKeyGesture = new MultiKeyGesture(enteredKeyGestureSequence);
                var multiKeyGestureString = new MultiKeyGestureConverter().ConvertToInvariantString(multiKeyGesture);
                shortcutTextBox.Text = multiKeyGestureString;
            }
            else
            {
                Clear();
            }

            if (!enteredKeyGestureSequence[0].IsFull)
            {
                DisplayNotification(StringParser.Parse("${res:ShortcutsManagement.GestureTextBox.FirstChordIsIncomplete}"), NotificationType.Invalid);
            }
            else if (partialKeyGesture.Key == Key.None)
            {
                DisplayNotification(StringParser.Parse("${res:ShortcutsManagement.GestureTextBox.LastChordIsIncomplete}"), NotificationType.Invalid);
            }
            else
            {
                DisplayNotification(StringParser.Parse("${res:ShortcutsManagement.GestureTextBox.GestureIsValid}"), NotificationType.Valid);
            }
        }
    }
}
