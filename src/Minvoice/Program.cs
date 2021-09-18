using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Fluid;
using Microsoft.Playwright;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var templateSource = await File.ReadAllTextAsync(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "invoice-template.html"));
var parser = new FluidParser();
TemplateOptions.Default.MemberAccessStrategy.Register<InvoiceRequest>();
TemplateOptions.Default.MemberAccessStrategy.Register<Recipient>();
TemplateOptions.Default.MemberAccessStrategy.Register<Address>();
TemplateOptions.Default.MemberAccessStrategy.Register<InvoiceItem>();

if (!parser.TryParse(templateSource, out var template, out var error))
{
    throw new Exception(error);
}

app.MapPost("/invoices", async (InvoiceRequest invoiceRequest) =>
{
    if (!MinimalValidation.TryValidate(invoiceRequest, out var errors))
    {
        return Results.BadRequest(errors);
    }

    var context = new TemplateContext(invoiceRequest);
    string renderedInvoice = template.Render(context);

    using var playwright = await Playwright.CreateAsync();
    await using var browser = await playwright.Chromium.LaunchAsync();
    var page = await browser.NewPageAsync();

    await page.SetContentAsync(renderedInvoice);

    byte[] pdf = await page.PdfAsync(new PagePdfOptions { Format = "A4" });

    return Results.File(pdf, MediaTypeNames.Application.Pdf, "invoice.pdf");
});

app.Run();

public class InvoiceRequest
{

    [Required]
    public string? CompanyName { get; init; }

    [Required]
    public Address? CompanyAddress { get; init; }

    [Required]
    public string? InvoiceNumber { get; init; }

    [Required]
    public string? Currency { get; init; }

    public string? LogoUrl { get; init; }

    [Required]
    public IEnumerable<InvoiceItem> Items { get; init; } = Array.Empty<InvoiceItem>();

    public DateTime Created { get; init; } = DateTime.UtcNow;
    public DateTime Due { get; init; } = DateTime.UtcNow.AddDays(30);

    [Required]
    public Recipient? Recipient { get; init; }

    public decimal Total => Math.Round(Items.Sum(i => i.Amount), 2);
}

public record Recipient(string Company, [Required] string Name, [Required] string Email);
public record Address([Required] string Line1, [Required] string TownCity, string County, [Required] string Zip);
public record InvoiceItem(string Title, int Quantity, decimal Amount);
