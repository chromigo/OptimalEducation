[![Stories in Ready](https://badge.waffle.io/chromigo/optimaleducation.png?label=ready&title=Ready)](https://waffle.io/chromigo/optimaleducation)
# OptimalEducation #

Web-сервис, помогающий абитуриентам(ученикам) выбрать ВУЗ, где им будет наиболее комфортно обучаться.

## Инструкция для запуска локально: ##

* Добавить в главный проект недостающие файлы с информацией о настройках подключения вида:

Credentials.config
```xml
<appSettings>
  <!--SendGrid auth-->
  <add key="SendGrid_Login" value="YOUR_Login"/>
  <add key="SendGrid_Password" value="YOUR_Password"/>
  <!--Google auth-->
  <add key="Google_ClientId" value="YOUR_ClientId"/>
  <add key="Google_ClientSecret" value="YOUR_ClientSecret"/>
  <!--Facebook auth-->
  <add key="Facebook_ClientId" value="YOUR_ClientId"/>
  <add key="Facebook_ClientSecret" value="YOUR_ClientSecret"/>
  <!--Twitter auth-->
  <add key="Twitter_ClientId" value="YOUR_ClientId"/>
  <add key="Twitter_ClientSecret" value="YOUR_ClientSecret"/>
  <!--Microsoft auth-->
  <add key="Microsoft_ClientId" value="YOUR_ClientId"/>
  <add key="Microsoft_ClientSecret" value="YOUR_ClientSecret"/>
</appSettings>
```
DatabaseCredentials.config
```xml
<connectionStrings>
  <add name="YOUR_DB_NAME" 
       providerName="System.Data.SqlClient" 
       connectionString="YOUR_CONNECTION_STRING" />
</connectionStrings>
```

* Выполнить построение базы данных
 Открыть Package Manager Console и ввести по очереди:

```
#!

 update-database -ProjectName: OptimalEducation
 update-database -ProjectName: OptimalEducation.DAL
```
 Должна создасться база данных с начальной информацией, необходимой для работы.
 Если нужно проинициализировать бд на Azure(или любом другом удаленном хосте) - вбиваем настройки connectionString нужного сервера в DatabaseCredentials.config и запускаем комманды выше.