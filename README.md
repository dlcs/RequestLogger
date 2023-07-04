# RequestLogger
An API used to log all requests so that they can be reviewed at a later point

### Local installation

#### Docker

This project makes use of entity framework code-first and a postgres SQL database. in order to run the application, the following steps must be followed.

first there is a local docker compose file found in the `compose` directory.  This can be run from that directory with the following command:

```powershell
docker-compose -f docker-compose-local.yml up
```
**Note:** the postgres container needs environment variables in a `.env` file. There's an example in the project under `.env-dist`, but it's reccomended to change the username and password to something more secure.

This will run 4 containers.  These are as follows:

- [Nginx](https://hub.docker.com/_/nginx) 
    - The application is designed to run as a [mirror](http://nginx.org/en/docs/http/ngx_http_mirror_module.html) for the main application behind Nginx. This container is to mimic this behaviour
- [Mock Server](https://www.mock-server.com/#what-is-mockserver)
    - This is to mock out a succesful response from all endpoints going through the initial `proxy_pass` from Nginx. it will always respond with `{"test": "test"}`
- [PostgreSQL](https://hub.docker.com/_/postgres)
    - This is the database that holds all the request being logged by the request logger
- A built version of the Request Logger application
    - This is built from the Dockerfile found in RequestLogger

#### Entity Framework

Once the docker containers are built, the database can be constructed by running the following command from the `src` directory

**Note:** you will need to replace the `User Id` and `Password` values with your own

```powershell
dotnet ef database update --project .\Repository\ --connection "Server=127.0.0.1;Port=5452;Database=postgres;User Id=<username>;Password=<password>;"
```

Alternatively, the `RunMigrations` value can be set to `true` in the `appsettings` and the migrations will be run when the code is run from the project.