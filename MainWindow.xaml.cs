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
using CsvHelper;
using System.IO;
using System.Globalization;
using Microsoft.Win32;
using System.Collections.ObjectModel;

/// <summary>
///  do wywalenia
using System.Diagnostics;
using System.Data;
using System.Reflection;
using CsvHelper.Configuration;

using System.Windows.Controls.Primitives;
using Newtonsoft.Json;
/// </summary>
namespace Sekretariacik
{
    public class Configuration
    {
        public static string DATE_FORMAT = "dd/MM/yyyy";
        public static Key loadKey = Key.O;
        public static Key exportKey = Key.S;
        public static Key photoKey = Key.F;
        public static Key editKey = Key.E;
        public static string PROGNAME = "Sekretariacik (c) Filip Jurczuk v0.1";
    }
    public enum DateSearch
    {
        Before,
        After,
        NoDateSearch
    }
    public enum Shortcuts
    {
        Load,
        Export,
        Edit,
        AddPhoto
    }
    /// Logika interakcji dla klasy MainWindow.xaml
    public partial class MainWindow : Window
    {
 
        /*
         * Zmienne globalne
         */
        List<Uczen> users = new List<Uczen>();
        List<Uczen> usersFromFile = new List<Uczen>();
        List<Pracownik> pracownicy = new List<Pracownik>();
        List<Pracownik> pracownicyFromFile = new List<Pracownik>();
        List<Nauczyciel> teachers = new List<Nauczyciel>();
        List<Nauczyciel> teachersFromFile = new List<Nauczyciel>();

        /*
         * klasa z pomocnymi funkcjami w pliku Helpers.cs
         */
        Helpers helper = new Helpers();

        /*
         * Zmienne przechowywujące aktualnie zaznaczone obiekty
         * używana przy dodawaniu zdjęcia, kasowaniu rekordu
         */
        Uczen selectedStudent;
        Pracownik selectedEmployee;
        Nauczyciel selectedTeacher;

        /*
         * Miejsce przchowywania zdjęć
         */
        string workingDirectory = System.IO.Directory.GetCurrentDirectory();

        /*
         * dynamicznie aktualizowany typ danych powiązany z wybraną listą
         * po podłaczeniu do DataGrida wszelkie zmiany ObservableCollection
         * są natychmiastowo odzwierciedlane w widoku DataGrid
         */
        ObservableCollection<Uczen> _students = new ObservableCollection<Uczen>();
        ObservableCollection<Nauczyciel> _nauczyciele = new ObservableCollection<Nauczyciel>();
        ObservableCollection<Pracownik> _pracownicy = new ObservableCollection<Pracownik>();
        
        /*
         *zmienna debug potrzebna przy szukaniu błedów
         */
        string debug;
 
