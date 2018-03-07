FROM microsoft/aspnetcore:2.0
ARG source
WORKDIR /app

COPY /app .
EXPOSE 8080
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "Derivco.Orniscient.Viewer.dll",  "–host=0.0.0.0"]
