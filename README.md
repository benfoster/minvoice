# Minvoice

Minvoice is a minimal PDF invoice generation API built using ASP.NET Core. It uses [Playwright](https://playwright.dev/) to convert a rendered HTML template to PDF using Chromium.

## Run in Docker

The fastest way to launch Minvoice is via [Docker](https://hub.docker.com/r/benfoster/minvoice):

```
docker run -p 5000:5000 --rm -it benfoster/minvoice:latest
```

_Note that the image is quite large (~500MB) due to Playwright's dependencies_

You can override the [default invoice template](./src/Minvoice/invoice-template.html) with your own HTML file:

```
docker run --rm -it -p 5000:5000 \
    -v $(pwd)/custom-template.html:/app/invoice-template.html benfoster/minvoice:latest
```

This will start Minvoice at `http://localhost:5000`. You can then generate your first invoice:

```
POST http://localhost:5000/invoices HTTP/1.1
content-type: application/json

{
  "invoice_number": "123456",
  "company_name": "Acme Ltd.",
  "company_address": {
    "line1": "Mansfield House",
    "town_city": "London",
    "zip": "WC12 4HP"
  },
  "currency": "GBP",
  "title": "Invoice from Acme",
  "recipient": {
    "name": "John Doe",
    "email": "johndoh@gmail.com"
  },
  "items": [
    {
      "title": "Website design",
      "amount": 300.00
    },
    {
      "title": "Hosting (3 months)",
      "amount": 75.00
    },
    {
      "title": "Domain name (1 year)",
      "amount": 10.00
    }
  ],
  "logo_url": "https://upload.wikimedia.org/wikipedia/commons/8/80/Logo_acme.svg"
}
```

This will return the generated PDF invoice in the response:

![Generating an invoice](/assets/postman.png)


## Building and running locally

Minvoice takes advantage of a number of new features in .NET 6 and C# 10. To run locally you'll need to [install .NET 6.0 rc1](https://github.com/dotnet/installer).

To build and run the application:

```
dotnet run --project ./src/Minvoice/Minvoice.csproj
```
## Built with

- [ASP.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Playwright](https://playwright.dev/)
- [Fluid](https://github.com/sebastienros/fluid)
- [Invoice Template](https://github.com/sparksuite/simple-html-invoice-template)
