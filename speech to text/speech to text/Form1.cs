using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition;

namespace speech_to_text
{
    public partial class Form1 : Form
    {
        SpeechRecognitionEngine recEngine = new SpeechRecognitionEngine();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string info = "Yüklü Olan Konuşma Tanıyıcılar:\n\n";
            foreach (var recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                info += "Dil: " + recognizer.Culture.ToString() + "\n";
                info += "Adı: " + recognizer.Name + "\n\n";
            }
            MessageBox.Show(info);
            try
            {
               
                recEngine = new SpeechRecognitionEngine(); 

               
                recEngine.SetInputToDefaultAudioDevice();

               
                recEngine.LoadGrammar(new DictationGrammar());

                
                recEngine.SpeechRecognized += RecEngine_SpeechRecognized;

            
                recEngine.SpeechRecognitionRejected += RecEngine_SpeechRecognitionRejected;

                lblDurum.Text = "Hazır. Başlat butonuna basın.";
                btnDurdur.Enabled = false; 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Konuşma motoru yüklenemedi. Windows konuşma paketlerini kontrol edin.\nHata: " + ex.Message);
                btnBaslat.Enabled = false;
                btnDurdur.Enabled = false;
            }
        }
        private void RecEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            
            txtSonuc.AppendText(e.Result.Text + Environment.NewLine);
            lblDurum.Text = "Konuşma algılandı...";
        }

       
        private void RecEngine_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            lblDurum.Text = "Konuşma anlaşılamadı.";
        }

        private void btnBaslat_Click(object sender, EventArgs e)
        {
            try
            {
                
                recEngine.RecognizeAsync(RecognizeMode.Multiple);

                btnBaslat.Enabled = false;
                btnDurdur.Enabled = true;
                lblDurum.Text = "Dinleniyor... Lütfen konuşun.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dinleme başlatılamadı: " + ex.Message);
            }
        }

        private void btnDurdur_Click(object sender, EventArgs e)
        {
            recEngine.RecognizeAsyncStop();

            btnBaslat.Enabled = true;
            btnDurdur.Enabled = false;
            lblDurum.Text = "Durduruldu. Tekrar başlatabilirsiniz.";
        }
    }
}

