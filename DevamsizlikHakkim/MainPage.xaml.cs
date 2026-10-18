using System.Collections.ObjectModel;
using DevamsizlikHakkim.Models;
using DevamsizlikHakkim.Services;

namespace DevamsizlikHakkim
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Ders> Dersler { get; set; } = new();

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;

            YeniDersPage.DersEklendi += OnDersEklendi;
            DersDetayPage.DersGuncellendi += OnDersGuncellendi;
            DersDetayPage.DersSilindi += OnDersSilindi;

            _ = DersleriYukleAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            IstatistikleriGuncelle();
        }

        private async Task DersleriYukleAsync()
        {
            try
            {
                var yuklenenDersler = await DersDeposu.DersleriYukleAsync();
                Dersler.Clear();
                foreach (var ders in yuklenenDersler)
                {
                    Dersler.Add(ders);
                }
                IstatistikleriGuncelle();
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Hata", $"Dersler yüklenirken bir sorun oluştu: {ex.Message}", "Tamam");
            }
        }

        private async void OnDersEklendi(Ders yeniDers)
        {
            Dersler.Add(yeniDers);
            IstatistikleriGuncelle();

            bool basarili = await DersDeposu.DersleriKaydetAsync(Dersler);
            if (!basarili)
            {
                await DisplayAlertAsync("Kayıt Hatası", "Yeni ders kaydedilirken bir hata oluştu.", "Tamam");
            }
        }

        private async void OnDersGuncellendi(Ders guncellenenDers)
        {
            IstatistikleriGuncelle();

            bool basarili = await DersDeposu.DersleriKaydetAsync(Dersler);
            if (!basarili)
            {
                await DisplayAlertAsync("Kayıt Hatası", "Ders güncellemeleri kaydedilirken bir hata oluştu.", "Tamam");
            }
        }

        private async void OnDersSilindi(Ders silinenDers)
        {
            if (Dersler.Contains(silinenDers))
            {
                Dersler.Remove(silinenDers);
            }
            else
            {
                // Referans eşleşmezse DersAdi'na göre eşleştirip sil
                var mevcut = Dersler.FirstOrDefault(d => d.DersAdi == silinenDers.DersAdi);
                if (mevcut != null)
                {
                    Dersler.Remove(mevcut);
                }
            }

            IstatistikleriGuncelle();

            bool basarili = await DersDeposu.DersleriKaydetAsync(Dersler);
            if (!basarili)
            {
                await DisplayAlertAsync("Kayıt Hatası", "Ders silme işlemi kaydedilirken bir hata oluştu.", "Tamam");
            }
        }

        public void IstatistikleriGuncelle()
        {
            ToplamDers.Text = Dersler.Count.ToString();
            Guvenli.Text = Dersler.Count(d => d.GuvenliMi == "Güvenli").ToString();
            Dikkat.Text = Dersler.Count(d => d.GuvenliMi == "Dikkat").ToString();
        }

        private async void OnDerseGitmedimClicked(object? sender, EventArgs e)
        {
            if (sender is not Button button || button.CommandParameter is not Ders ders)
            {
                return;
            }

            // 1. "Kaç saat derse gelmediniz?" sorusunu göster
            // 2. Varsayılan olarak o dersin HaftalikSaat değerini öner
            // 3. Kullanıcı bu değeri değiştirebilsin
            string? result = await DisplayPromptAsync(
                "Derse Gitmedim",
                $"{ders.DersAdi} için kaç saat derse gelmediniz?",
                accept: "Onayla",
                cancel: "İptal",
                placeholder: "Saat miktarı giriniz",
                keyboard: Keyboard.Numeric,
                initialValue: ders.HaftalikSaat.ToString());

            // 4. Kullanıcı iptal ederse hiçbir işlem yapma
            if (result == null)
            {
                return;
            }

            string input = result.Trim();
            if (string.IsNullOrEmpty(input))
            {
                return;
            }

            // 5. Yalnızca pozitif tam sayı kabul et
            if (!int.TryParse(input, out int girilenSaat) || girilenSaat <= 0)
            {
                await DisplayAlertAsync("Geçersiz Değer", "Lütfen 0'dan büyük pozitif bir tam sayı giriniz.", "Tamam");
                return;
            }

            // 6. Onaylarsa girilen saat miktarını mevcut yapilanDevamsizlik değerine ekle (üzerine yazma, topla)
            ders.yapilanDevamsizlik += girilenSaat;

            // 7. Hesaplamaları ve istatistikleri otomatik güncelle
            IstatistikleriGuncelle();

            // 8. Kalıcı olarak kaydet
            bool basarili = await DersDeposu.DersleriKaydetAsync(Dersler);
            if (!basarili)
            {
                await DisplayAlertAsync("Kayıt Hatası", "Devamsızlık kaydedilirken bir hata oluştu.", "Tamam");
            }
        }

        private async void OnDersSecildi(object? sender, EventArgs e)
        {
            Ders? seciliDers = null;

            if (e is TappedEventArgs tappedEventArgs && tappedEventArgs.Parameter is Ders d1)
            {
                seciliDers = d1;
            }
            else if (sender is Element element && element.BindingContext is Ders d2)
            {
                seciliDers = d2;
            }

            if (seciliDers == null) return;

            DersDetayPage.AktifDers = seciliDers;
            await Shell.Current.GoToAsync(nameof(DersDetayPage), new Dictionary<string, object>
            {
                { "Ders", seciliDers }
            });
        }

        private async void OnYeniDersEkleClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(YeniDersPage));
        }
    }
}