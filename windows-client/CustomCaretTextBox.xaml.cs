using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OpenTerminal
{
    /// <summary>
    /// Interaction logic for CustomCaret.xaml
    /// </summary>
    /// 
    public partial class CustomCaretTextBox : UserControl
    {
        public delegate void OnTextUpdateFunc();
        public OnTextUpdateFunc OnTextUpdate;


        private const string PLACEHOLDER_TEXT = "Start typing to find a quote. Hit <ESC> if you want to clear the text.";
        private SolidColorBrush LIGHT_TEXT_COLOR = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#adadad"));
        private SolidColorBrush TEXT_COLOR = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FDFDFD"));
        public CustomCaretTextBox()
        {
            InitializeComponent();

            this.CustomTextBox.SelectionChanged += (sender, e) => MoveCustomCaret();
            this.CustomTextBox.LostFocus += (sender, e) => Caret.Visibility = Visibility.Collapsed;
            this.CustomTextBox.GotFocus += (sender, e) => Caret.Visibility = Visibility.Visible;


            TextChanged(null, null);
            
        }

        // TODO: deal with: Warning CS0108	'CustomCaretTextBox.Focus()' hides inherited member 'UIElement.Focus()'. Use the new keyword if hiding was intended
        public void Focus()
        {
            this.CustomTextBox.Focus();
        }

        /**
         * This runs on a timer set in the constructor so that 
         * the mutated graph data is updated.
         */
        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            // if it is an exact match we are just being notified of the Text being set on the window initialization
            // which we want to keep so the user can see the placeholder text.
            if (!CustomTextBox.Text.Equals(PLACEHOLDER_TEXT) && CustomTextBox.Text.Contains(PLACEHOLDER_TEXT))
            {
                CustomTextBox.Text = CustomTextBox.Text.Replace(PLACEHOLDER_TEXT, "");
                CustomTextBox.CaretIndex = CustomTextBox.Text.Length;
                CustomTextBox.Foreground = TEXT_COLOR;
            }

            if (CustomTextBox.Text == "")
            {
                CustomTextBox.Text = PLACEHOLDER_TEXT;
                CustomTextBox.CaretIndex = 0;

                CustomTextBox.Foreground = LIGHT_TEXT_COLOR;
            }
        }


        /**
         * Clear the text when escape is pressed.
         */
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CustomTextBox.Text = "";
                CustomTextBox.CaretIndex = 0;
            } else if (e.Key == Key.Return)
            {
                this.OnTextUpdate();
            }
        }

        private void MoveCustomCaret()
        {
            var caretLocation = CustomTextBox.GetRectFromCharacterIndex(CustomTextBox.CaretIndex).Location;

            if (!double.IsInfinity(caretLocation.X))
            {
                Canvas.SetLeft(Caret, caretLocation.X);
            }

            if (!double.IsInfinity(caretLocation.Y))
            {
                Canvas.SetTop(Caret, caretLocation.Y);
            }
        }
    }
}
