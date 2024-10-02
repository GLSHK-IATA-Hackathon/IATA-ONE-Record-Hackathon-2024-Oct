#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM harbor.ad.glshk.com/baseimgproxy/dotnet/aspnet:6.0.27
WORKDIR /app
EXPOSE 80
EXPOSE 443

#build through Jenkins: "COPY app ."
#COPY app .
#build at local [Window docker with Linux container]: "COPY app ."
COPY . app 

ENTRYPOINT ["dotnet", "WebAPITemplate.dll"]