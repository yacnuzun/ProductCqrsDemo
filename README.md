# ProductCqrsDemo

**CQRS pattern'ini MediatR ile nasıl uyguladığımı gösteren tek amaçlı bir demo.**

CRUD'un kendisi önemli değil. Önemli olan CRUD'un arkasında klasik bir `ProductService` yerine
**Command / Query / Handler / Mediator** zincirinin olması.

---

## Fikir

Klasik katmanlı mimaride controller bir `IProductService`'e bağlıdır ve o servis
hem okuma hem yazma sorumluluğunu taşır. CQRS bu sorumlulukları ikiye ayırır:

| | Command | Query |
|---|---|---|
| Amaç | Durum değiştirir | Sadece veri döndürür |
| Dönüş | Yok ya da sadece `Id` | `ProductDto` |
| EF tracking | Açık | `AsNoTracking()` |
| Örnek | `CreateProductCommand` | `GetAllProductsQuery` |

Controller hiçbir handler'ı tanımaz. Sadece bir mesaj nesnesi üretir ve `ISender.Send()` ile
MediatR'a verir. MediatR, DI container'da kayıtlı `IRequestHandler<TRequest, TResponse>`
implementasyonunu tipe göre bulup çağırır.

HTTP → Controller → ISender.Send(Command) → ValidationBehavior → Handler → DbContext → PostgreSQL


`ProductsController.cs` dosyasının `using` listesinde tek bir handler geçmez.
Handler'ı silsen controller yine derlenir — bağımlılık derleme zamanında değil, çalışma zamanında kurulur.

---

## Pipeline behavior: asıl kazanç

Doğrulama hiçbir handler'ın içinde değil. `ValidationBehavior<TRequest, TResponse>` tek bir yerde
kayıtlı ve **her** command/query ondan geçiyor:

```csharp
cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
```

Yeni bir command eklediğinde bir validator yazarsın, otomatik devreye girer. Kayıt yok, wiring yok.
Klasik yaklaşımda bu mantık ya controller'a girer ya da beş serviste tekrarlanırdı.

Validator'ı olmayan request'ler (iki query) doğrudan handler'a geçer.

---

## Mimari
```
src/ProductCqrsDemo.Api/
├── Modules/Products/
│ ├── Domain/ → Product entity (EF ve MediatR'dan habersiz, tek using yok)
│ ├── Application/
│ │ ├── Commands/ → Create, Update, Delete
│ │ ├── Queries/ → GetAll, GetById
│ │ ├── Handlers/ → her command/query için bir handler
│ │ ├── Validators/ → FluentValidation kuralları
│ │ └── Dtos/ → ProductDto
│ ├── Infrastructure/ → ProductsDbContext, ProductConfiguration
│ └── Api/ → ProductsController (sadece ISender)
└── Shared/
├── Behaviors/ → ValidationBehavior
├── Exceptions/ → NotFoundException
└── Handlers/ → NotFound ve Validation exception handler'ları
```

Katmanlar ayrı `.csproj` değil, klasör seviyesinde. Tek bir pattern'i göstermek için açılan bir repoda
dört ayrı proje, anlatılan şeyi gölgede bırakırdı. Bağımlılık yönü yine de doğru:
`Api → Application → Domain`, ve `Domain` hiçbir şeye bağlı değil.

---

## Endpoint'ler

| Method | Route | Gönderilen mesaj | Yanıt |
|---|---|---|---|
| `POST` | `/api/products` | `CreateProductCommand` | 201 + Guid / 400 |
| `GET` | `/api/products` | `GetAllProductsQuery` | 200 + liste |
| `GET` | `/api/products/{id}` | `GetProductByIdQuery` | 200 + DTO / 404 |
| `PUT` | `/api/products/{id}` | `UpdateProductCommand` | 204 / 400 / 404 |
| `DELETE` | `/api/products/{id}` | `DeleteProductCommand` | 204 / 404 |

Hata yanıtları RFC 9457 `ProblemDetails` formatında. Handler'lar `NotFoundException` fırlatır,
HTTP kodunu `IExceptionHandler` belirler — beş handler'ın hiçbirinde HTTP kavramı geçmez.

---

## Teknoloji

.NET 10 · MediatR 12.4.1 · EF Core 10 · PostgreSQL 16 · FluentValidation 12 · Scalar (OpenAPI 3.1)

OpenAPI dokümanı .NET'in native generator'ı ile üretiliyor (`Microsoft.AspNetCore.OpenApi`),
UI için Scalar kullanılıyor. Swashbuckle .NET 9'dan itibaren şablon varsayılanı değil.

---

## Çalıştırma

```bash
docker compose up -d
dotnet ef database update -p src/ProductCqrsDemo.Api -s src/ProductCqrsDemo.Api
dotnet run --project src/ProductCqrsDemo.Api
```

Scalar UI: `https://localhost:7061/scalar/v1`
OpenAPI JSON: `https://localhost:7061/openapi/v1.json`

---

## Bilinçli olarak kapsam dışı

- **Ayrı read/write modeli** — aynı DB, aynı entity; sadece Command/Query nesneleri ayrı
- **Event sourcing** — CQRS'in ön koşulu değil, ayrı bir konu
- **Repository katmanı** — handler zaten tek bir işlemin tamamı; araya bir soyutlama daha koymak dolaylılığı artırırdı
- **Transaction / concurrency senaryoları** ve **unit test derinliği** — ayrı bir projeye bırakıldı

Bu repo tek bir şeyi net göstermek için var.

---

## CQRS'in maliyeti

Dürüst olmak gerekirse: dosya sayısı arttı. Tek bir CRUD için 1 servis yerine
5 command/query + 5 handler + 2 validator var. "Bu endpoint hangi kodu çalıştırıyor"
sorusu da artık F12 ile cevaplanmıyor.

Bu maliyet, pipeline behavior'lar yeterince kullanıldığında karşılanır.
Kullanılmıyorsa CQRS gereksiz bir dolaylılık katmanıdır.

---

## Lisans

MIT