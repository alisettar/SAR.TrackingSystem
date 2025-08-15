using Newtonsoft.Json.Linq;
using System.Globalization;

namespace SAR.TrackingSystem.DataProcessor;

class Program
{
    static readonly HashSet<string> TurkishCities = new(StringComparer.OrdinalIgnoreCase)
    {
        "Adana", "Adıyaman", "Afyonkarahisar", "Ağrı", "Amasya", "Ankara", "Antalya", "Alanya", "Artvin",
        "Aydın", "Balıkesir", "Bilecik", "Bingöl", "Bitlis", "Bolu", "Burdur", "Bursa",
        "Çanakkale", "Çankırı", "Çorum", "Denizli", "Diyarbakır", "Edirne", "Elazığ", "Erzincan",
        "Erzurum", "Eskişehir", "Gaziantep", "Giresun", "Gümüşhane", "Hakkâri", "Hatay", "Isparta",
        "Mersin", "İstanbul", "İzmir", "Kars", "Kastamonu", "Kayseri", "Kırklareli", "Kırşehir",
        "Kocaeli", "Konya", "Kütahya", "Malatya", "Manisa", "Kahramanmaraş", "Mardin", "Muğla",
        "Muş", "Nevşehir", "Niğde", "Ordu", "Rize", "Sakarya", "Samsun", "Siirt", "Sinop",
        "Sivas", "Tekirdağ", "Tokat", "Trabzon", "Tunceli", "Şanlıurfa", "Uşak", "Van",
        "Yozgat", "Zonguldak", "Aksaray", "Bayburt", "Karaman", "Kırıkkale", "Batman", "Şırnak",
        "Bartın", "Ardahan", "Iğdır", "Yalova", "Karabük", "Kilis", "Osmaniye", "Düzce"
    };

    static readonly HashSet<string> GozlemciNames =
    [
        "harun çakır", "emin furkan bostan", "ensar dönmez", "emre toprak", 
        "mehmet ender hazar", "mehmet akıl", "abidin araboğa", "halit hakan derindere",
        "fatih sağlam", "hayrullah karakaş", "ibrahim ayan", "seyfullah furkan kılıç",
        "ibrahim taşdemir", "murat şaban tosun", "fatih özmen", "osman çavuş",
        "yavuz bilgen", "abdurrahman şeşe", "emrah ateş", "muhammet coşkun",
        "ramazan demirhan", "süleyman mengü"
    ];

    static readonly CultureInfo TurkishCulture = new CultureInfo("tr-TR");

