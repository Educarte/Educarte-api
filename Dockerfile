FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
COPY . .
RUN dotnet restore "Api/Api.csproj"
RUN dotnet publish "Api/Api.csproj" -c Release -o /app --no-restore
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runner
ENV ASPNETCORE_ENVIRONMENT=Development
ENV EmailOptions__EmailSender=istic@sp.senai.br
ENV EmailOptions__Key=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
ENV ConnectionStrings__Default=Server=db-mysql-nyc1-28555-do-user-14599667-0.b.db.ondigitalocean.com;Port=25060;Database=PetFast;Uid=doadmin;Pwd=AVNS_8DN8amIbwpHzM6LOHUE;SslMode=Required;
EXPOSE 8080
WORKDIR /app
COPY --from=build /app . 
ENTRYPOINT ["./Api"]