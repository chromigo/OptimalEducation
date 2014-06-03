[![Stories in Ready](https://badge.waffle.io/chromigo/optimaleducation.png?label=ready&title=Ready)](https://waffle.io/chromigo/optimaleducation)
OptimalEducation
================

Web-сервис, помогающий абитуриентам(ученикам) выбрать ВУЗ, где им будет наиболее комфортно обучаться.

===
Особености запуска на Microsoft Azure и локально:
  1. Web.config - раскомментировать строку с подключением + залить файл с настройками DatabaseCredentials.config
  вида
  ```xml
	  <connectionStrings>
	  <add name="DBName" 
		   providerName="System.Data.SqlClient" 
		   connectionString="yourConnectionString;" />
	</connectionStrings>
	```
  2. Залить файл с настройками для email службы оповещения SendGrid  - EmailNotificationsCredentials.config вида
    ```xml
	<appSettings>
	  <add key="SendGrid_Login" value="yourLogin"/>
	  <add key="SendGrid_Password" value="yourPassword"/>
	</appSettings>
	  ```
  3. В App_Start\BundleConfig.cs
      ```csharp
	    BundleTable.EnableOptimizations = true; 
	  ```
  4. Выключить Debug режим в Web.Config
    ```xml
	<system.web>
	    <compilation debug="false" targetFramework="4.5.1" />
	</system.web>
	```
  Дополнительно+под вопросом:
  1. В Web.config убрать все ссылки на сборки незивестные(например профайлер)
  2. В Web.config  
  ```xml
  <system.web>
	  <customErrors mode="Off" />
	  ... 
  <.system.web>
  ```
  3. XsrfKey поменять?