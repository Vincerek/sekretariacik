using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sekretariacik
{
    public class Helpers
    {
        public static ImageSource BitmapFromUri(Uri source)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = source;
            bitmap.DecodePixelWidth = 200;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }
        public string GetRandomAlphaNumeric()
        {
            return System.IO.Path.GetRandomFileName().Replace(".", "").Substring(0, 8);
        }
        public List<Sekretariacik.Uczen> FixUczenGroupFormat(List<Sekretariacik.Uczen> data)
        {
            List<Sekretariacik.Uczen> tmpUsers = new List<Uczen>();
            foreach (Uczen u in data)
            {
                Debug.WriteLine(u);
                u.Grupy = u.Grupy.Replace("_", ",");
                tmpUsers.Add(u);
            }
            //Debug.WriteLine(tmpUsers.Count);
            return tmpUsers;
        }
        //public List<Sekretariacik.Uczen> RestoreUczenGroupFormat(List<Sekretariacik.Uczen> data)
        //{
        //    List<Sekretariacik.Uczen> tmpUsers = new List<Uczen>();
        //    foreach (Uczen u in data)
        //    {
        //        Debug.WriteLine(u);
        //        u.Grupy = u.Grupy.Replace(",", "_");
        //        tmpUsers.Add(u);
        //    }
        //    //Debug.WriteLine(tmpUsers.Count);
        //    return tmpUsers;
        //}
        //public List<Uczen> GetUczenFromFile(string filepath)
        //{
        //    List<Uczen> users;
        //    //= new List<Sekretariacik.Uczen>();\
        //    var csvConfig = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture);
        //    csvConfig.Delimiter = ";";

        //    using (var reader = new StreamReader(filepath))
        //    using (var csv = new CsvReader(reader, csvConfig))
        //    {
        //        users = csv.GetRecords<Uczen>().ToList();
        //    }

        //    return users;
        //}
        public List<Pracownik> GetPracownikFromFile(string filepath)
        {
            List<Pracownik> pracownicy;
            var csvConfig = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture);
            csvConfig.Delimiter = ";";

            using (var reader = new StreamReader(filepath))
            using (var csv = new CsvReader(reader, csvConfig))
            {
                pracownicy = csv.GetRecords<Pracownik>().ToList();
            }

            return pracownicy;
        }
        // funcje generyczne - Tato wytłumaczył co to jest :)
        // zamiast 3 funkcji dla Ucznia, Nauczyciela, Pracownika do pobierania danych można uzyć jednej
        // a zwracany typ jest częscią wywołania
        // funkcja generyczna to taka, gdzie nie znamy typy argumentów, albo zwracanego typu przed wywowołaniem
        // te dane podanemy w chwili wywołania funcji
        public List<T> GetDataFromFile<T>(string filepath)
        {
            List<T> data;
            var csvConfig = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture);
            csvConfig.Delimiter = ";";

            using (var reader = new StreamReader(filepath))
            using (var csv = new CsvReader(reader, csvConfig))
            {
                if (typeof(T) == typeof(Pracownik))
                {
                    csv.Context.RegisterClassMap<CsvPracownikMap>();
                }
                if (typeof(T) == typeof(Nauczyciel))
                {
                    csv.Context.RegisterClassMap<CsvNauczycielMap>();
                }
                data = csv.GetRecords<T>().ToList();
            }

            return data;
        }
        public static void ExportData<T>(ref T writeData, string filePath) where T : System.Collections.IEnumerable
        {
            var csvConfig = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture);
            csvConfig.Delimiter = ";";
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, csvConfig))
                csv.WriteRecords(writeData);
        }
        public List<Sekretariacik.Uczen> FilterUczen(Uczen[] input, int prop, string value)
        {
            switch (prop)
            {
                // Imię
                case 0:
                    return input.Where(c => c.Name.ToLower().Contains(value)).ToList();
                // Drugie Imię
                case 1:
                    return input.Where(c => c.MiddleName.ToLower().Contains(value)).ToList();
                // Nazwisko
                case 2:
                    return input.Where(c => c.Surname.ToLower().Contains(value)).ToList();
                // Panieńskie
                case 3:
                    return input.Where(c => c.MothersName.ToLower().Contains(value)).ToList();
                // Imię Ojca
                case 4:
                    return input.Where(c => c.DadName.ToLower().Contains(value)).ToList();
                // Imię Matki
                case 5:
                    return input.Where(c => c.MomName.ToLower().Contains(value)).ToList();
                // Pesel
                case 6:
                    return input.Where(c => c.Pesel.ToLower().Contains(value)).ToList();
                // Płeć
                case 7:
                    return input.Where(c => c.Gender.ToLower().Contains(value)).ToList();
                // Klasa
                case 8:
                    return input.Where(c => c.Klasa.ToLower().Contains(value)).ToList();
                // Grupy
                case 9:
                    return input.Where(c => c.Grupy.ToLower().Contains(value)).ToList();
                default:
                    return input.ToList();
            }
        }
        public List<Pracownik> FilterPracownik(Pracownik[] input, int prop, string value, DateSearch ds)
        {
            switch (prop)
            {
                // Imię
                case 0:
                    return input.Where(c => c.Name.ToLower().Contains(value)).ToList();
                // Drugie Imię
                case 1:
                    return input.Where(c => c.MiddleName.ToLower().Contains(value)).ToList();
                // Nazwisko
                case 2:
                    return input.Where(c => c.Surname.ToLower().Contains(value)).ToList();
                // Panieńskie
                case 3:
                    return input.Where(c => c.MothersName.ToLower().Contains(value)).ToList();
                // Imię Ojca
                case 4:
                    return input.Where(c => c.DadName.ToLower().Contains(value)).ToList();
                // Imię Matki
                case 5:
                    return input.Where(c => c.MomName.ToLower().Contains(value)).ToList();
                // Pesel
                case 6:
                    return input.Where(c => c.Pesel.ToLower().Contains(value)).ToList();
                // Płeć
                case 7:
                    return input.Where(c => c.Gender.ToLower().Contains(value)).ToList();
                // Etat
                case 8:
                    return input.Where(c => c.Etat.ToLower().Contains(value)).ToList();
                // Opis
                case 9:
                    return input.Where(c => c.Opis.ToLower().Contains(value)).ToList();
                //DataZatrudnienia
                case 10:

                    if (ds == DateSearch.Before)
                    {
                        string debug = String.Format("Filtruję przed datą: {0}", value);
                        Debug.WriteLine(debug);
                        return input.Where(c => DateTime.ParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture) > c.DataZatrudnienia).ToList();
                    }
                    else
                    {
                        string debug = String.Format("Filtruję po dacie: {0}", value);
                        Debug.WriteLine(debug);
                        return input.Where(c => DateTime.ParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture) < c.DataZatrudnienia).ToList();
                    }
                // operacje na dacie
                default:
                    return input.ToList();
            }
            //return input.ToList();
            //return input.Where(c => c.Name.Contains(value)).ToList();
        }
        public List<Nauczyciel> FilterNauczyciel(Nauczyciel[] input, int prop, string value, DateSearch ds)
        {
            switch (prop)
            {
                // Imię
                case 0:
                    return input.Where(c => c.Name.ToLower().Contains(value)).ToList();
                // Drugie Imię
                case 1:
                    return input.Where(c => c.MiddleName.ToLower().Contains(value)).ToList();
                // Nazwisko
                case 2:
                    return input.Where(c => c.Surname.ToLower().Contains(value)).ToList();
                // Panieńskie
                case 3:
                    return input.Where(c => c.MothersName.ToLower().Contains(value)).ToList();
                // Imię Ojca
                case 4:
                    return input.Where(c => c.DadName.ToLower().Contains(value)).ToList();
                // Imię Matki
                case 5:
                    return input.Where(c => c.MomName.ToLower().Contains(value)).ToList();
                // Pesel
                case 6:
                    return input.Where(c => c.Pesel.ToLower().Contains(value)).ToList();
                // Płeć
                case 7:
                    return input.Where(c => c.Gender.ToLower().Contains(value)).ToList();
                // Etat
                //case 8:
                //    return input.Where(c => c.Supervising.ToLower().Contains(value).ToList();
                //DataZatrudnienia
                case 8:

                    if (ds == DateSearch.Before)
                    {
                        string debug = String.Format("Filtruję przed datą: {0}", value);
                        Debug.WriteLine(debug);
                        return input.Where(c => DateTime.ParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture) > c.DataZatrudnienia).ToList();
                    }
                    else
                    {
                        string debug = String.Format("Filtruję po dacie: {0}", value);
                        Debug.WriteLine(debug);
                        return input.Where(c => DateTime.ParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture) < c.DataZatrudnienia).ToList();
                    }
                // operacje na dacie
                default:
                    return input.ToList();
            }
            //return input.ToList();
            //return input.Where(c => c.Name.Contains(value)).ToList();
        }
    }
}