using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace memo_leak_managed
{
    /// <summary>
    /// Interaction logic for SomeDialog.xaml
    /// </summary>
    public partial class SomeDialog : Window
    {
        public SomeDialog(MainWindow main)
        {
            InitializeComponent();

            main.Closing += new CancelEventHandler(Main_Closing);
            main.txtTB.TextChanged += new TextChangedEventHandler(txtTB_TextChanged);
        }

        private void txtTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in ((TextBox)sender).Text.Reverse())
            {
                builder.Append(item);
            }
        }

        private void Main_Closing(object sender, CancelEventArgs e)
        {
            Close();
        }
    }
}
