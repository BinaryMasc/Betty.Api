# Betty.Api

## Run Docker


1. Create a Docker network
```Bash
docker network create betty-network
```
2. Pull mysql image
```Bash
docker pull mysql
```

3. Run db image
```Bash
docker run --name betty-db --network betty-network -p 3306:3306 -e MYSQL_ROOT_PASSWORD=my-secret-pw -d mysql
```

4. Execute the required commands in **sql_scheme.sql** in the sql interpreter
**Powershell:**
```Bash	
Get-Content sql_scheme.sql | docker exec -i betty-db mysql -uroot -pmy-secret-pw
```
**Linux:**
```Bash	
cat sql_scheme.sql | docker exec -i betty-db mysql -uroot -pmy-secret-pw
```

5. Execute build command in the container folder
```Bash
docker build -t betty-api .
```
6. Then run this
```Bash
docker run --network betty-network -d -p 5000:5000 betty-api
```

That's it! Your .NET Betty is now Dockerized and running inside a Docker container. You can access it at http://localhost:5000