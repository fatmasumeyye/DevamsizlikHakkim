using DevamsizlikHakkim.Models;

namespace DevamsizlikHakkim
{
    [QueryProperty(nameof(SeciliDers), "Ders")]
    public partial class DersDetayPage : ContentPage
    {
        public static event Action<Ders>? DersGuncellendi;
        public static event Action<Ders>? DersSilindi;

        public static Ders? AktifDers { get; set; }

        private Ders? _seciliDers;
        public Ders? SeciliDers
        {
            get => _seciliDers;
            set
            {
                _seciliDers = value;
                BindingContext = _seciliDers;
                if (_seciliDers != null && ManuelSaatEntry != null)
                {
                    ManuelSaatEntry.Text = _seciliDers.yapilanDevamsizlik.ToString();
                }
            }
        }

        public DersDetayPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (SeciliDers == null && AktifDers != null)
            {
                SeciliDers = AktifDers;
            }

            if (SeciliDers != null)
            {
                BindingContext = SeciliDers;
                ManuelSaatEntry.Text = SeciliDers.yapilanDevamsizlik.ToString();
            }
        }

        private void OnDevamsizlikAzaltClicked(object? sender, EventArgs e)
        {
            if (SeciliDers == null) return;

            if (SeciliDers.yapilanDevamsizlik > 0)
            {
                SeciliDers.yapilanDevamsizlik--;
                ManuelSaatEntry.Text = SeciliDers.yapilanDevamsizlik.ToString();
                DersGuncellendi?.Invoke(SeciliDers);
            }
            else
            {
                DisplayAlertAsync("Bilgi", "Devamsızlık saati 0'ın altına düşemez.", "Tamam");
            }
        }

        private void OnDevamsizlikArttirClicked(object? sender, EventArgs e)
        {
            if (SeciliDers == null) return;

            SeciliDers.yapilanDevamsizlik++;
            ManuelSaatEntry.Text = SeciliDers.yapilanDevamsizlik.ToString();
            DersGuncellendi?.Invoke(SeciliDers);
        }

        private async void OnManuelAyarlaClicked(object? sender, EventArgs e)
        {
            if (SeciliDers == null) return;

            string text = ManuelSaatEntry.Text?.Trim() ?? "";
            if (!int.TryParse(text, out int saat) || saat < 0)
            {
                await DisplayAlertAsync("Geçersiz Bilgi", "Lütfen 0 veya daha büyük bir tam sayı giriniz.", "Tamam");
                return;
            }

            SeciliDers.yapilanDevamsizlik = saat;
            DersGuncellendi?.Invoke(SeciliDers);
            await DisplayAlertAsync("Başarılı", "Devamsızlık saati güncellendi.", "Tamam");
        }

        private async void OnKaydetClicked(object? sender, EventArgs e)
        {
            if (SeciliDers != null)
            {
                string text = ManuelSaatEntry.Text?.Trim() ?? "";
                if (int.TryParse(text, out int saat) && saat >= 0 && saat != SeciliDers.yapilanDevamsizlik)
                {
                    SeciliDers.yapilanDevamsizlik = saat;
                }
                DersGuncellendi?.Invoke(SeciliDers);
            }

            await Shell.Current.GoToAsync("..");
        }

        private async void OnDersiSilClicked(object? sender, EventArgs e)
        {
            if (SeciliDers == null) return;

            bool onay = await DisplayAlertAsync(
                "Dersi Sil",
                $"'{SeciliDers.DersAdi}' dersini silmek istediğinize emin misiniz? Bu işlem geri alınamaz.",
                "Evet, Sil",
                "Vazgeç");

            if (onay)
            {
                DersSilindi?.Invoke(SeciliDers);
                await Shell.Current.GoToAsync("..");
            }
        }

        private async void OnGeriClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
