# RequestLogger
An API used to log all requests so that they can be reviewed at a later point

### Local installation

#### Docker

This project makes use of entity framework code-first and a postgres SQL database. in order to run the application, the following steps must be followed.

first there is a local docker compose file found in the `compose` directory.  This can be run from that directory with the following command:

```powershell
docker-compose up
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

#### RequestLogger Settings

by default, the application is set to use settings from the `appsettings.Docker.json` app settings file.  If changes are made to this file after the containers are built, the command `docker-compose build` will need to be run from the `compose` folder.

#### Entity Framework

Once the docker containers are built, the database can be constructed by setting the `RunMigrations` value to `true` in the `appsettings` and the migrations will be run on startup.

##### Adding migrations

Migrations can be added with the following commaand being run from the `src` directory

```powershell
dotnet ef migrations add <migration name> -p .\Repository\ -s .\RequestLogger\
```

### Debugging

There is a `docker-compose.local.yml` that can be used when debugging the RequestLogger app. 

This will start all of the same resources as main `docker-compose.yml` with the following exceptions:

* RequestLogger is _not_ ran
* Nginx is using host port `:7020` (https port for RequestLogger) as mirror destination

This allows RequestLogger to be run and save to Postgres instance running via compose. Any request to `http://localhost:8080/` will be mirrored to running RequestLogger instance.

This can be ran via:

```bash
cd compose && docker compose -f docker-compose.local.yml up
```