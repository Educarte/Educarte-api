FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
COPY . .
RUN dotnet restore "Api/Api.csproj"
RUN dotnet publish "Api/Api.csproj" -c Release -o /app --no-restore
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runner
ENV ASPNETCORE_ENVIRONMENT=Development
ENV EmailOptions__EmailSender=istic@sp.senai.br
ENV EmailOptions__Key=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
ENV ConnectionStrings__Default=Server=db-mysql-nyc3-61172-do-user-15730039-0.c.db.ondigitalocean.com;Port=25060;Database=educarteDb;Uid=doadmin;Pwd=AVNS_YwTdAOiRLRrkcfoSqgS;SslMode=Required;
ENV SpaceOptions__SpaceName=educarte-storage
ENV SpaceOptions__ServiceURL=https://nyc3.digitaloceanspaces.com/
ENV SpaceOptions__AccessKey=DO00D8V4D8QYULRYQRB4
ENV SpaceOptions__SpaceLink=https://educarte-storage.nyc3.digitaloceanspaces.com
ENV SpaceOptions__AwsSecretAccessKey=dqQprB83+UuwqeBxjA0/nYnyHeIz5u509bVegGw7sTM
EXPOSE 5000
WORKDIR /app
COPY --from=build /app . 
ENTRYPOINT ["./Api"]