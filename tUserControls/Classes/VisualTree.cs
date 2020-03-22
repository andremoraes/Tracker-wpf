using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Tracker.UserControls.Classes
{
    public static class VisualTree
    {
        public static ComboBox SearchVisualTreeForCombo(DependencyObject targetElement, string CboName)
        {
            var count = VisualTreeHelper.GetChildrenCount(targetElement);
            if (count == 0)
                return null;

            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(targetElement, i);
                if (child is ComboBox)
                {
                    ComboBox Cbo = (ComboBox)child;

                    if (Cbo.Name == CboName)
                    {
                        //myItems.Foreground = new SolidColorBrush(Colors.Green);
                        return Cbo;
                    }
                }
                else
                {
                    var child_ = SearchVisualTreeForCombo(child, CboName);
                    if (child_ is ComboBox)
                    {
                        ComboBox Cbo = (ComboBox)child_;

                        if (Cbo.Name == CboName)
                        {
                            //myItems.Foreground = new SolidColorBrush(Colors.Green);
                            return Cbo;
                        }
                    }
                }
            }
            return null;
        }
        public static Button SearchVisualTreeForButton(DependencyObject targetElement, string BtnName)
        {
            var count = VisualTreeHelper.GetChildrenCount(targetElement);
            if (count == 0)
                return null;

            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(targetElement, i);
                if (child is Button)
                {
                    Button Btn = (Button)child;

                    if (Btn.Name == BtnName)
                    {
                        //myItems.Foreground = new SolidColorBrush(Colors.Green);
                        return Btn;
                    }
                }
                else
                {
                    var child_ = SearchVisualTreeForButton(child, BtnName);
                    if (child_ is Button)
                    {
                        Button Btn = (Button)child_;

                        if (Btn.Name == BtnName)
                        {
                            //myItems.Foreground = new SolidColorBrush(Colors.Green);
                            return Btn;
                        }
                    }
                }
            }
            return null;
        }
        public static T FindFirstElementInVisualTree<T>(DependencyObject parentElement) where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(parentElement);
            if (count == 0)
                return null;

            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parentElement, i);

                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    var result = FindFirstElementInVisualTree<T>(child);
                    if (result != null)
                        return result;

                }
            }
            return null;
        }

    }
}
