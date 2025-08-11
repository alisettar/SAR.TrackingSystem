## Yapılacaklar

### Genel

- [ ] Tüm arayüz uyarıları türkçe olacak şekilde ayarlanacak
- [ ] Ekip Üyelieri sayfasında datatable arama özelliği olacak.
- [x] Tüm datatable sayfalarında sayfa numaralarının çok fazla olmasından kaynaklı oluşacak hatalar giderilecek.
- [x] Dark/Light tema toggle eklendi
- [x] Dark theme dropdown uyumluluğu eklendi

### Dashboard sayfası için yapılacaklar

- [x] Sektörlere göre Ekip üyesi sayısı dağılımı - Pie Chart veya Bar Chart
- [x] Gelmeyenlerin raporu
- [x] Şehirlere göre ekip üyesi sayısının gösterilmesi (Sadece gelenler)
- [x] Takım sayısının gösterimi
- [x] Takımlardaki ekip üyesi sayısının gösterilmesi (Sadece gelenler)

### Ekip Üyesi sayfası için yapılacaklar

- [x] Ekip üyesi için QRId property'si eklenecek (zorunlu değil) - ✅ Domain'e eklendi
- [x] Tim (Ekip) olarak değiştirilecek. Yanına ekip ekle butonu konulacak
- [x] Ekip Üyesi döndürülürken QRkod ID'si de döndürülecek - ✅ Response'a eklendi
- [x] Görev alanı eklenecek - (Manuel tanımlanacak) - ✅ Role property eklendi
- [x] Volunteer entity sadeleştirildi (sadece FullName, QRId, TeamId, Role)
- [x] Commands/Queries yeni schema'ya uyarlandı
- [x] Views güncellendi (Create/Edit/Index)
- [x] Listeden olmayan yeni eklenen kayıt için QR kod okutularak ID girilecek

### Hareket sayfası için yapılacaklar

- [x] Ekip üyesinin yanında Ekibi (Takımı) da belirtilecek - ✅ View'da TeamName gösteriliyor
- [x] Hareket kaydı için kaynak sektör girmeye gerek yok. Mevcut kurallar üzerinden kaynak otomatik olarak belirlenecek
- [x] Ekip üyesi arama QRId ile veya arama ile yapılacak. Yanına Ekip Üyesi ekle butonu gelecek
- [x] İlk giriş (Alana giriş işlemi) QR üzerinden yapılacağından bunu kolaylaştıran bir arayüz de lazım
- [x] Ekibin toplu hareketi için Ekip seçilerek intikal bölgesi girilecek ve sonra tüm ekip üyelerine hareket tanımlanacak. Böyle bir sürece ihtiyacımız var
- [x] Hareket silme opsiyonu eklenecek

### Sektör sayfası için yapılacaklar

- [x] Sektör silme kaldırılacak (backdoor ile gizli hale getirilecek)

### Takım sayfası için yapılacaklar

- [x] Takım domaini'ne şehir bilgisi eklenecek. Boş geçilebilir.
- [x] Takım detay sayfasında takım üyeleri gösterilecek
- [x] Takım isimlendirmesi "Ekip" olarak değiştirilecek

### Teknik Güncellemeler

- [x] ValidationScriptsPartial view eklendi
- [x] Test dosyaları (VolunteerMockFactory) güncellendi
- [x] Volunteer domain refactor tamamlandı
- [x] Migration hazır (SimplifyVolunteerSchema)
- [x] Team domain'ine City property eklendi
- [x] Team migration hazır (AddCityToTeam)

### DataInitializer

- [ ] Excel dosyasından Ekip Üyeleri otomatik girilecek
