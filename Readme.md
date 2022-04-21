# Help To Grow Back End API

BEIS Help to Grow Back End API codebase.

## TL;DR
- Go to [localhost:65443] (For Swagger: https://localhost:65443/swagger/index.html)

See the `docs` subdirectory for help with specific tasks.

## Pre-requisites

- [Docker](https://docs.docker.com/get-docker/)
- [Docker Compose](https://docs.docker.com/compose/install/)

## .NET Core

This project uses:

* .NET Core 3.1 LTS


### Dependencies

You will need to restore / install the nuget package dependencies locally, even though we're using
Docker Compose to run locally.

Right click on the solutio and select `restore nuget packages`

## Running

> Docker and Docker Compose are both very well documented. Please check their
> documentation for advice on running it in development.

To run the development stack, if using visual studio you can just run directly or `F5`. Alternatively you can run this in Docker Compose. The following `dotnet run` command will
start the project:

```
dotnet run
```

Then go to [localhost:65443] (For Swagger: https://localhost:65443/swagger/index.html)
