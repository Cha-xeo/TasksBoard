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

Routes

Auth
```

Return a Jwt access token needed to accesss other route
POST
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
```

Sample
```
GET
  Test whether your jwt token is working
  /api/Sample/jwt
  -H 'Authorization: Bearer {Token}'

  Test whether your ApiKey is working
  /api/Sample/apikey
  -H 'X-API-KEY: {Key}'

  Test whether your ApiKey or your jwt token is working
  /api/Sample/either
  -H 'Authorization: Bearer {Token}'
  -H 'X-API-KEY: {Key}'

POST
  Return a functionning ApiKey if jwt token is valid
  /api/sample
  -H 'Authorization: Bearer {Token}'
```

TaskBoard.Api
```
GET
  No specifiq access needed
  /
  return Hello world

  Require a valid Jwt Token
  /secret

  Require a valid Jwt Token with the correct scope ( not implemented yet)
  /secret2
```

Tasks
```
GET
  Get all task
  /api/Tasks
  -H 'Authorization: Bearer {Token}'

  Get a task with specified {id}
  Can be expanded with 'users' to get user details on specified task
  /api/Tasks/{id}?expand=["", users]
  -H 'Authorization: Bearer {Token}'

POST
  Create a task with specified Users Ids
  /api/Tasks
  -H 'Authorization: Bearer {Token}'
  {
    "name": "string",
    "description": "string",
    "userIDS": [
      1
    ]
  }

PUT
  Modifie Task with specified {id}
  /api/Tasks/{id}
  -H 'Authorization: Bearer {Token}'
  {
    "id": {id},
    "name": "string",
    "description": "string",
    "userIDS": [
      1
    ]
  }

DELETE
  Delete Task with specified {id}
  /api/Tasks/{id}
  -H 'Authorization: Bearer {Token}'
```

Users ( POST adn PUT will extensivly be reworked next push)
```
GET
  Get all Users
  /api/Users
  -H 'Authorization: Bearer {Token}'

  Get a User with specified {id}
  Can be expanded with 'tasks' to get task details on specified user
  /api/Users/{id}?expand=["", tasks]
  -H 'Authorization: Bearer {Token}'

POST
  Create a new User, should not be used since registration happen through Auth controller
  /api/Users
  -H 'Authorization: Bearer {Token}'

PUT
  /api/Users
  -H 'Authorization: Bearer {Token}'

DELETE
  Delete user with specified {id}
  /api/Users/{id}
  -H 'Authorization: Bearer {Token}'

PATCH
  Soft delete user with specified {id}
  /api/Users/{id}/soft-delete
  -H 'Authorization: Bearer {Token}'

  Restore a soft deleted user with specified {id}
  /api/Users/{id}/restore
  -H 'Authorization: Bearer {Token}'
```
