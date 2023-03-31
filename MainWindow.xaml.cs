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
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace WhisperWpfWindows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string selLang = "English";
        private static string selWhisperModel = "tiny";

        private static long irCmdFileCounter = 0;
        public enum enLanguages
        {
            Afrikaans, Albanian, Amharic, Arabic, Armenian, Assamese, Azerbaijani, Bashkir, Basque, Belarusian, Bengali, Bosnian, Breton,
            Bulgarian, Burmese, Castilian, Catalan, Chinese, Croatian, Czech, Danish, Dutch, English, Estonian, Faroese, Finnish, Flemish,
            French, Galician, Georgian, German, Greek, Gujarati, Haitian, HaitianCreole, Hausa, Hawaiian, Hebrew, Hindi, Hungarian, Icelandic, 
            Indonesian, Italian, Japanese, Javanese, Kannada, Kazakh, Khmer, Korean, Lao, Latin, Latvian, Letzeburgesch, Lingala, Lithuanian,
            Luxembourgish, Macedonian, Malagasy, Malay, Malayalam, Maltese, Maori, Marathi, Moldavian, Moldovan, Mongolian, Myanmar, Nepali, 
            Norwegian, Nynorsk, Occitan, Panjabi, Pashto, Persian, Polish, Portuguese, Punjabi, Pushto, Romanian, Russian, Sanskrit, Serbian, 
            Shona, Sindhi, Sinhala, Sinhalese, Slovak, Slovenian, Somali, Spanish, Sundanese, Swahili, Swedish, Tagalog, Tajik, Tamil, Tatar,
            Telugu, Thai, Tibetan, Turkish, Turkmen, Ukrainian, Urdu, Uzbek, Valencian, Vietnamese, Welsh, Yiddish, Yoruba

        }

        public enum whisperModels
        {
            tiny,
            _base,
            small,
            medium,
            large
        }
        public MainWindow()
        {
            InitializeComponent();
            foreach (var enLang in Enum.GetValues(typeof(enLanguages)).Cast<enLanguages>())
            {
                cmdBoxLang.Items.Add(enLang);
            }
            cmdBoxLang.SelectedItem = enLanguages.English;

            foreach (var wModel in Enum.GetValues(typeof(whisperModels)).Cast<whisperModels>())
            {
                cmdBoxWhisperModel.Items.Add(wModel.extentionWhisperModelEnums());
            }
            cmdBoxWhisperModel.SelectedItem = whisperModels.tiny.ToString();

        }

        private void btnStartProcess_Click(object sender, RoutedEventArgs e)
        {
            selLang = cmdBoxLang.SelectedItem.ToString();
            selWhisperModel = cmdBoxWhisperModel.SelectedItem.ToString();

            Directory.CreateDirectory("testCmds");
            string selFilePath = txtFileName.Text.ToString();

            Task.Factory.StartNew(() => { startProcesing(selFilePath); });
        }

        private void startProcesing(string selFilePath)
        {
            string filesPath = null;
            bool isDirPath = false;

            

            if (Directory.Exists(selFilePath))
            {
                isDirPath = true;
            }

            if (isDirPath == false)
            {
                if (File.Exists(selFilePath))
                {
                    filesPath = selFilePath;
                }
            }

            if (isDirPath)
            {
                var dirFiles = Directory.GetFiles(selFilePath);

                ParallelOptions parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 2 };

                Parallel.ForEach(dirFiles, parallelOptions, filePath =>
                {
                    startCmd(filePath);
                });
            }
            else
            {
                if (string.IsNullOrEmpty(filesPath) == false)
                {
                    startCmd(filesPath);
                }
            }
        }

        private void startCmd(string filePath)
        {

            string strCmd = @$"cmd /K whisper ""{filePath}"" --language {selLang} --model {selWhisperModel}  --task translate";
            string strCmdName = $"testCmds/cmd_{Interlocked.Read(ref irCmdFileCounter)}.cmd";
            Interlocked.Add(ref irCmdFileCounter, 1);
            File.WriteAllText(strCmdName, strCmd);

            ProcessStartInfo processStartInfo = new ProcessStartInfo(strCmdName);
            processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
            var procesStart = Process.Start(processStartInfo);
            procesStart.WaitForExit();
        }

        private void btnSelectProcessingFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (folderDialog.ShowDialog().ToString() == "OK")
            {
                txtFileName.Text = folderDialog.SelectedPath;
            }
        }

        private void btnSelectProcessingFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog().ToString() == "OK")
            {
                txtFileName.Text = fileDialog.FileName;
            }
        }
    }
}
