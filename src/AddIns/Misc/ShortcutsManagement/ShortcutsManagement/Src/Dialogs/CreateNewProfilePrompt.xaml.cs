using System.Collections.Generic;
using System.Windows;
using ICSharpCode.Core.Presentation;

namespace ICSharpCode.ShortcutsManagement.Dialogs
{
    /// <summary>
    /// Window prompting user to enter new or existing profile name
    /// and base profile
    /// </summary>
    public partial class CreateNewProfilePrompt : Window
    {
        /// <summary>
        /// Identifies <see cref="BaseProfile"/> dependency property
        /// </summary>
        public static readonly DependencyProperty BaseProfileProperty = DependencyProperty.Register(
            "BaseProfile",
            typeof(UserGestureProfile),
            typeof(CreateNewProfilePrompt),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Specifies profile which new profile will be based on
        /// </summary>
        public UserGestureProfile BaseProfile
        {
            get
            {
                return (UserGestureProfile)GetValue(BaseProfileProperty);
            }
            set
            {
                SetValue(BaseProfileProperty, value);
            }
        }
        
        /// <summary>
        /// Identifies <see cref="BaseProfilesVisibility"/> dependency property
        /// </summary>
        public static readonly DependencyProperty BaseProfilesVisibilityProperty = DependencyProperty.Register(
            "BaseProfilesVisibility",
            typeof(Visibility),
            typeof(CreateNewProfilePrompt),
            new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Base profiles combo box visibility
        /// </summary>
        public Visibility BaseProfilesVisibility
        {
            get
            {
                return (Visibility)GetValue(BaseProfilesVisibilityProperty);
            }
            set
            {
                SetValue(BaseProfilesVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Identifies <see cref="AvailableBaseProfiles"/> dependency property
        /// </summary>
        public static readonly DependencyProperty AvailableBaseProfilesProperty = DependencyProperty.Register(
            "AvailableBaseProfiles",
            typeof(ICollection<UserGestureProfile>),
            typeof(CreateNewProfilePrompt),
            new FrameworkPropertyMetadata(new List<UserGestureProfile>(), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Profiles user can choose from when selecting base profile
        /// </summary>
        public ICollection<UserGestureProfile> AvailableBaseProfiles
        {
            get
            {
                return (ICollection<UserGestureProfile>)GetValue(AvailableBaseProfilesProperty);
            }
            set
            {
                SetValue(AvailableBaseProfilesProperty, value);
            }
        }

        /// <summary>
        /// Identifies <see cref="Text"/> dependency property
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(CreateNewProfilePrompt),
            new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Specifies text entered in window text box
        /// </summary>
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Creates new instance of <see cref="CreateNewProfilePrompt"/>
        /// </summary>
        public CreateNewProfilePrompt()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Occurs when user clicks on OK button
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event argument</param>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Occurs when user clicks on Cancel buttons
        /// </summary>
        /// <param name="sender">Sender obect</param>
        /// <param name="e">Event argument</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
