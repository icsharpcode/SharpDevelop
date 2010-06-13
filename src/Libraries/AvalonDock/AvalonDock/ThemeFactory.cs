//Copyright (c) 2009, Juergen Schildmann
//Copyright (c) 2007-2010, Adolfo Marinucci
//All rights reserved.

//Redistribution and use in source and binary forms, with or without modification, 
//are permitted provided that the following conditions are met:
//
//* Redistributions of source code must retain the above copyright notice, 
//  this list of conditions and the following disclaimer.
//* Redistributions in binary form must reproduce the above copyright notice, 
//  this list of conditions and the following disclaimer in the documentation 
//  and/or other materials provided with the distribution.
//* Neither the name of Adolfo Marinucci nor the names of its contributors may 
//  be used to endorse or promote products derived from this software without 
//  specific prior written permission.
//
//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
//AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
//IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, 
//INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, 
//PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
//HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
//OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, 
//EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 


using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace AvalonDock
{
    /// <summary>
    /// Is used for color-support to change the colors depending on a base theme.
    /// </summary>
    public sealed class ThemeFactory
    {
        /// <summary>
        /// Change the theme to one from AvalonDock.
        /// </summary>
        /// <param name="theme">for example: "aero.normalcolor" (default style)</param>
        public static void ChangeTheme(string theme)
        {
            ResourceDictionary rd = new ResourceDictionary();
            rd.Source = new Uri("/AvalonDock;component/themes/" + theme + ".xaml", UriKind.RelativeOrAbsolute);

            // first search and remove old one
            ResetTheme();

            Application.Current.Resources.MergedDictionaries.Add(rd);
        }

        /// <summary>
        /// Change the theme to one from AvalonDock.
        /// </summary>
        /// <param name="theme">for example: /AvalonDock;component/themes/aero.normalcolor.xaml" (default style)</param>
        public static void ChangeTheme(Uri themeUri)
        {
            ResourceDictionary rd = new ResourceDictionary();
            rd.Source = themeUri;

            // first search and remove old one
            ResetTheme();

            Application.Current.Resources.MergedDictionaries.Add(rd);
        }

        /// <summary>
        /// Change the colors based on the aero-theme from AvalonDock.
        /// <para>
        /// <example>Example: ChangeColors(Colors.DarkGreen)</example>
        /// </para>
        /// </summary>
        /// <param name="color">the new Color</param>
        public static void ChangeColors(Color color)
        {
            ChangeColors("aero.normalcolor", color);
        }

        /// <summary>
        /// Change the colors based on a theme-name from AvalonDock.
        /// <para>
        /// <example>Example: ChangeColors("classic", Colors.DarkGreen)</example>
        /// </para>
        /// </summary>
        /// <param name="baseTheme">the string of the base theme we want to change</param>
        /// <param name="color">the new Color</param>
        public static void ChangeColors(string baseTheme, Color color)
        {
            ResourceDictionary rd = new ResourceDictionary();
            rd.Source = new Uri("/AvalonDock;component/themes/" + baseTheme + ".xaml", UriKind.RelativeOrAbsolute);

            ChangeKeysInResourceDictionary(rd, color);
            foreach (ResourceDictionary rd2 in rd.MergedDictionaries)
            {
                ChangeKeysInResourceDictionary(rd2, color);
            }

            ResetTheme();

            Application.Current.Resources.MergedDictionaries.Add(rd);
        }

        /// <summary>
        /// Reset custom colors to theme defaults
        /// </summary>
        public static void ResetTheme()
        {
//-            foreach (ResourceDictionary res in Application.Current.Resources.MergedDictionaries)
            ResourceDictionary res = GetActualResourceDictionary();
            if (res != null)
                Application.Current.Resources.MergedDictionaries.Remove(res);
        }

        /// <summary>
        /// Change a specified brush inside the actual theme.
        /// Look at AvalonDockBrushes.cs for possible values.
        /// </summary>
        /// <param name="brushName">an AvalonDockBrushes value</param>
        /// <param name="brush">The new brush. It can be every brush type that is derived from Brush-class.</param>
        public static void ChangeBrush(AvalonDockBrushes brushName, Brush brush)
        {
            ChangeBrush(brushName.ToString(), brush);
        }

        /// <summary>
        /// Change a specified brush inside the actual theme.
        /// </summary>
        /// <param name="brushName">a brush name</param>
        /// <param name="brush">The new brush. It can be every brush type that is derived from Brush-class.</param>
        public static void ChangeBrush(string brushName, Brush brush)
        {
            // get the actual ResourceDictionary
            ResourceDictionary rd = GetActualResourceDictionary();
            if (rd == null)
             {
//-                string source = res.Source.ToString();
//-                if (source.Contains("/AvalonDock;component/themes/"))
                ChangeTheme("aero.normalcolor");
                rd = GetActualResourceDictionary();
            }

            if (rd != null)
            {
                foreach (ResourceDictionary rd2 in rd.MergedDictionaries)
                 {
//-                    Application.Current.Resources.MergedDictionaries.Remove(res);
//-                    break;
                    foreach (object key in rd2.Keys)
                    {
                        object item = rd2[key];
                        string keyTypeName = key.GetType().Name;

                        string str = "";
                        switch (keyTypeName)
                        {
                            case "ComponentResourceKey":
                                str = ((ComponentResourceKey)key).ResourceId.ToString();
                                break;
                            case "String":
                                str = (string)key;
                                break;
                        }
                        if (str == brushName)
                        {
                            rd[key] = brush;
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Searches for keys in the ResourceDictionary for brushes and changes the color-values
        /// </summary>
        /// <param name="rd">the ResourceDictionary</param>
        /// <param name="color">the new Color</param>
        static void ChangeKeysInResourceDictionary(ResourceDictionary rd, Color color)
        {
            foreach (object key in rd.Keys)
            {
                object item = rd[key];

                if (item is SolidColorBrush)
                {
                    SolidColorBrush sb = item as SolidColorBrush;
                    sb.Color = GetColor(sb.Color, color);
                }
                else if (item is LinearGradientBrush)
                {
                    LinearGradientBrush lg = item as LinearGradientBrush;
                    foreach (GradientStop gs in lg.GradientStops)
                    {
                        gs.Color = GetColor(gs.Color, color);
                    }
                }
                else if (item is RadialGradientBrush)
                {
                    RadialGradientBrush rb = item as RadialGradientBrush;
                    foreach (GradientStop gs in rb.GradientStops)
                    {
                        gs.Color = GetColor(gs.Color, color);
                    }
                }
            }
        }
        
        static ResourceDictionary GetActualResourceDictionary()
        {
            // get the actual ResourceDictionary
            foreach (ResourceDictionary res in Application.Current.Resources.MergedDictionaries)
            {
                if (res.Source != null)
                {
                    string source = res.Source.ToString();
                    if (source.Contains("/AvalonDock;component/themes/") ||
                        source.Contains("/AvalonDock.Themes;component/themes/"))
                    {
                        return res;
                    }
                }
            }
            return null;
        }

        static Color GetColor(Color c, Color newCol)
        {
            if (c.A == 0) return c;

            // get brightness for RGB values
            byte brighness = (byte)(c.R * 0.2126 + c.G * 0.7152 + c.B * 0.0722);

            return Color.FromArgb(c.A, 
                            GetSmoothColor(brighness, newCol.R), 
                            GetSmoothColor(brighness, newCol.G), 
                            GetSmoothColor(brighness, newCol.B));
        }

        static byte GetSmoothColor(int a, int b)
        {
            int c = a * b / 255;
            return (byte)(c + a * (255 - ((255 - a) * (255 - b) / 255) - c) / 255);
        }

        static Color GetSmoothColor(Color c, Color light)
        {
            return Color.FromArgb(c.A, GetSmoothColor(c.R, light.R), GetSmoothColor(c.G, light.G), GetSmoothColor(c.B, light.B));
        }
    }
}
