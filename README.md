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
  }
}
```
Please note that appsettings.Development.json shoould never be pushed

Sample for adding  Users and Tasks tasksIDS and userIDS must exist before being added to the other
Post
```
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