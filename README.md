# Minvoice

Minvoice is a minimal PDF invoice generation API built using ASP.NET Core. It uses [Playwright](https://playwright.dev/) to convert a rendered HTML template to PDF using Chromium.

## Building and running locally

Minvoice takes advantage of a number of new features in .NET 6 and C# 10. To run locally you'll need to [install .NET 6.0 rc1](https://github.com/dotnet/installer).

To run the application:

```
dotnet run --project ./src/Minvoice/Minvoice.csproj
```

This will start Minvoice at `http://localhost:5000`. You can then generate your first invoice:

```
POST http://localhost:5000/invoices HTTP/1.1
content-type: application/json

{
  "InvoiceNumber": "123456",
  "CompanyName": "Acme Ltd.",
  "CompanyAddress": {
    "Line1": "Mansfield House",
    "TownCity": "London",
    "Zip": "WC12 4HP"
  },
  "Currency": "GBP",
  "Title": "Invoice from Acme",
  "Recipient": {
    "Name": "John Doe",
    "Email": "johndoh@gmail.com"
  },
  "Items": [
    {
      "Title": "Website design",
      "Amount": 300.00
    },
    {
      "Title": "Hosting (3 months)",
      "Amount": 75.00
    },
    {
      "Title": "Domain name (1 year)",
      "Amount": 10.00
    }
  ],
  "LogoUrl": "https://upload.wikimedia.org/wikipedia/commons/8/80/Logo_acme.svg"
}
```

This will return the generated PDF invoice in the response:

![Generating an invoice](/assets/postman.png)


## Modifying the invoice template

Feel free to tweak the [invoice template](./src/Minvoice/invoice-template.html) to suit your needs. Credit for this template goes to 

## Running in docker

To run Minvoice in docker, first build the image:

```
docker build -t minvoice -f ./src/Minvoice/Dockerfile .
```

Then run:

```
docker run --rm -it -p 5000:5000 minvoice
```

If you would like to override the invoice template when running in docker, use the following command:

```
docker run --rm -it -p 5000:5000 \
    -v $(pwd)/custom-template.html:/app/invoice-template.html minvoice
```

## Built with

- [ASP.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Playwright](https://playwright.dev/)
- [Fluid](https://github.com/sebastienros/fluid)
- [Invoice Template](https://github.com/sparksuite/simple-html-invoice-template)
