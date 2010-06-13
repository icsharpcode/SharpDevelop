using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace AvalonDock
{
    public sealed class PaneCommands
    {
        static object syncRoot = new object();




        private static RoutedUICommand dockCommand = null;

        /// <summary>
        /// Dock <see cref="Pane"/> to container <see cref="DockingManager"/>
        /// </summary>
        public static RoutedUICommand Dock
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == dockCommand)
                    {
                        dockCommand = new RoutedUICommand("Dock", "Dock", typeof(PaneCommands));
                    }
                }
                return dockCommand;
            }
        }

    }
}
