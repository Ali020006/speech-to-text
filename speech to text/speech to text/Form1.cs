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
                // Türkçe tanıma için kültür ayarı
                // Eğer Windows'ta Türkçe konuşma paketi yüklü değilse hata verebilir.
                // Yüklü değilse "en-US" (İngilizce) deneyebilirsin.
                recEngine = new SpeechRecognitionEngine(); // Varsayılan dili (İngilizce'yi) kullanır

                // Giriş olarak varsayılan ses cihazını (mikrofonu) ayarla
                recEngine.SetInputToDefaultAudioDevice();

                // Tanınacak kelimeler için bir dil bilgisi (grammar) yükle
                // DictationGrammar, serbest konuşmayı tanımak için kullanılır
                recEngine.LoadGrammar(new DictationGrammar());

                // Konuşma tanındığında hangi metodun çalışacağını belirle
                recEngine.SpeechRecognized += RecEngine_SpeechRecognized;

                // (İsteğe bağlı) Konuşma algılandı ama anlaşılamadıysa
                recEngine.SpeechRecognitionRejected += RecEngine_SpeechRecognitionRejected;

                lblDurum.Text = "Hazır. Başlat butonuna basın.";
                btnDurdur.Enabled = false; // Başlangıçta durdur butonu pasif olsun
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
            // Tanınan metni al ve TextBox'a ekle
            txtSonuc.AppendText(e.Result.Text + Environment.NewLine);
            lblDurum.Text = "Konuşma algılandı...";
        }

        // Konuşma algılandı ama ANLAŞILAMADIYSA bu metot çalışır
        private void RecEngine_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            lblDurum.Text = "Konuşma anlaşılamadı.";
        }

        private void btnBaslat_Click(object sender, EventArgs e)
        {
            try
            {
                // Asenkron olarak dinlemeye başla (birden fazla kelime için)
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
