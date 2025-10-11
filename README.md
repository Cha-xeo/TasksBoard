appsettings.Development.json should ressemble something like that where it links to your local mysql database.
TaskDatabase will be used in Develpoment environment.
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "TaskDatabase": "server=localhost;user=;password=;database=",
  },
  "Jwt": {
    "Key": "MySuperStrongSecretKey1234567890",
    "Issuer": [
      "https://localhost:7085"
    ],
    "Audience": [
      "https://localhost:7085"
    ],
    "TokenValidityMins": 30
  },
  "Authentication": {
    "Schemes": {}
  }
}
```

Sample for adding Interacting with the api
use the Jwt token in header
'Authorization: Bearer {Token}'
Use apiKey in Header
X-API-KEY

Post
```
Return a Jwt access token needed to accesss other route
/api/Auth/login
{
  "userName": "string",
  "password": "string"
}

Register a user
/api/Auth/register
{
  "email": "nullable",
  "displayName": "nullable",
  "firstName": "nullable",
  "lastName": "nullable",
  "userName": "string",
  "password": "string"
}

Return an ApiKey to use inthe other route of sample for now
/api/Sample

/api/User
{
  "age": 0,
  "userName": "string",
  "birthDate": "2025-10-09T07:55:27.140Z",
  "tasks": [
    {
      "id": 1
    }
  ]
}

/api/Task
{
  "name": "test",
  "description": "string",
  "user": [
    {
      "id": 1
    }
  ]
}
```

Get
```
/api/User/


/api/User/{id}
?expand=tasks can be added to show Tasks


/api/Task/


/api/Task/{id}
?expand=users can be added to show Users

```

Put ids in route and body must match
```
/api/User/{id}
{
  "id":1,
  "age": 0,
  "userName": "string",
  "birthDate": "2025-10-09T07:55:27.140Z",
  "tasks": [
    {
      "id": 1
    }
  ]
}

/api/Task/{id}
{
  "id": 1,
  "name": "test",
  "description": "string",
  "user": [
    {
      "id": 1
    }
  ]
}
```

Delete
```
/api/User/{id}
/api/Task/{id}
```