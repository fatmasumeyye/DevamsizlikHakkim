using DevamsizlikHakkim.Models;

namespace DevamsizlikHakkim
{
    public partial class YeniDersPage : ContentPage
    {
        public static event Action<Ders>? DersEklendi;

        private int _haftalikSaat = 3;

        public YeniDersPage()
        {
            InitializeComponent();
            HaftalikSaatLabel.Text = _haftalikSaat.ToString();
            DevamsizlikSiniriSlider.Value = 20;
            DevamsizlikSiniriLabel.Text = "%20";
        }

        private void OnArtiClicked(object? sender, EventArgs e)
        {
            if (_haftalikSaat < 24)
            {
                _haftalikSaat++;
                HaftalikSaatLabel.Text = _haftalikSaat.ToString();
            }
        }

        private void OnEksiClicked(object? sender, EventArgs e)
        {
            if (_haftalikSaat > 1)
            {
                _haftalikSaat--;
                HaftalikSaatLabel.Text = _haftalikSaat.ToString();
            }
        }

        private void OnSliderValueChanged(object? sender, ValueChangedEventArgs e)
        {
            // 5'er puanlık adımlarla yuvarla
            double stepValue = Math.Round(e.NewValue / 5.0) * 5.0;

            if (Math.Abs(DevamsizlikSiniriSlider.Value - stepValue) > 0.01)
            {
                DevamsizlikSiniriSlider.Value = stepValue;
                return;
            }

            DevamsizlikSiniriLabel.Text = $"%{stepValue:0}";
        }

        private async void OnGeriClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void OnDersiKaydetClicked(object? sender, EventArgs e)
        {
            // 1. Ders adı doğrulama
            string dersAdi = DersAdiEntry.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(dersAdi))
            {
                await DisplayAlertAsync("Eksik Bilgi", "Lütfen ders adını giriniz.", "Tamam");
                return;
            }

            // 2. Toplam hafta doğrulama
            string toplamHaftaText = ToplamHaftaEntry.Text?.Trim() ?? "";
            if (!int.TryParse(toplamHaftaText, out int toplamHafta) || toplamHafta <= 0)
            {
                await DisplayAlertAsync("Geçersiz Bilgi", "Toplam hafta 0'dan büyük bir tam sayı olmalıdır.", "Tamam");
                return;
            }

            // 3. Haftalık ders saati doğrulama
            if (_haftalikSaat < 1 || _haftalikSaat > 24)
            {
                await DisplayAlertAsync("Geçersiz Bilgi", "Haftalık ders saati 1 ile 24 arasında olmalıdır.", "Tamam");
                return;
            }

            // 4. Devamsızlık sınırı doğrulama
            double devamsizlikSiniri = Math.Round(DevamsizlikSiniriSlider.Value / 5.0) * 5.0;
            if (devamsizlikSiniri <= 0 || devamsizlikSiniri > 50)
            {
                await DisplayAlertAsync("Geçersiz Bilgi", "Devamsızlık sınırı %0'dan büyük ve en fazla %50 olmalıdır.", "Tamam");
                return;
            }

            // 5. Yapılan devamsızlık saati doğrulama (boşsa 0 kabul edilir)
            int yapilanDevamsizlik = 0;
            string yapilanText = YapilanDevamsizlikEntry.Text?.Trim() ?? "";
            if (!string.IsNullOrEmpty(yapilanText))
            {
                if (!int.TryParse(yapilanText, out yapilanDevamsizlik) || yapilanDevamsizlik < 0)
                {
                    await DisplayAlertAsync("Geçersiz Bilgi", "Yapılan devamsızlık 0 veya daha büyük bir tam sayı olmalıdır.", "Tamam");
                    return;
                }
            }

            // Yeni ders nesnesini oluştur
            var yeniDers = new Ders
            {
                DersAdi = dersAdi,
                ToplamHafta = toplamHafta,
                HaftalikSaat = _haftalikSaat,
                DevamsizlikSiniri = devamsizlikSiniri,
                yapilanDevamsizlik = yapilanDevamsizlik
            };

            // Ana sayfaya bildir
            DersEklendi?.Invoke(yeniDers);

            // Ana sayfaya geri dön
            await Shell.Current.GoToAsync("..");
        }
    }
}
