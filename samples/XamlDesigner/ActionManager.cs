using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Markup;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Threading;

namespace ICSharpCode.XamlDesigner
{
	public enum DisableMode
	{
		Disable, Collapse
	}

	public class ActionArgs
	{
		public FrameworkElement Element { get; internal set; }
	}

	public delegate void ActionHandler(ActionArgs e);
	public delegate bool CanActionHandler(ActionArgs e);

    public class Action
    {
		public Action()
		{
			Elements = new List<FrameworkElement>();
			Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Loaded, new System.Action(RegisterRequery));
		}

		void RegisterRequery()
		{
			CommandManager.RequerySuggested += new EventHandler(CommandManager_RequerySuggested);
		}

		void CommandManager_RequerySuggested(object sender, EventArgs e)
		{
			UpdateElements();
		}

        public string Text { get; set; }
        public Shortcut Shortcut { get; set; }
		public ImageSource IconSource { get; set; }
        public DisableMode DisableMode { get; set; }
        public event ActionHandler Executed;
        public event CanActionHandler CanExecute;
		public List<FrameworkElement> Elements { get; private set; }

		public object ExecuteHost {
            get {
				if (Executed != null) return Executed.Target;
				return null;
            }
        }

		public void AttachTo(FrameworkElement f)
		{
			SetText(f);
			SetShortcut(f);
			SetIcon(f);
			SetEvent(f);

			Elements.Add(f);
		}

		public void UpdateElements()
		{
			if (CanExecute != null) {
				foreach (var f in Elements) {
					f.IsEnabled = CanExecute(new ActionArgs() { Element = f });
				}
			}
		}

		void SetText(FrameworkElement f)
		{
			if (Text == null) return;
			if (f is ContentControl) {
				(f as ContentControl).Content = Text;
			}
			else if (f is HeaderedItemsControl) {
				(f as HeaderedItemsControl).Header = Text;
			}
		}

		void SetShortcut(FrameworkElement f)
		{
			if (Shortcut == null) return;
			if (f is MenuItem) {
				(f as MenuItem).InputGestureText = Shortcut.ToString();
			}
			if (ExecuteHost == null) return;
			(ExecuteHost as IInputElement).KeyDown += delegate(object sender, KeyEventArgs e) {
                if (e.Key == Shortcut.Key && Keyboard.Modifiers == Shortcut.Modifiers) {
                    Executed(new ActionArgs() { Element = f });
                }
            };
		}

		void SetIcon(FrameworkElement f)
		{
			if (IconSource == null) return;
			if (f is MenuItem) {
				(f as MenuItem).Icon = new Image() { Source = IconSource };
			}
		}

		void SetEvent(FrameworkElement f)
		{
			if (Executed == null) return;
			f.PreviewMouseLeftButtonUp += delegate {
				Executed(new ActionArgs() { Element = f });
			};
		}
    }

    public static class ActionManager
    {
		public static void SetAction(DependencyObject obj, object value)
		{
			Action a = value as Action;
			if (a == null) {
			    if (obj is FrameworkElement) {
			        a = (Action)(obj as FrameworkElement).FindResource(value);
			    }
			}
			a.AttachTo(obj as FrameworkElement);
		}
    }

    [TypeConverter(typeof(ShortcutConverter))]
    public class Shortcut
    {
        public ModifierKeys Modifiers;
        public Key Key;

        static KeyConverter KeyConverter = new KeyConverter();
        static ModifierKeysConverter ModifierKeysConverter = new ModifierKeysConverter();

        public static Shortcut FromString(string s)
        {
            var result = new Shortcut();
            var pos = s.LastIndexOf('+');
            if (pos < 0) {
                result.Key = (Key)KeyConverter.ConvertFromString(s);
            }
            else {
                result.Modifiers = (ModifierKeys)ModifierKeysConverter.ConvertFromString(s.Substring(0, pos));
                result.Key = (Key)KeyConverter.ConvertFromString(s.Substring(pos + 1));
            }
            return result;
        }

        public override string ToString()
        {
            if (Modifiers == ModifierKeys.None) return KeyConverter.ConvertToString(Key);
            return ModifierKeysConverter.ConvertToString(Modifiers) + "+" + KeyConverter.ConvertToString(Key);
        }
    }

    public class ShortcutConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return Shortcut.FromString((string)value);
        }
    }
}
