using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Storm.NetFramework
{
    /// <summary>
    /// Логика взаимодействия для EditTerm.xaml
    /// </summary>
    public partial class EditTerm : Window
    {
        public EditTerm()
        {
            InitializeComponent();
        }

        private void SaveTerm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool flag = true;

                if (gridMain.RowDefinitions[0].Height.Value > 0 && String.IsNullOrWhiteSpace(this.TextBox1.Text))
                {
                    flag = false;
                    LabelTextBox1.Foreground = Brushes.Red;
                }
                else
                    LabelTextBox1.Foreground = Brushes.Black;

                if (gridMain.RowDefinitions[1].Height.Value > 0 && String.IsNullOrWhiteSpace(this.TextBox2.Text))
                {
                    flag = false;
                    LabelTextBox2.Foreground = Brushes.Red;
                }
                else
                    LabelTextBox2.Foreground = Brushes.Black;

                if (gridMain.RowDefinitions[2].Height.Value > 0 && this.ComboBox1.SelectedValue == null)
                {
                    flag = false;
                    LabelComboBox1.Foreground = Brushes.Red;
                }
                else
                    LabelComboBox1.Foreground = Brushes.Black;

                if (gridMain.RowDefinitions[3].Height.Value > 0 && this.ComboBox2.SelectedValue == null)
                {
                    flag = false;
                    LabelComboBox2.Foreground = Brushes.Red;
                }
                else
                    LabelComboBox2.Foreground = Brushes.Black;

                if (gridMain.RowDefinitions[4].Height.Value > 0 && this.ComboBox3.SelectedValue == null)
                {
                    flag = false;
                    LabelComboBox3.Foreground = Brushes.Red;
                }
                else
                    LabelComboBox3.Foreground = Brushes.Black;

                if (flag)
                {
                    DialogResult = true;
                }
                else
                    MessageBox.Show("Внимание, проверьте правильность ввода!", "Добавление нового термина в словарь", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                //Logging.RecordError(error);
            }

        }
    }
}
