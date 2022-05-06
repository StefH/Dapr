# Dapr

Default component configuration files are stored in the `$HOME/.dapr/components` folder on
Linux/macOS, and in the `%USERPROFILE%\.dapr\components` folder on Windows.

## Dapr Dashboard
``` cmd
dapr dashboard
```

## Dapr List
``` cmd
PS C:\Users\StefHeyenrath> dapr list
  APP ID       HTTP PORT  GRPC PORT  APP PORT  COMMAND     AGE  CREATED              PID
  DaprCounter  55777      55778      0         dotnet run  41s  2022-05-06 22:10.23  16332
```


# Server
Test if running

``` cmd
curl http://localhost:3500/v1.0/invoke/MyBackend/method/weatherforecast
```

Response:
``` json
[{"date":"2022-05-07T23:24:39.848279+02:00","temperatureC":36,"temperatureF":96,"summary":"Warm"},{"date":"2022-05-08T23:24:39.8488653+02:00","temperatureC":-17,"temperatureF":2,"summary":"Balmy"},{"date":"2022-05-09T23:24:39.8488734+02:00","temperatureC":-15,"temperatureF":6,"summary":"Warm"},{"date":"2022-05-10T23:24:39.8488741+02:00","temperatureC":43,"temperatureF":109,"summary":"Scorching"},{"date":"2022-05-11T23:24:39.8488745+02:00","temperatureC":29,"temperatureF":84,"summary":"Mild"}]
```




## References
- https://garywoodfine.com/getting-started-with-net-core-microservices-with-dapr/