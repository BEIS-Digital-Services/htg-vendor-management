#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base

EXPOSE 80 443

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["Beis.VendorManagement.Web/Beis.VendorManagement.Web.csproj", "Beis.VendorManagement.Web/"]
RUN dotnet restore "Beis.VendorManagement.Web/Beis.VendorManagement.Web.csproj"
COPY . .
WORKDIR "/src/Beis.VendorManagement.Web"
RUN dotnet build "Beis.VendorManagement.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Beis.VendorManagement.Web.csproj" -c Release -o /app/publish

FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Beis.VendorManagement.Web.dll"]