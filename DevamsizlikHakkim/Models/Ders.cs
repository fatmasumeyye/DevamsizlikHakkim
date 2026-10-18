using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Microsoft.Maui.Graphics;

namespace DevamsizlikHakkim.Models
{
    public class Ders : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _dersAdi = "";
        public string DersAdi
        {
            get => _dersAdi;
            set
            {
                if (_dersAdi != value)
                {
                    _dersAdi = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _toplamHafta;
        public int ToplamHafta
        {
            get => _toplamHafta;
            set
            {
                if (_toplamHafta != value)
                {
                    _toplamHafta = value;
                    OnPropertyChanged();
                    NotifyCalculatedProperties();
                }
            }
        }

        private int _haftalikSaat;
        public int HaftalikSaat
        {
            get => _haftalikSaat;
            set
            {
                if (_haftalikSaat != value)
                {
                    _haftalikSaat = value;
                    OnPropertyChanged();
                    NotifyCalculatedProperties();
                }
            }
        }

        private double _devamsizlikSiniri;
        public double DevamsizlikSiniri
        {
            get => _devamsizlikSiniri;
            set
            {
                if (Math.Abs(_devamsizlikSiniri - value) > 0.001)
                {
                    _devamsizlikSiniri = value;
                    OnPropertyChanged();
                    NotifyCalculatedProperties();
                }
            }
        }

        private int _yapilanDevamsizlik;
        public int yapilanDevamsizlik
        {
            get => _yapilanDevamsizlik;
            set
            {
                if (_yapilanDevamsizlik != value)
                {
                    _yapilanDevamsizlik = value;
                    OnPropertyChanged();
                    NotifyCalculatedProperties();
                }
            }
        }

        public void NotifyCalculatedProperties()
        {
            OnPropertyChanged(nameof(ToplamDersSaati));
            OnPropertyChanged(nameof(DevamsizlikHakki));
            OnPropertyChanged(nameof(KullanimOrani));
            OnPropertyChanged(nameof(GuvenliMi));
            OnPropertyChanged(nameof(DurumRengi));
            OnPropertyChanged(nameof(DurumArkaPlanRengi));
            OnPropertyChanged(nameof(CubukDolulukOrani));
            OnPropertyChanged(nameof(KullanimYuzdesi));
            OnPropertyChanged(nameof(KalanDevamsizlikSaati));
        }

        // Toplam ders saati
        [JsonIgnore]
        public int ToplamDersSaati => ToplamHafta * HaftalikSaat;

        // Toplam izin verilen devamsızlık saati hakkı
        [JsonIgnore]
        public double DevamsizlikHakki => ToplamDersSaati * (DevamsizlikSiniri / 100.0);

        // Devamsızlık kullanım oranı
        [JsonIgnore]
        public double KullanimOrani
        {
            get
            {
                if (DevamsizlikHakki <= 0)
                {
                    return 0;
                }

                return yapilanDevamsizlik / DevamsizlikHakki;
            }
        }

        // Devamsızlık durumu
        [JsonIgnore]
        public string GuvenliMi
        {
            get
            {
                if (ToplamHafta <= 0 || HaftalikSaat <= 0 ||
                    DevamsizlikSiniri <= 0 || yapilanDevamsizlik < 0)
                {
                    return "Belirsiz";
                }

                if (KullanimOrani < 0.60)
                {
                    return "Güvenli";
                }
                else if (KullanimOrani <= 1.0)
                {
                    return "Dikkat";
                }
                else
                {
                    return "Sınır Aşıldı";
                }
            }
        }

        // Durum yazısının rengi
        [JsonIgnore]
        public Color DurumRengi
        {
            get
            {
                if (GuvenliMi == "Güvenli")
                    return Color.FromArgb("#229765");

                else if (GuvenliMi == "Dikkat")
                    return Color.FromArgb("#E88729");

                else if (GuvenliMi == "Sınır Aşıldı")
                    return Color.FromArgb("#BF2948");

                else
                    return Colors.Gray;
            }
        }

        // Durum etiketinin arka plan rengi
        [JsonIgnore]
        public Color DurumArkaPlanRengi
        {
            get
            {
                if (GuvenliMi == "Güvenli")
                    return Color.FromArgb("#DFF5E8");

                else if (GuvenliMi == "Dikkat")
                    return Color.FromArgb("#FFF0DC");

                else if (GuvenliMi == "Sınır Aşıldı")
                    return Color.FromArgb("#FCE0E4");

                else
                    return Color.FromArgb("#F0F0F0");
            }
        }

        // ProgressBar doluluk oranı
        [JsonIgnore]
        public double CubukDolulukOrani
        {
            get
            {
                return Math.Clamp(KullanimOrani, 0, 1);
            }
        }

        // Kullanılan hakkın yüzdesi
        [JsonIgnore]
        public double KullanimYuzdesi
        {
            get
            {
                return KullanimOrani * 100.0;
            }
        }

        // Kalan devamsızlık saati
        [JsonIgnore]
        public double KalanDevamsizlikSaati
        {
            get
            {
                double kalan = DevamsizlikHakki - yapilanDevamsizlik;
                return kalan > 0 ? kalan : 0;
            }
        }
    }
}
