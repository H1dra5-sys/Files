using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Standart {

  [Serializable]
  public class NewOperation {
    public string Name, Surname, MiddleName;
    
    public NewOperation() {
    }

    public NewOperation(string name, string surname, string middle) {
      Name = name;
      Surname = surname;
      MiddleName = middle;
    }

    public void Serialize(FileStream binaryCode) {
      BinaryFormatter translater = new BinaryFormatter();
      translater.Serialize(binaryCode, this);
      binaryCode.Flush();
      binaryCode.Close();
    }

    public void Deserialize(FileStream binaryCode) {
      BinaryFormatter binaryFormatter = new BinaryFormatter();
      NewOperation deserialized = (NewOperation)binaryFormatter.Deserialize(binaryCode);

      Name = deserialized.Name;
      Surname = deserialized.Surname;
      MiddleName = deserialized.MiddleName;
      binaryCode.Close();
    }

    public void Print() {
      Console.WriteLine($"Name = {Name}, Surname = {Surname}, Middlename = {MiddleName}");
    }
  }

  //Поиск по содержимому файла
  public class Find {

    public void FindInFiles(string folder, string word) {
      try {
        string[] files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);

        foreach (string file in files) {
          try {
            string text = File.ReadAllText(file);
            if (text.Contains(word)) {
              Console.WriteLine(file);
            }
          } catch { } // Пропускаем файлы, которые не можем прочитать
        }
      } catch (UnauthorizedAccessException) {
        Console.WriteLine($"Not Available: {folder}");
      }
    }
  }

  public class ConsoleEditor {
    int choose;
    string fileName, fileContents, newFileName, oldFileName, folder, keyWord, content;

    //Поиск файлов в папке по ключевому слову
    public void Search() {
      string[] files = Directory.GetFiles(folder, "*.*");

      for (int аrrayIndex = 0; аrrayIndex < files.Length; ++аrrayIndex) {
        content = File.ReadAllText(files[аrrayIndex]);

        if (Regex.IsMatch(content, keyWord)) {
          Console.WriteLine($"A match was found in the {files[аrrayIndex]}");
        }
      }
    }

    public void Menu() {
      Console.WriteLine("\n1. Create a file without touching the contents\n2. Create a file and change the contents \n3. Change the file name\n4. Find all files in a folder by keyword");

      choose = int.Parse(Console.ReadLine());
      switch (choose) {
        case 1:
          Console.WriteLine("\nEnter the name of the file you want to create");

          fileName = Console.ReadLine();
          CreateFile(fileName, fileContents);

          break;

        case 2:
          Console.Write("\nEnter the file name: ");
          fileName = Console.ReadLine();

          Console.Write("\nEnter the contents of the file: ");
          fileContents = Console.ReadLine();
          CreateFile(fileName, fileContents);

          break;

        case 3:
          Console.Write("\nEnter the name of the file you want to rename: \n");
          oldFileName = Console.ReadLine();

          Console.Write("\nEnter a new file name\r\n: \n");
          newFileName = Console.ReadLine();

          File.Move(oldFileName, newFileName);
          
          break;

        case 4:
          Console.Write("Enter the name of the folder where you want to search: ");
          folder = Console.ReadLine();

          Console.Write("Enter the keyword you want to use to search for files: \n");
          keyWord = Console.ReadLine();

          Search();
          
          break;

        default:
          Console.WriteLine("\nERROR");

          break;
      }
    }

     public void CreateFile(string fileName, string fileContents) {
      File.WriteAllText(fileName, fileContents);
    }
  }

  class MainProgramm {

    static void Main() {
      NewOperation fnc = new NewOperation("Mixail", "Dundukov", "Mixailovich");
      Find search = new Find();
      fnc.Print();


      //Бинарная Сериализация
      Console.WriteLine("\nBinary serialization");
      FileStream binaryFile = new FileStream("Fail.bin", FileMode.Create);
      fnc.Serialize(binaryFile);
      binaryFile.Close();
      Console.WriteLine("The Binary file has been created");

      //XML Сериализация
      Console.WriteLine("\nXML serialization");
      XmlSerializer xmlSerialized = new XmlSerializer(typeof(NewOperation));
      using (FileStream xmlFile = new FileStream("Fail.xml", FileMode.Create)) {
        xmlSerialized.Serialize(xmlFile, fnc);
      }
      Console.WriteLine("The XML file has been created\n");

      Console.Write("After serialization, we set a new person to be checked: ");
      fnc = new NewOperation("Alexey", "Kozlovskiy", "Alexeyevich");
      fnc.Print();

      //Бинарная десиреализация
      Console.WriteLine("\nBinary deserialization");
      binaryFile = new FileStream("Fail.bin", FileMode.Open);
      fnc.Deserialize(binaryFile);
      binaryFile.Close();
      fnc.Print();

      //XML Дeсериализация (в новый объект)
      Console.WriteLine("\nXML deserialization");
      using (FileStream xmlFile = new FileStream("Fail.xml", FileMode.Open)) {
        NewOperation fromXml = (NewOperation)xmlSerialized.Deserialize(xmlFile);
        Console.Write("Из XML: ");
        fromXml.Print();
      }

      //Показываем содержимое файлов
      Console.WriteLine("\nContent Fail.xml: ");
      Console.WriteLine(File.ReadAllText("Fail.xml"));

      Console.WriteLine("\nContent Fail.bin: ");
      byte[] binBytes = File.ReadAllBytes("Fail.bin");
      Console.WriteLine(BitConverter.ToString(binBytes));

      //Консольный редактор файлов и поиск файлов по ключевым словам
      ConsoleEditor name = new ConsoleEditor();
      name.Menu();
    }
  }
}

