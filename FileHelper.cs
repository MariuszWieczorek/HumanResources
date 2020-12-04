using HumanResources.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Runtime.Serialization.Json;


// umieszczamy w tej klasie uniwersalne metody do serializacji i deserializacji aby nie powielać ich kodu
// przerabiamy tę klasę na generyczną aby nadawała się nie tylko do obiektów klasy List<Student>
// w tym celi do nazwy klasy dodajemy <T>
// i każde wystąpienie List<Student> zastępujemy T
// aby móc zapisaćlinię kodu: return new T();
// musimy zawęzić tryp generyczny do klas, które mogą miećpusty konstruktor
// FileHelper<T> where T : new()

namespace HumanResources
{
    public class FileHelper<T> where T : new()
    {
        private string _filePath;

        public FileHelper(string filePath)
        {
            _filePath = filePath;
        }

        public void SerializeToFile(T students)
        {
            // Zapisujemy Listę studentów do pliku wersja z użyciem USING
            // przekazujemy listę obiektów typu Student
            // typeof() - zwróci typ podczas kompilacji
            // The typeof is an operator keyword which is used to get a type at the compile-time.

            var serializer = new XmlSerializer(typeof(T));
            StreamWriter streamWriter = null;

            // jeżeli w using jest deklaracja jakiegoś obiektu
            // to zawsze na tym obiekcie zostanie automatycznie wywołana metoda Dispose
            using (streamWriter = new StreamWriter(_filePath))
            {
                // stream jest to klasa, która zapewnia nam transfer bajtów
                serializer.Serialize(streamWriter, students);
                streamWriter.Close();
            }
        }

        public void SerializeToFileJson(T list)
        {
            // Zapisujemy Listę studentów do pliku wersja z użyciem USING
            // przekazujemy listę obiektów typu Student
            // typeof() - zwróci typ podczas kompilacji
            // The typeof is an operator keyword which is used to get a type at the compile-time.

            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            StreamWriter streamWriter = null;

            using (streamWriter = new StreamWriter(_filePath+"x"))
            using (JsonWriter writer = new JsonTextWriter(streamWriter))
            {
                serializer.Serialize(writer, list);
            }

        }

        /// <summary>
        /// Odczytuje Listę obiektów z pliku w tym przypadku z XML'a 
        /// </summary>
        /// <returns></returns>
        public T DeserializeFromFileJson()
        {
            if (!File.Exists(_filePath+'x'))
            {
                return new T();
            }

            //JsonSerializer serializer = new JsonSerializer();
            //serializer.Converters.Add(new JavaScriptDateTimeConverter());
            // serializer.NullValueHandling = NullValueHandling.Ignore;

            var list = (T)JsonConvert.DeserializeObject<T>(_filePath);
            return list;

            /*
            using (var streamReader = new StreamReader(_filePath))
            {
                // stream jest to klasa, która zapewnia nam transfer bajtów
                // Deserializer zwraca typ obiekt, musimy go rzutować na listę studentów
                //var listx = (T)serializer.Deserialize(streamReader);
                
                streamReader.Close();
                return list;
            }
            */

        }
  

        /// <summary>
        /// Odczytuje Listę obiektów z pliku w tym przypadku z XML'a 
        /// </summary>
        /// <returns></returns>
        public T DeserializeFromFile()
        {
            if (!File.Exists(_filePath))
            {
                return new T();
            }

            var serializer = new XmlSerializer(typeof(T));

            using (var streamReader = new StreamReader(_filePath))
            {
                // stream jest to klasa, która zapewnia nam transfer bajtów
                // Deserializer zwraca typ obiekt, musimy go rzutować na listę studentów
                var students = (T)serializer.Deserialize(streamReader);
                streamReader.Close();
                return students;
            }
        }

        // ----------------------------------------------------------------------------------------------------------
        // - stare wersje metod, w celach archiwizacji 


        public void SerializeToFile1(T students)
        {
            // Zapisujemy Listę studentów do pliku wersja z użyciem TRY ... CATCH    
            // przekazujemy listę obiektów typu Student
            // typeof - zwróci typ podczas kompilacji
            // The typeof is an operator keyword which is used to get a type at the compile-time.

            var serializer = new XmlSerializer(typeof(T));
            StreamWriter streamWriter = null;

            try
            {
                streamWriter = new StreamWriter(_filePath);

                // stream jest to klasa, która zapewnia nam transfer bajtów
                serializer.Serialize(streamWriter, students);
                streamWriter.Close();
            }
            finally
            {

                // Obiekty typu stream trzeba ręcznie usunąć z pamięci
                streamWriter.Dispose();
            }
        }


        // Różne metody zapisujące i odczytujące z pliku
        public void ReadFromFile()
        {
            string filePath = $@"{Path.GetDirectoryName(Application.ExecutablePath)}\..\..\NowyPlik2.txt";
            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }
            // File.Delete(filePath);
            // Obydwie poniższe metody tworzą plik jeżeli gop jeszcze nie ma
            // nie trzebw więc używać Create
            File.WriteAllText(filePath, "Zostań programistą .Net");
            File.AppendAllText(filePath, "\nZostań programistą .Net");
            var text = File.ReadAllText(filePath);
            MessageBox.Show(text);
        }



    }
}
