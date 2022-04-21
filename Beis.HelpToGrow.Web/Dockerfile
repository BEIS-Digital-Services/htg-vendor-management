#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:3.1-alpine AS base
# Install OpenSSH and set the password for root to "Docker!". In this example, "apk add" is the install instruction for an Alpine Linux-based image.
RUN apk add openssh \
     && echo "root:Docker!" | chpasswd

# Copy the sshd_config file to the /etc/ssh/ directory
COPY sshd_config /etc/ssh/

RUN /usr/bin/ssh-keygen -A

EXPOSE 80 443 2222

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:3.1-alpine AS build
WORKDIR /src
COPY ["Beis.HelpToGrow.Web/Beis.HelpToGrow.Web.csproj", "Beis.HelpToGrow.Web/"]
RUN dotnet restore "Beis.HelpToGrow.Web/Beis.HelpToGrow.Web.csproj"
COPY . .
WORKDIR "/src/Beis.HelpToGrow.Web"
RUN dotnet build "Beis.HelpToGrow.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Beis.HelpToGrow.Web.csproj" -c Release -o /app/publish

FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .

CMD /usr/sbin/sshd -D -p 2222 & dotnet Beis.HelpToGrow.Web.dll

# ENTRYPOINT ["dotnet", "Beis.HelpToGrow.Web.dll"]