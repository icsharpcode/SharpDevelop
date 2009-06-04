using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ICSharpCode.ShortcutsManagement
{
    /// <summary>
    /// Interaction logic for ShortcutManagementWindow.xaml
    /// </summary>
    public partial class ShortcutManagementWindow : Window
    {
        private ShortcutsProvider shortcutsProvider;
        private Shortcut shortcut;
        private InputGesture lastEnteredInputGesture;

        public ShortcutManagementWindow(Shortcut shortcut, ShortcutsProvider provider)
        {
            DataContext = shortcut;
            this.shortcut = shortcut;
            shortcutsProvider = provider;

            InitializeComponent();
        }

        private void shortcutTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            shortcutTextBox.Text = GesturesHelper.GetGestureFromKeyEventArgument(e);

            lastEnteredInputGesture = null;
            try
            {
                lastEnteredInputGesture = new KeyGesture(e.Key, Keyboard.Modifiers);
            }
            catch (NotSupportedException)
            { }
        }

        private void removeGestureButton_Click(object sender, RoutedEventArgs e)
        {
            var tag = ((Button) sender).Tag as InputGesture;
            shortcut.Gestures.Remove(tag);
        }

        private void addGestureButton_Click(object sender, RoutedEventArgs e)
        {
            if(lastEnteredInputGesture != null)
            {
                shortcut.Gestures.Add(lastEnteredInputGesture);
            }
        }
    }
}
