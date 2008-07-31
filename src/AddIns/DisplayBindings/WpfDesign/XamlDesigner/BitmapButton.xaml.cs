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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ICSharpCode.XamlDesigner
{
    public partial class BitmapButton
    {
        public BitmapButton()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string ImageHover {
            get { return "Images/" + GetType().Name + ".Hover.png"; }
        }

        public string ImageNormal {
            get { return "Images/" + GetType().Name + ".Normal.png"; }
        }

        public string ImagePressed {
            get { return "Images/" + GetType().Name + ".Pressed.png"; }
        }

        public string ImageDisabled {
            get { return "Images/" + GetType().Name + ".Disabled.png"; }
        }
    }

    class CloseButton : BitmapButton
    {
    }
}
