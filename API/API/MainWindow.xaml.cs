using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using Newtonsoft.Json;
using Word = Microsoft.Office.Interop.Word;

namespace API
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private string FullName { get; set; }
        private readonly char[] _forbiddenChars = { '%', '&' };

        private bool ContainsForbiddenChars(string input)
        {
            return input.Any(c => _forbiddenChars.Contains(c));
        }

        private void GetFullName(object sender, RoutedEventArgs e)
        {
            string URL = "http://localhost:4444/TransferSimulator/fullName";
            var request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "GET";

            request.Proxy.Credentials = new NetworkCredential("student", "student");

            using (var response = (HttpWebResponse)request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string text = reader.ReadToEnd();
                var JsonText = JsonConvert.DeserializeObject<FullNameSerializator>(text);

                FullName = JsonText.value;
                TextBoxFullName.Text = FullName;
            }
        }

        public void AddToWordTable(string[] rowData)
        {
            string filePath = @"C:\Users\ROMA_W\Desktop\Прил_4_ОЗ_КОД 09.02.07-5-2026-М6\ТестКейс.docx";

            Word.Application wordApp = null;
            Word.Document doc = null;

            try
            {
                wordApp = new Word.Application();

                doc = wordApp.Documents.Open(filePath);
                wordApp.Visible = false;

                Word.Table table = doc.Tables[1];
                Word.Row row = table.Rows.Add();

                for (int i = 0; i < rowData.Length; i++)
                {
                    row.Cells[i + 1].Range.Text = rowData[i];
                }

                doc.Save();
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close(Word.WdSaveOptions.wdSaveChanges);
                }
                if (wordApp != null)
                {
                    wordApp.Quit(Word.WdSaveOptions.wdSaveChanges);
                }
            }
        }

        private void SendTestResult(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FullName))
            {
                MessageBox.Show("Данные не были получены.");
                return;
            }

            bool isForbidden = ContainsForbiddenChars(FullName);

            string validationResult;
            if (isForbidden)
            {
                validationResult = "ФИО содержит запрещенные символы";
                tb_result.Text = validationResult;
            }
            else
            {
                validationResult = "ФИО валидно";
                tb_result.Text = validationResult;
            }

            string[] rowData = { "Столбец действие", FullName, !isForbidden ? "Валидно" : "ФИО содержит запрещенные символы" };

            AddToWordTable(rowData);

            MessageBox.Show("Информация была добавлена в файл!");
        }
    }

    public class FullNameSerializator
    {
        public string value { get; set; }
    }
}