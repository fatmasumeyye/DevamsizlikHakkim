using System.Text.Json;
using DevamsizlikHakkim.Models;

namespace DevamsizlikHakkim.Services
{
    public static class DersDeposu
    {
        private static readonly string DosyaYolu = Path.Combine(FileSystem.AppDataDirectory, "dersler.json");

        public static async Task<List<Ders>> DersleriYukleAsync()
        {
            try
            {
                if (File.Exists(DosyaYolu))
                {
                    string json = await File.ReadAllTextAsync(DosyaYolu);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        var liste = JsonSerializer.Deserialize<List<Ders>>(json);
                        if (liste != null)
                        {
                            return liste;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Dersleri yüklerken hata: {ex.Message}");
            }

            // Dosya yoksa veya ilk çalıştırmaysa varsayılan dersi yükle ve kaydet
            var varsayilanListe = new List<Ders>
            {
                new Ders
                {
                    DersAdi = "Mobil Programlama",
                    ToplamHafta = 14,
                    HaftalikSaat = 3,
                    DevamsizlikSiniri = 20,
                    yapilanDevamsizlik = 2
                }
            };

            await DersleriKaydetAsync(varsayilanListe);
            return varsayilanListe;
        }

        public static async Task<bool> DersleriKaydetAsync(IEnumerable<Ders> dersler)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string json = JsonSerializer.Serialize(dersler, options);
                await File.WriteAllTextAsync(DosyaYolu, json);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Dersleri kaydederken hata: {ex.Message}");
                return false;
            }
        }
    }
}
