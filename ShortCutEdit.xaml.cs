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
using System.Windows.Shapes;

namespace Sekretariacik
{
    /// <summary>
    /// Logika interakcji dla klasy Window1.xaml
    /// </summary>
    public partial class ShortCutEdit : Window
    {
        private Shortcuts editedValue;

        public ShortCutEdit(Shortcuts s)
        {

            InitializeComponent();
            editedValue = s;
            switch (s)
            {
                case Shortcuts.Load:
                    this.Title = "Edycja skrótu Wczytaj";
                    break;
                case Shortcuts.Export:
                    this.Title = "Edycja skrótu Export";
                    break;
                case Shortcuts.Edit:
                    this.Title = "Edycja skrótu Edycji Rekordów";
                    break;
                case Shortcuts.AddPhoto:
                    this.Title = "Edycja skrótu Dodawania Zdjęcia";
                    break;
                default:
                    this.Title = "Nieznany skrót";
                    break;

            }
            
        }

        private void Click_CancelShortcut(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Click_SaveShortcut(object sender, RoutedEventArgs e)
        {
            KeyConverter kc = new KeyConverter();
            switch(editedValue) {
                case Shortcuts.Load:
                    Configuration.loadKey = (Key)kc.ConvertFromString(ShortcutValue.Text);
                    this.Close();
                    break;
                case Shortcuts.Export:
                    Configuration.exportKey = (Key)kc.ConvertFromString(ShortcutValue.Text);
                    this.Close();
                    break;
                case Shortcuts.Edit:
                    Configuration.editKey = (Key)kc.ConvertFromString(ShortcutValue.Text);
                    this.Close();
                    break;
                case Shortcuts.AddPhoto:
                    Configuration.photoKey = (Key)kc.ConvertFromString(ShortcutValue.Text);
                    this.Close();
                    break;
            }
        }
    }
}
