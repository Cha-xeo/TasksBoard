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

POST
  Return a Jwt access token needed to accesss other route
  /api/Auth/login
  {
    "userName": "string",
    "password": "string"
  }
  Both required

  Register a user
  /api/Auth/register
  {
    "email": "string",
    "displayName": "nullable",
    "firstName": "nullable",
    "lastName": "nullable",
    "userName": "string",
    "password": "string"
  }
  userName: required, must be unique
  email: must be unique
  password: required
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
  Can be expanded with 'users' to get users details on specified task
  Summary can be set to true (default to false) to get a summary of requested datas
  /api/Tasks?expand=users&summary=false
  -H 'Authorization: Bearer {Token}'

  Get a task with specified {id}
  Can be expanded with 'users' to get users details on specified task
  Summary can be set to true (default to false) to get a summary of requested datas
  /api/Tasks/{id}?expand=users&summary=false
  -H 'Authorization: Bearer {Token}'

POST
  Create a task with specified Users Ids
  /api/Tasks
  -H 'Authorization: Bearer {Token}'
  {
    "name": "string",
    "description": "string",
    "userIDS": [1]
  }
  userIDS: Ids Must exists before creating a task with users, can be left empty.

PUT
  Modifie Task with specified {id}
  /api/Tasks/{id}
  -H 'Authorization: Bearer {Token}'
  {
    "id": {id},
    "name": "string",
    "description": "string",
    "userIDS": [1]
  }
  id: must match the route
  userIDS: empty to clear, omit to not change, will remove any non present id

DELETE
  Delete Task with specified {id}
  /api/Tasks/{id}
  -H 'Authorization: Bearer {Token}'
```

Users
```
GET
  Get all Users
  Can be expanded with 'tasks' to get tasks details on specified user
  Summary can be set to true (default to false) to get a summary of requested datas
  /api/Users?expand=tasks&summary=false
  -H 'Authorization: Bearer {Token}'

  Get a User with specified {id}
  Can be expanded with 'tasks' to get tasks details on specified user
  Summary can be set to true (default to false) to get a summary of requested datas
  /api/Users/{id}?expand=tasks&summary=false
  -H 'Authorization: Bearer {Token}'

POST
  Deleted since registration happen through Auth controller
  /api/Users

PUT
  /api/Users/{id}
  -H 'Authorization: Bearer {Token}'
  {
    "id": {id},
    "email": "string", 
    "displayName": "string",
    "firstName": "string",
    "lastName": "string",
    "userName": "string", 
    "passWord": "string",
    "tasksIDS": [1,2]
  }
  id: must match the route
  email:must be unique
  userName: must be unique
  passWord: omit password to not change it, will be changed soon
  tasksIDS: empty to clear, omit to not change, will remove any non present id

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

TODO restrict who can interact with what
TODO Change how password are updated