        public MainWindow()
        {
            /*
             * Inicjujemy kilka wspólnych rzeczy 
             */
            InitializeComponent();
            baza.IsReadOnly = true;
            InicjujKomponenty();
            

            debug = String.Format("Aktualny katalog: {0}", workingDirectory);
            Debug.WriteLine(debug);
            this.Title = Configuration.PROGNAME;
        }
        private void InicjujKomponenty()
        {
            DateRule.Visibility = Visibility.Hidden;
            TeacherDateRule.Visibility = Visibility.Hidden;
            EmployeeDatePickerSince.Visibility = Visibility.Hidden;
            TeacherDatePickerSince.Visibility = Visibility.Hidden;
            // Zakładka uczniowie
            searchList.Items.Add("Imię");
            searchList.Items.Add("Drugie Imię");
            searchList.Items.Add("Nazwisko");
            searchList.Items.Add("Panieńskie");
            searchList.Items.Add("Imię Ojca");
            searchList.Items.Add("Imię Matki");
            searchList.Items.Add("PESEL");
            searchList.Items.Add("Płeć");
            searchList.Items.Add("Klasa");
            searchList.Items.Add("Grupy");
            searchList.SelectedIndex = 0;
            // Zakładka Pracownicy
            // kolejność tego ma znaczenie dla funcji FilterPracownik!!
            searchListEmployee.Items.Add("Imię"); // 0
            searchListEmployee.Items.Add("Drugie Imię"); // 1
            searchListEmployee.Items.Add("Nazwisko"); // 2
            searchListEmployee.Items.Add("Panieńskie"); //3
            searchListEmployee.Items.Add("Imię Ojca"); //4
            searchListEmployee.Items.Add("Imię Matki"); //5 
            searchListEmployee.Items.Add("PESEL"); //6
            searchListEmployee.Items.Add("Płeć"); //7
            searchListEmployee.Items.Add("Etat"); //8
            searchListEmployee.Items.Add("Opis"); //8
            searchListEmployee.Items.Add("Data Zatrudnienia"); //9

            //Zakładka Nauczyciele
            searchListTeacher.Items.Add("Imię"); // 0
            searchListTeacher.Items.Add("Drugie Imię"); // 1
            searchListTeacher.Items.Add("Nazwisko"); // 2
            searchListTeacher.Items.Add("Panieńskie"); //3
            searchListTeacher.Items.Add("Imię Ojca"); //4
            searchListTeacher.Items.Add("Imię Matki"); //5 
            searchListTeacher.Items.Add("PESEL"); //6
            searchListTeacher.Items.Add("Płeć"); //7
            //searchListTeacher.Items.Add("Wychowawstwo"); //8
            searchListTeacher.Items.Add("Data Zatrudnienia"); //9

            //searchListEmployee.Items.Add("Wychowawstwo");
            DateRule.Items.Add("Przed:");
            DateRule.Items.Add("Po:");
            TeacherDateRule.Items.Add("Przed:");
            TeacherDateRule.Items.Add("Po:");
            DateRule.SelectedIndex = 0;
            TeacherDateRule.SelectedIndex = 0;
            searchListEmployee.SelectedIndex = 0;
            // Zakładka Nauczyciele

            // ustawiamy dynamiczne nazwy menu
            SetupEditMenu();



        }
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show("Zmiana zakładki");
            if (Uczniowie.IsSelected)
            {
                baza.ItemsSource = _students;
                this.Title = "UCZNIOWIE -\t" + Configuration.PROGNAME;
            }
            if (Nauczyciele.IsSelected)
            {
                //baza.ItemsSource = _pracownicy;
                baza.ItemsSource = _nauczyciele;
                this.Title = "NAUCZYCIELE- \t" + Configuration.PROGNAME;
            }
            if (Pracownicy.IsSelected)
            {
                baza.ItemsSource = _pracownicy;
                this.Title = "PRACOWNICY- \t" + Configuration.PROGNAME;
            }
        }
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Uczniowie.IsSelected) {
                if (baza.SelectedItem != null)
                {
                    // or baza.SelectedItem != MS.Internal.NamedObject
                    try
                    {
                        selectedStudent = (Uczen)baza.SelectedItem;
                        // ładujemy obrazej wybranego ucznia
                        try
                        {
                            string filePath = string.Format("{0}\\{1}.jpg", workingDirectory, selectedStudent.PhotoId);
                            imagePreview.Source = Helpers.BitmapFromUri(new Uri(filePath));

                        }
                        catch
                        {
                            imagePreview.Source = null;
                        }
                    }
                    catch
                    {

                    }
                }
            }
            if (Nauczyciele.IsSelected)
            {
                if (baza.SelectedItem != null)
                {
                    // or baza.SelectedItem != MS.Internal.NamedObject
                    try
                    {
                        selectedTeacher= (Nauczyciel)baza.SelectedItem;
                        // ładujemy obrazej wybranego ucznia
                        try
                        {
                            string filePath = string.Format("{0}\\{1}.jpg", workingDirectory, selectedTeacher.PhotoId);
                            imagePreview.Source = Helpers.BitmapFromUri(new Uri(filePath));

                        }
                        catch
                        {
                            imagePreview.Source = null;
                        }
                    }
                    catch
                    {

                    }
                }
            }
            if (Pracownicy.IsSelected)
            {
                
                if (baza.SelectedItem != null)
                {
                    // or baza.SelectedItem != MS.Internal.NamedObject
                    try
                    {
                        selectedEmployee = (Pracownik)baza.SelectedItem;
                        // ładujemy obrazej wybranego ucznia
                        try
                        {
                            string filePath = string.Format("{0}\\{1}.jpg", workingDirectory, selectedEmployee.PhotoId);
                            imagePreview.Source = Helpers.BitmapFromUri(new Uri(filePath));

                        }
                        catch
                        {
                            imagePreview.Source = null;
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }
        /*
         * obsługa skrótów klawiaturowych
         * Działa to tak, że symulujemy kliknięcia odpowiednich przycisków
         * dzięki temu nie muszę duplikować kodu. Ten sam kod odpowiada i za skróty
         * i za akcje na przyciskach
         */
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Configuration.loadKey)
            {
                ActionLoad();
            }
            // import
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Configuration.exportKey)
            {
                ActionExport();
            }
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Configuration.photoKey)
            {
                if (Uczniowie.IsSelected)
                {
                    // Button_Click()
                    AddPhotoStudent.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
                if (Pracownicy.IsSelected)
                {
                    // Button_Click()
                    AddPhotoTeacher.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
                if (Pracownicy.IsSelected)
                {
                    // Button_Click()
                    AddPhotoEmployee.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
            }
            // Włacz edycje
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Configuration.editKey)
            {
                SwitchDataGridEdit();
            }
            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                if (Uczniowie.IsSelected)
                {
                    // Button_Click()
                    DeleteStudent.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
                if (Pracownicy.IsSelected)
                {
                    // Button_Click()
                    DeleteTeacher.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
                if (Pracownicy.IsSelected)
                {
                    // Button_Click()
                    DeleteEmployee.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
            }
        }

        /*
         * Wczytywanie danych z pliku CSV opisanego w projekcie
         */
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;
            Debug.WriteLine("Otwieram okno wyboru pliku");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\Users\\filip\\source\\";
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                Debug.WriteLine("Obsługuje ShowDialog() == true");
                //Get the path of specified file
                filePath = openFileDialog.FileName;
                try
                {
                    //usersFromFile = helper.GetDataFromFile<Uczen>(filePath);
                    //debug = String.Format("Ilośc wczytanych uczniów {0}", users.Count);
                    //Debug.WriteLine(debug);
                    // zamieniamy _ na "," w liście grup
                    // obsługa wczytywania uczniów
                    if (Uczniowie.IsSelected)
                    {
                        usersFromFile = helper.GetDataFromFile<Uczen>(filePath);
                        debug = String.Format("Ilośc wczytanych uczniów {0}", users.Count);
                        Debug.WriteLine(debug);
                        // usersFromFile = helper.FixUczenGroupFormat(usersFromFile);
                        users = usersFromFile;
                        // observable collection
                        _students.Clear();
                        foreach (Uczen u in users)
                        {
                            _students.Add(u);
                        }
                        int colCount = baza.Columns.Count;
                        baza.Columns[colCount - 1].Visibility = Visibility.Collapsed;
                    }
                    if (Nauczyciele.IsSelected)
                    {
                        teachersFromFile = helper.GetDataFromFile<Nauczyciel>(filePath);
                        debug = String.Format("Ilośc wczytanych uczniów {0}", users.Count);
                        Debug.WriteLine(debug);
                        // usersFromFile = helper.FixUczenGroupFormat(usersFromFile);
                        teachers = teachersFromFile;
                        // observable collection
                        _nauczyciele.Clear();
                        foreach (Nauczyciel n in teachers)
                        {
                            _nauczyciele.Add(n);
                        }
                        int colCount = baza.Columns.Count;
                        baza.Columns[colCount - 1].Visibility = Visibility.Collapsed;
                    }

                    if (Pracownicy.IsSelected)
                    {
                        pracownicyFromFile = helper.GetDataFromFile<Pracownik>(filePath);
                        pracownicy = pracownicyFromFile;
                        int colCount = baza.Columns.Count;
                        baza.Columns[colCount - 1].Visibility = Visibility.Collapsed;
                        _pracownicy.Clear();
                        foreach (Pracownik p in pracownicyFromFile)
                            _pracownicy.Add(p);
                    }
                    //baza.ItemsSource = users;
                    //ukrywamy kolumnę z ID zdjęcia

                } catch (Exception ex)
                { 
                    MessageBox.Show(String.Format("Nieprawidłowa składnia wczytywanego pliku !:\n{0}",ex.ToString()));
                }
            }
            //}
            //}

            //MessageBox.Show(fileContent, "File Content at path: " + filePath, MessageBoxButtons.OK);
        }
        /*
         * zapisywanie danych do pliku
         */

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;
            //using (OpenFileDialog openFileDialog = new OpenFileDialog())
            //{
            //Debug.WriteLine("Otwieram okno wyboru pliku");
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = "c:\\Users\\filip\\source\\";
            saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == true)
            {
                Debug.WriteLine("Obsługuje ShowDialog() == true");
                //Get the path of specified file
                filePath = saveFileDialog.FileName;
                // zamieniamy "," na "_" przed zapisem 
                if (Uczniowie.IsSelected)
                {
                    //var writeList = helper.RestoreUczenGroupFormat(_students.ToList());
                    var writeList = _students.ToList();
                    // zapisujemy
                    //nowy kod z pomysłu Taty z funkcjami generycznymi
                    Helpers.ExportData<List<Uczen>>(ref writeList, filePath);
                }
                if (Nauczyciele.IsSelected)
                {
                    var writeList = _nauczyciele.ToList();
                    Helpers.ExportData<List<Nauczyciel>>(ref writeList, filePath);
                }
                if (Pracownicy.IsSelected)
                {
                    var writeList = _pracownicy.ToList();
                    Helpers.ExportData<List<Pracownik>>(ref writeList, filePath);
                }
            }
        }

        /// Edycja Danych !
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SwitchDataGridEdit();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (Uczniowie.IsSelected) 
            { 
                _students.Remove(selectedStudent);
            }
            if (Nauczyciele.IsSelected)
            {
                _nauczyciele.Remove(selectedTeacher);
            }
            if (Pracownicy.IsSelected)
            {
                _pracownicy.Remove(selectedEmployee);
            }
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((string)searchListEmployee.SelectedItem == "Data Zatrudnienia")
            {
                DateRule.Visibility = Visibility.Visible;
                EmployeeDatePickerSince.Visibility = Visibility.Visible;
            }
            if ((string)searchListTeacher.SelectedItem == "Data Zatrudnienia") {
                TeacherDateRule.Visibility = Visibility.Visible;
                TeacherDatePickerSince.Visibility = Visibility.Visible;
            }
            else
            {
                DateRule.Visibility = Visibility.Hidden;
                EmployeeDatePickerSince.Visibility = Visibility.Hidden;
                TeacherDatePickerSince.Visibility = Visibility.Hidden;
                /*
                 * Jak wychodzimy z wyboiru daty musimy przywrócić wszystkich klientów
                 */
                if (Pracownicy.IsSelected)
                {
                    _pracownicy.Clear();
                    foreach (Pracownik u in pracownicyFromFile)
                    {
                        _pracownicy.Add(u);
                    }
                }
                if (Nauczyciele.IsSelected)
                {
                    _nauczyciele.Clear();
                    foreach (Nauczyciel u in teachersFromFile)
                    {
                        _nauczyciele.Add(u);
                    }
                }

            }
        }
        private void DateRule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EmployeeDatePickerSince.SelectedDate = null;
        }
        private void TeacherDateRule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TeacherDatePickerSince.SelectedDate = null;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Uczniowie.IsSelected) { 
                string searchValue = searchText.Text;
            if (searchValue.Length == 0)
            {
                _students.Clear();
                foreach (Uczen u in usersFromFile)
                {
                    _students.Add(u);
                }
                //baza.ItemsSource = usersFromFile;
                //   users = usersFromFile;
            }
            else
            {
                users = helper.FilterUczen(usersFromFile.ToArray(), searchList.SelectedIndex, searchValue.ToLower());
                _students.Clear();
                foreach (Uczen u in users)
                {
                    _students.Add(u);
                }
                //baza.ItemsSource = users;
            }
        }
            if (Pracownicy.IsSelected)
            {
                string searchValue = searchText.Text;
                if (searchValue.Length == 0)
                {
                    _pracownicy.Clear();
                    foreach (Pracownik u in pracownicyFromFile)
                    {
                        _pracownicy.Add(u);
                    }
                    //baza.ItemsSource = usersFromFile;
                    //   users = usersFromFile;
                }
                else
                {
                    DateSearch ds = DateSearch.NoDateSearch;

                    pracownicy = helper.FilterPracownik(pracownicyFromFile.ToArray(), searchListEmployee.SelectedIndex, searchValue.ToLower(),ds);
                    _pracownicy.Clear();
                    foreach (Pracownik p in pracownicy)
                    {
                        _pracownicy.Add(p);
                    }
                    //baza.ItemsSource = users;
                }
            }
            if (Nauczyciele.IsSelected)
            {
                string searchValue = searchText.Text;
                if (searchValue.Length == 0)
                {
                    _nauczyciele.Clear();
                    foreach (Nauczyciel u in teachersFromFile)
                    {
                        _nauczyciele.Add(u);
                    }
                }
                else
                {
                    DateSearch ds = DateSearch.NoDateSearch;

                    teachers = helper.FilterNauczyciel(teachersFromFile.ToArray(), searchListEmployee.SelectedIndex, searchValue.ToLower(), ds);
                    _nauczyciele.Clear();
                    foreach (Nauczyciel p in teachers)
                    {
                        _nauczyciele.Add(p);
                    }
                    //baza.ItemsSource = users;
                }
            }
            //}
        }

        private void AddPhoto_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Nie wybrano rekordu do którego ma być dodane zdjęcie.");
            if (selectedStudent != null || selectedEmployee != null) {
                var fileContent = string.Empty;
                var filePath = string.Empty;
                //using (OpenFileDialog openFileDialog = new OpenFileDialog())
                //{
                Debug.WriteLine("Otwieram okno wyboru pliku");
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = "c:\\Users\\filip\\source\\";
                openFileDialog.Filter = "Image Files| *.jpg; *.jpeg; ...";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == true)
                {
                    string newPhotoId = helper.GetRandomAlphaNumeric();
                    filePath = openFileDialog.FileName;
                    string fileDest = string.Format("{0}\\{1}.jpg", workingDirectory, newPhotoId);
                    BitmapImage myBitmapImage = new BitmapImage();
                    imagePreview.Source = null;
                    imagePreview.Source = Helpers.BitmapFromUri(new Uri(filePath));
                    // Uczniowie
                    if (Uczniowie.IsSelected) {                       
                        //debug = string.Format("Przydzielam zdjęcie {0} dla ucznia {1}", newPhotoId, selectedStudent.Pesel);
                       // Debug.WriteLine(debug);
                        string currentPhotoId = selectedStudent.PhotoId;                        
                        //try
                        //{
                        // dodawanie zdjęcia
                        File.Copy(filePath, fileDest);
                        // kasowanie starego !!
                        //try
                        //{// odblokowanie pliku do kasowania                       
                        // } catch
                        // {
                        //     string msg = string.Format("Bład kasowania starego zdjęcia. Skasuje je recznie\nZapisz ID zdjęcia: {0}", currentPhotoId);
                        //      MessageBox.Show(msg);
                        //  }
                        
                        selectedStudent.PhotoId = newPhotoId;
                        File.Delete(string.Format("{0}\\{1}.jpg", workingDirectory, currentPhotoId));
                        // } catch
                        //{
                        //    MessageBox.Show("Bład dodawania zdjęcia dla ucznia");
                        // }
                    }
                    // Pracownicy
                    if(Pracownicy.IsSelected)
                    {
                        string currentPhotoId = selectedEmployee.PhotoId;
                        File.Copy(filePath, fileDest);
                        selectedEmployee.PhotoId = newPhotoId;
                        File.Delete(string.Format("{0}\\{1}.jpg", workingDirectory, currentPhotoId));
                    }
                    if (Nauczyciele.IsSelected)
                    {
                        string currentPhotoId = selectedTeacher.PhotoId;
                        File.Copy(filePath, fileDest);
                        selectedTeacher.PhotoId = newPhotoId;
                        File.Delete(string.Format("{0}\\{1}.jpg", workingDirectory, currentPhotoId));
                    }

                }            }
        }
        private void DatePickerSince_DateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeeDatePickerSince.SelectedDate != null)
            {
                DateTime dt = (DateTime)EmployeeDatePickerSince.SelectedDate;
                DateSearch ds = DateSearch.NoDateSearch;
                if (DateRule.SelectedIndex == 0)
                {
                    ds = DateSearch.Before;
                }
                else
                {
                    ds = DateSearch.After;
                }
                debug = String.Format("Wybrano datę: {0}", dt.ToString(Configuration.DATE_FORMAT));
                Debug.WriteLine(debug);
                pracownicy = helper.FilterPracownik(pracownicyFromFile.ToArray(), searchListEmployee.SelectedIndex, dt.ToString("dd/MM/yyyy"), ds);
                _pracownicy.Clear();
                foreach (Pracownik p in pracownicy)
                {
                    _pracownicy.Add(p);
                }
            }
        }
        private void TeacherDatePickerSince_DateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TeacherDatePickerSince.SelectedDate != null)
            {
                DateTime dt = (DateTime)TeacherDatePickerSince.SelectedDate;
                DateSearch ds = DateSearch.NoDateSearch;
                if (TeacherDateRule.SelectedIndex == 0)
                {
                    ds = DateSearch.Before;
                }
                else
                {
                    ds = DateSearch.After;
                }
                debug = String.Format("Wybrano datę: {0}", dt.ToString(Configuration.DATE_FORMAT));
                Debug.WriteLine(debug);
                teachers = helper.FilterNauczyciel(teachersFromFile.ToArray(), searchListEmployee.SelectedIndex, dt.ToString("dd/MM/yyyy"), ds);
                _nauczyciele.Clear();
                foreach (Nauczyciel p in teachers)
                {
                    _nauczyciele.Add(p);
                }
            }
        }
        private void SwitchDataGridEdit()
        {
            
                bool edycja = baza.IsReadOnly;
                baza.IsReadOnly = !edycja;
            
        }

        private void SetupEditMenu()
        {
            // Ustawiamy dynamicznie wartości tekstu menu
            MenuShortcutsLoad.Header = String.Format("_Wczytaj (CTRL + {0})", Configuration.loadKey.ToString());
            MenuShortcutsExport.Header = String.Format("_Export (CTRL + {0})", Configuration.exportKey.ToString());
            MenuShortcutsEdit.Header = String.Format("_Edycja (CTRL + {0})", Configuration.editKey.ToString());
            MenuShortcutsAddPhotoEdit.Header = String.Format("_Dodaj Zdjęcie (CTRL + {0})", Configuration.photoKey.ToString());
        }
        private void menu_Exit(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        /*
         *  Obsługa menu
         */
        private void menu_LoadEdit(object sender, RoutedEventArgs e)
        {
            //"Edycja skrótu Import/Załaduj";
            Window shortcutWin = new ShortCutEdit(Shortcuts.Load);
            shortcutWin.ShowDialog();
            SetupEditMenu();
        }

        private void menu_ExportEdit(object sender, RoutedEventArgs e)
        {
            //"Edycja skrótu Import/Załaduj";
            Window shortcutWin = new ShortCutEdit(Shortcuts.Export);
            shortcutWin.ShowDialog();
            SetupEditMenu();
        }
        private void menu_EditEdit(object sender, RoutedEventArgs e)
        {
            //"Edycja skrótu Import/Załaduj";
            Window shortcutWin = new ShortCutEdit(Shortcuts.Edit);
            shortcutWin.ShowDialog();
            SetupEditMenu();
        }
        private void menu_AddPhotoEdit(object sender, RoutedEventArgs e)
        {
            //"Edycja skrótu Import/Załaduj";
            Window shortcutWin = new ShortCutEdit(Shortcuts.Edit);
            shortcutWin.ShowDialog();
            SetupEditMenu();
        }
        private void mainMenu_Load(object sender, RoutedEventArgs e)
        {
            ActionLoad();
        }
        private void mainMenu_Export(object sender, RoutedEventArgs e)
        {
            ActionExport();
        }
        private void mainMenu_Edit(object sender, RoutedEventArgs e)
        {
            SwitchDataGridEdit();
        }
        /** poprawianie formatu danych wyświetlanych w DataGrid
         */
        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = Configuration.DATE_FORMAT;
        }

        private void ActionLoad()
        {
            if (Uczniowie.IsSelected)
            {
                // Button_Click()
                LoadStudent.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
            if (Pracownicy.IsSelected)
            {
                // Button_Click()
                LoadEmployee.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void ActionExport()
        {
            if (Uczniowie.IsSelected)
            {
                // Button_Click()
                ExportStudent.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
            if (Pracownicy.IsSelected)
            {
                // Button_Click()
                ExportEmployee.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }
    // koniec klasy
    }


    public sealed class CsvPracownikMap : CsvHelper.Configuration.ClassMap<Pracownik>
    {
        public CsvPracownikMap()
        {
            var msMY = CultureInfo.GetCultureInfo("ms-MY");
            Map(m => m.Name);
            Map(m => m.MiddleName);
            Map(m => m.Surname);
            Map(m => m.MothersName);
            Map(m => m.DadName);
            Map(m => m.MomName);
            Map(m => m.DateOfBirth).TypeConverterOption.Format(Configuration.DATE_FORMAT)
              .TypeConverterOption.CultureInfo(msMY);
            Map(m => m.PhotoId); 
            Map(m => m.Pesel);
            Map(m => m.Gender);
            Map(m => m.Etat);
            Map(m => m.Opis);
            Map(m => m.DataZatrudnienia).TypeConverterOption.Format(Configuration.DATE_FORMAT)
              .TypeConverterOption.CultureInfo(msMY);
            Map(m => m.PhotoId);

        }
    }
    public sealed class CsvNauczycielMap : CsvHelper.Configuration.ClassMap<Nauczyciel>
    {
        public CsvNauczycielMap()
        {
            var msMY = CultureInfo.GetCultureInfo("ms-MY");
            Map(m => m.Name);
            Map(m => m.MiddleName);
            Map(m => m.Surname);
            Map(m => m.MothersName);
            Map(m => m.DadName);
            Map(m => m.MomName);
            Map(m => m.DateOfBirth).TypeConverterOption.Format(Configuration.DATE_FORMAT)
              .TypeConverterOption.CultureInfo(msMY);
            Map(m => m.PhotoId);
            Map(m => m.Pesel);
            Map(m => m.Gender);
            Map(m => m.DataZatrudnienia).TypeConverterOption.Format(Configuration.DATE_FORMAT)
              .TypeConverterOption.CultureInfo(msMY);
            Map(m => m.PhotoId);

        }
    }
    //public sealed class CsvNauczycielMap : CsvHelper.Configuration.ClassMap<Nauczyciel>
    //{
    //    public CsvNauczycielMap()
    //    {
    //        var msMY = CultureInfo.GetCultureInfo("ms-MY");
    //        Map(m => m.Name);
    //        Map(m => m.MiddleName);
    //        Map(m => m.Surname);
    //        Map(m => m.MothersName);
    //        Map(m => m.DadName);
    //        Map(m => m.MomName);
    //        Map(m => m.DateOfBirth).TypeConverterOption.Format(Configuration.DATE_FORMAT)
    //          .TypeConverterOption.CultureInfo(msMY);
    //        Map(m => m.PhotoId);
    //        Map(m => m.Pesel);
    //        Map(m => m.Gender);
    //        //Map(m => m.Etat);
    //        //Map(m => m.Opis);
    //        //Map(m => m.Klasy).TypeConverterOption.Format(Configuration.DATE_FORMAT)
    //        Map(m => m.Klasy).Convert(row  => JsonConvert.DeserializeObject<UczoneKlasy>(row.Row));
    //        Map(m => m.PhotoId);

    //    }
    //}

    public class Person
    {
        public string Name { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string MothersName { get; set; }
        public string DadName { get; set; }
        public string MomName { get; set; }
        public string DateOfBirth { get; set; }
        public string Pesel { get; set; }
    }
    public class Uczen
    {
        //[CsvHelper.Configuration.Attributes.Name("Imię")]
        public string Name { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string MothersName { get; set; }
        public string DadName { get; set; }
        public string MomName { get; set; }
        public string DateOfBirth { get; set; }
        public string Pesel { get; set; }
        public string Gender { get; set; }
        public string Klasa { get; set; }
        public string Grupy { get; set; }
        public string PhotoId { get; set; }
    }

    public class Pracownik
    {
        public string Name { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string MothersName { get; set; }
        public string DadName { get; set; }
        public string MomName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Pesel { get; set; }
        public string Gender { get; set; }
        public string Etat { get; set; }
        public string Opis { get; set; }
        public DateTime DataZatrudnienia { get; set; }
        public string PhotoId { get; set; }
    }
    public class Nauczyciel
    {
        public string Name { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string MothersName { get; set; }
        public string DadName { get; set; }
        public string MomName { get; set; }
        public string DateOfBirth { get; set; }
        public string Pesel { get; set; }
        public string Gender { get; set; }
        public bool Supervising { get; set; }
        //public string TeachingSub { get; set; }
        //klasy z godzinami
        public DateTime DataZatrudnienia { get; set; }
 //       public List<UczoneKlasy> Klasy { get; set; }
        public string PhotoId { get; set; }
    }
    public class UczoneKlasy
    {
        public string Klasa { get; set; }
        public int IloscGodzin { get; set; }
    }
}