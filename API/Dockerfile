# create docker image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
# sets diectory that the container starts in (any coammnds executed in this file happens in this working directory)
WORKDIR /app
EXPOSE 8080

# copy csproj and restore as distinct layers
COPY *.csproj ./
# entity packages and other things used inside CSS page will be restored into container(working directory)
RUN dotnet restore

# copy everything else and build (take all content inside projects and copy into working directory)
COPY . ./
RUN dotnet publish -c Release -o out

# build runtime image (instead of running in container)
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
# Copy from out directory into root container
COPY --from=build-env /app/out .
# looks inside program class and executes code inside there and runs application
ENTRYPOINT ["dotnet", "API.dll"]
