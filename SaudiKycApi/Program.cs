using SaudiKycApi.Services;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. تسجيل الخدمات (Services Configuration)
// ==========================================

// إضافة المكونات الرئيسية للـ Controllers مع ضبط JSONSerializer لضمان مرونة أسماء الحقول
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// إضافة دعم Swagger/OpenAPI للتوثيق والاختبار
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Saudi KYC Verification API",
        Version = "v1",
        Description = "API للتحقق من بيانات الهوية والآيبان والرقم الضريبي وفق المعايير السعودية"
    });
});

// تسجيل خدمة الـ KYC كـ Singleton لمنع إعادة الإنشاء بدون داعٍ
builder.Services.AddSingleton<KycService>();

// إعداد سياسة CORS لتسمح للطلب بالوصول من متصفحات الـ Frontend بدون قيود أثناء التطوير
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ==========================================
// 2. إعداد البرمجيات الوسيطة (Middleware Pipeline)
// ==========================================

// تفعيل Swagger في بيئة التطوير والإنتاج لسهولة الاختبار
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Saudi KYC API v1");
    c.RoutePrefix = "swagger"; // يمكن الوصول إليه عبر /swagger
});

app.UseHttpsRedirection();

// تفعيل CORS
app.UseCors("AllowAll");

// تفعيل دعم تقديم ملفات الـ Frontend الثابتة من مجلد wwwroot (مثل index.html)
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();

// توجيه الطلبات إلى الـ Controllers
app.MapControllers();

// تشغيل التطبيق
app.Run();