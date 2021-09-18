## Build
FROM mcr.microsoft.com/dotnet/sdk:6.0.100-rc.1-focal-amd64 as build

WORKDIR /source

# copy csproj and restore as distinct layers
COPY src/**/*.csproj .
COPY nuget.config .
RUN dotnet restore --runtime ubuntu.20.04-x64

# copy everything else and build app
COPY ./src/Html2Pdf .
RUN dotnet publish --runtime ubuntu.20.04-x64 -c Release -o /publish --self-contained true --no-restore

FROM mcr.microsoft.com/playwright:v1.14.1-focal AS playwright
COPY --from=build /publish/ /app/
RUN chown -R pwuser:pwuser /app
EXPOSE 80
ENTRYPOINT [ "/app/Html2Pdf" ]