    static async Task Main(string[] args)
    {
        var yakaKartiPath = "Data/2025_yaka_karti_liste_v2.json";
        var ekipGorevPath = "Data/ekip_gorevlendirme.json";
        var outputPath = "SeedData/2025_yaka_karti_liste_processed.json";
        
        if (!File.Exists(yakaKartiPath))
        {
            Console.WriteLine($"Dosya bulunamadı: {yakaKartiPath}");
            return;
        }

        try
        {
            // 1. Ana dosyayı oku ve capitalize et
            var yakaKartiContent = await File.ReadAllTextAsync(yakaKartiPath);
            var yakaKartiArray = JArray.Parse(yakaKartiContent);
            
            foreach (var item in yakaKartiArray)
            {
                CapitalizeAllUppercaseValues(item);
            }
            Console.WriteLine($"Ana dosya capitalize edildi: {yakaKartiArray.Count} kayıt");

            // 2. Ekip görevlendirme dosyasını oku ve capitalize et
            Dictionary<string, string> ekipGorevDict = [];
            if (File.Exists(ekipGorevPath))
            {
                var ekipGorevContent = await File.ReadAllTextAsync(ekipGorevPath);
                var ekipGorevArray = JArray.Parse(ekipGorevContent);
                
                foreach (var item in ekipGorevArray)
                {
                    CapitalizeAllUppercaseValues(item);
                    
                    var adSoyad = item["AdSoyad"]?.ToString()?.Trim();
                    var gorev = item["Görev"]?.ToString()?.Trim();
                    
                    if (!string.IsNullOrWhiteSpace(adSoyad) && !string.IsNullOrWhiteSpace(gorev))
                    {
                        var key = adSoyad.ToLower(TurkishCulture);
                        ekipGorevDict[key] = gorev;
                    }
                }
                Console.WriteLine($"Ekip görevlendirme capitalize edildi: {ekipGorevDict.Count} kayıt");
            }

            // 3. Match işlemleri
            foreach (var item in yakaKartiArray)
            {
                var adSoyad = item["AdSoyad"]?.ToString()?.Trim();
                if (string.IsNullOrWhiteSpace(adSoyad)) continue;
                
                var adSoyadLower = adSoyad.ToLower(TurkishCulture);
                
                // Önce ekip görevlendirme match
                if (ekipGorevDict.TryGetValue(adSoyadLower, out var ekipGorev))
                {
                    item["Görev2"] = ekipGorev == "Enkaz Çalişmasi" ? "Arama Kurtarma" : ekipGorev;
                    Console.WriteLine($"Ekip görevi eklendi: {adSoyad} -> {ekipGorev}");
                }
                
                // Sonra gözlemci match (override eder)
                if (GozlemciNames.Contains(adSoyadLower))
                {
                    item["Görev2"] = "Gözlemci";
                    Console.WriteLine($"Gözlemci eklendi: {adSoyad}");
                }
                
                // Şehir ayrıştırma
                if (item["Ekip"] != null)
                {
                    var gorev = item["Ekip"]?.ToString();
                    var ekip = ExtractCityFromGorev(gorev) ?? "Belirsiz";
                    item["Şehir"] = ekip;
                }

                // Field isimlerini yeniden düzenle
                RenameFields(item);
            }
            
            await File.WriteAllTextAsync(outputPath, yakaKartiArray.ToString());
            Console.WriteLine($"İşlem tamamlandı: {outputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
        }
    }

    static void RenameFields(JToken item)
    {
        // Mevcut değerleri al
        var gorevValue = item["Görev"]?.ToString();
        var gorev2Value = item["Görev2"]?.ToString();

        // Eski alanları kaldır
        if (item["Görev"] != null) ((JObject)item).Remove("Görev");
        if (item["Görev2"] != null) ((JObject)item).Remove("Görev2");

        // Yeni isimlerde ekle
        if (!string.IsNullOrWhiteSpace(gorevValue))
            item["Ekip"] = gorevValue;
        
        if (!string.IsNullOrWhiteSpace(gorev2Value))
        {
            Console.WriteLine($"Görev eklendi: {gorev2Value}");
            item["Görev"] = gorev2Value;
        }
        else
        {
            item["Görev"] = "Arama Kurtarma";
        }
    }

    static string? ExtractCityFromGorev(string? gorev)
    {
        if (string.IsNullOrWhiteSpace(gorev))
            return null;

        var words = gorev.Split([' ', ',', '-', '(', ')', '/', '\\'], 
                              StringSplitOptions.RemoveEmptyEntries);
        
        return words.FirstOrDefault(word => TurkishCities.Contains(word.Trim()));
    }

    static void CapitalizeAllUppercaseValues(JToken token)
    {
        switch (token.Type)
        {
            case JTokenType.Object:
                foreach (var property in token.Children<JProperty>().ToList())
                {
                    CapitalizeAllUppercaseValues(property.Value);
                }
                break;
                
            case JTokenType.Array:
                foreach (var item in token.Children().ToList())
                {
                    CapitalizeAllUppercaseValues(item);
                }
                break;
                
            case JTokenType.String:
                var stringValue = token.Value<string>();
                if (!string.IsNullOrWhiteSpace(stringValue) && IsAllUppercase(stringValue))
                {
                    var capitalizedValue = CapitalizeString(stringValue);
                    if (token.Parent is JProperty property)
                    {
                        property.Value = capitalizedValue;
                    }
                }
                break;
        }
    }

    static bool IsAllUppercase(string text)
    {
        return text.Any(char.IsLetter) && text.Where(char.IsLetter).All(char.IsUpper);
    }

    static string CapitalizeString(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        var textInfo = new CultureInfo("tr-TR", false).TextInfo;
        return textInfo.ToTitleCase(text.ToLowerInvariant());
    }
}
