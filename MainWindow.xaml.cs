using System;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;

namespace DovizKuruWpf
{
    
    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        
        /// Kurları güncelle butonuna tıklandığında TCMB API'sinden döviz kurlarını çeker
        
        private async void BtnGuncelle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Butonu devre dışı bırak ve yükleniyor durumuna geçir
                btnText.Text = "Yükleniyor... 🔄";
                btnGuncelle.Background = new SolidColorBrush(Colors.Gray);
                btnGuncelle.IsEnabled = false;
                lblUsd.Content = "USD: ... TL";
                lblTry.Content = "TRY: ... USD";

                // TCMB API'sinden XML verisini çek
                using (HttpClient client = new HttpClient())
                {
                    string xmlData = await client.GetStringAsync("http://www.tcmb.gov.tr/kurlar/today.xml");
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlData);

                    // USD'nin TL karşılığını al
                    string usdTl = xmlDoc.SelectSingleNode("//Currency[@Kod='USD']/ForexSelling").InnerText;
                    decimal usdToTl = Convert.ToDecimal(usdTl.Replace(".", ","));

                    // 1 TL'nin USD karşılığını hesapla
                    decimal tryToUsd = 1 / usdToTl;

                    // Label'ları güncelle
                    lblUsd.Content = $"USD: {usdToTl:F2} TL";
                    lblTry.Content = $"TRY: {tryToUsd:F4} USD";
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda kullanıcıyı bilgilendir
                MessageBox.Show($"Hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Butonu tekrar etkinleştir ve orijinal haline getir
                btnGuncelle.IsEnabled = true;
                btnGuncelle.Background = new SolidColorBrush(Color.FromRgb(0x34, 0x98, 0xDB));
                btnText.Text = "Kurları Güncelle 🔄";
            }
        }

        
        /// Pencereyi kapatır
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        
        /// Başlık çubuğuna tıklayıp sürükleyerek pencereyi taşır
        
        private void titleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        
        /// Pencereyi görev çubuğuna küçültür
        
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}